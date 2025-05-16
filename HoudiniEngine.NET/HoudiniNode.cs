using System.Buffers;
using HoudiniEngineCSharp;
using HAPI = HoudiniEngineCSharp.HECSharp_Functions;

namespace HoudiniEngine.NET;

public class HoudiniNode : IDisposable
{
    private readonly int _nodeId;
    private readonly HoudiniSession _session;
    
    public HoudiniSession Session => _session;

    private readonly List<IHoudiniParameter> _parameters = [];
    
    public IEnumerable<IHoudiniParameter> Parameters => _parameters;

    public int Id => _nodeId;

    internal HoudiniNode(HoudiniSession session, int nodeId)
    {
        _session = session;
        _nodeId = nodeId;
    }

    public void RefreshParameters()
    {
        var nodeInfo = GetNodeInfo();
        
        var parmBuffer = ArrayPool<HAPI_ParmInfo>.Shared.Rent(nodeInfo.parmCount);

        HAPI.HAPI_GetParameters(ref _session.GetRef(), _nodeId, parmBuffer, 0, nodeInfo.parmCount).Ok();
        
        _parameters.Clear();
        
        foreach (var parm in parmBuffer.AsSpan()[..nodeInfo.parmCount])
        {
            var parsedParm = MakeParameter(parm);
            if (parsedParm != null) _parameters.Add(parsedParm);
        }
        
        ArrayPool<HAPI_ParmInfo>.Shared.Return(parmBuffer);
    }

    public void Cook(HAPI_CookOptions cookOptions)
    {
        ref var session = ref _session.GetRef();
        HAPI.HAPI_CookNode(ref session, _nodeId, ref cookOptions);
        HAPI.HAPI_GetStatus(ref session, HAPI_StatusType.HAPI_STATUS_COOK_STATE, out var status);
        while (status > (int)HAPI_State.HAPI_STATE_READY)
        {
            HAPI.HAPI_GetStatus(ref session, HAPI_StatusType.HAPI_STATUS_COOK_STATE, out status);
            Thread.Sleep(15);
        }
    }

    public async Task CookAsync(HAPI_CookOptions cookOptions)
    {
        await Task.Run(() => Cook(cookOptions));
    }

    public HAPI_PartInfo GetPartInfo()
    {
        HAPI.HAPI_GetPartInfo(ref _session.GetRef(), 0, 0, out var partInfo).Ok();
        return partInfo;
    }

    public HAPI_NodeInfo GetNodeInfo()
    {
        HAPI.HAPI_GetNodeInfo(ref _session.GetRef(), _nodeId, out var nodeInfo).Ok();
        return nodeInfo;
    }

    public HoudiniGeometry GetGeometry()
    {
        return new HoudiniGeometry(this);
    }

    public void Dispose()
    {
        if (HAPI.HAPI_IsSessionValid(ref _session.GetRef()).Ok())
        {
            HAPI.HAPI_DeleteNode(ref _session.GetRef(), _nodeId);
            Console.WriteLine($"Deleted node {_nodeId}");
        }
    }

    private IHoudiniParameter? MakeParameter(HAPI_ParmInfo parmInfo)
    {
        var size = parmInfo.size;
        var name = _session.GetHString(parmInfo.nameSH);
        IHoudiniParameter? parsed = null;
        switch (parmInfo.type)
        {
            case HAPI_ParmType.HAPI_PARMTYPE_INT:
            {
                var intValue = ArrayPool<int>.Shared.Rent(size);
                HAPI.HAPI_GetParmIntValues(ref _session.GetRef(), _nodeId, intValue, parmInfo.intValuesIndex, size);
                parsed = new HoudiniParameterInt(parmInfo, name, intValue.AsSpan()[..size]);
                ArrayPool<int>.Shared.Return(intValue);
                break;
            }
            case HAPI_ParmType.HAPI_PARMTYPE_STRING:
            {
                var strValue = ArrayPool<int>.Shared.Rent(size);
                HAPI.HAPI_GetParmStringValues(ref _session.GetRef(), _nodeId, true, strValue, parmInfo.stringValuesIndex, size);
                parsed = new HoudiniParameterString(parmInfo, name, strValue[..size].Select(handle => _session.GetHString(handle)).ToArray());
                ArrayPool<int>.Shared.Return(strValue);
                break;
            }
            case HAPI_ParmType.HAPI_PARMTYPE_FLOAT:
            {
                var floatValue = ArrayPool<float>.Shared.Rent(size);
                HAPI.HAPI_GetParmFloatValues(ref _session.GetRef(), _nodeId, floatValue, parmInfo.intValuesIndex, size);
                parsed = new HoudiniParameterFloat(parmInfo, name, floatValue.AsSpan()[..size]);
                ArrayPool<float>.Shared.Return(floatValue);
                break;
            }
            case HAPI_ParmType.HAPI_PARMTYPE_TOGGLE:
            {
                var intValue = ArrayPool<int>.Shared.Rent(size);
                HAPI.HAPI_GetParmIntValues(ref _session.GetRef(), _nodeId, intValue, parmInfo.intValuesIndex, size);
                parsed = new HoudiniParameterBool(parmInfo, name, intValue[0] > 0);
                ArrayPool<int>.Shared.Return(intValue);
                break;
            }
            case HAPI_ParmType.HAPI_PARMTYPE_MULTIPARMLIST:
            case HAPI_ParmType.HAPI_PARMTYPE_BUTTON:
            case HAPI_ParmType.HAPI_PARMTYPE_COLOR:
            case HAPI_ParmType.HAPI_PARMTYPE_PATH_FILE:
            case HAPI_ParmType.HAPI_PARMTYPE_PATH_FILE_GEO:
            case HAPI_ParmType.HAPI_PARMTYPE_PATH_FILE_IMAGE:
            case HAPI_ParmType.HAPI_PARMTYPE_NODE:
            case HAPI_ParmType.HAPI_PARMTYPE_FOLDERLIST:
            case HAPI_ParmType.HAPI_PARMTYPE_FOLDERLIST_RADIO:
            case HAPI_ParmType.HAPI_PARMTYPE_FOLDER:
            case HAPI_ParmType.HAPI_PARMTYPE_LABEL:
            case HAPI_ParmType.HAPI_PARMTYPE_SEPARATOR:
            case HAPI_ParmType.HAPI_PARMTYPE_PATH_FILE_DIR:
            case HAPI_ParmType.HAPI_PARMTYPE_MAX:
                Console.WriteLine($"Warning: {parmInfo.type} is not implemented yet");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return parsed;
    }
}