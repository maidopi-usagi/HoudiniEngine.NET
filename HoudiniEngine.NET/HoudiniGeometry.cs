using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using HoudiniEngineCSharp;
using HAPI = HoudiniEngineCSharp.HECSharp_Functions;

namespace HoudiniEngine.NET;

[Flags]
public enum EGeometryData
{
    Position = 1 << 0,
    Normal = 1 << 1,
    Tangent = 1 << 2,
    Binormal = 1 << 3,
    Bitangent = 1 << 4,
    Index = 1 << 5,
    TexCoord0 = 1 << 6,
    TexCoord1 = 1 << 7,
    TexCoord2 = 1 << 8,
    TexCoord3 = 1 << 9,
    BoneCapture = 1 << 10,
    Max = 1 << 11,
    Default = Position | Index
}

public class HoudiniGeometry
{
    private HAPI_GeoInfo _displayGeoInfo;
    private HAPI_PartInfo _geoPartInfo;
    private readonly HoudiniNode _node;
    
    internal HoudiniGeometry(HoudiniNode node)
    {
        _node = node;
        node.Cook(HAPI_CookOptions.Default);
        ref var session = ref node.Session.GetRef();
        HAPI.HAPI_GetDisplayGeoInfo(ref session, node.Id, out _displayGeoInfo).Ok();
        HAPI.HAPI_GetPartInfo(ref session, _displayGeoInfo.nodeId, 0, out _geoPartInfo).Ok();

#if false // Option has been set to auto-triangulated.
        var faceCountBuffer = ArrayPool<int>.Shared.Rent(geoPartInfo.faceCount);
        HAPI.HAPI_GetFaceCounts(ref session, geoInfo.nodeId, geoPartInfo.id, faceCountBuffer, 0, geoPartInfo.faceCount).Ok();
#endif
    }

    public int[] GetVertices()
    {
        ref var session = ref _node.Session.GetRef();
        var indiciesBuffer = new int[_geoPartInfo.vertexCount];
        HAPI.HAPI_GetVertexList(ref session, _displayGeoInfo.nodeId, _geoPartInfo.id, indiciesBuffer, 0, _geoPartInfo.vertexCount).Ok();
        return indiciesBuffer;
    }

    public float[] GetFloatBuffer(string attrName, HAPI_AttributeOwner owner)
    {
        ref var session = ref _node.Session.GetRef();
        var attrUtf8 = Encoding.UTF8.GetBytes(attrName);
        HAPI.HAPI_GetAttributeInfo(ref session, _displayGeoInfo.nodeId, _geoPartInfo.id, attrUtf8,
            owner, out var attrInfo).Ok();
        if (attrInfo.storage != HAPI_StorageType.HAPI_STORAGETYPE_FLOAT) return [];
        var buffer = new float[attrInfo.count];
        HAPI.HAPI_GetAttributeFloatData(ref session, _displayGeoInfo.nodeId, _geoPartInfo.id, attrUtf8,
            ref attrInfo, -1, buffer, 0, attrInfo.count);
        return buffer;
    }

    public T[] GetFloatBuffer<T>(string attrName, HAPI_AttributeOwner owner) where T : struct
    {
        ref var session = ref _node.Session.GetRef();
        var attrUtf8 = Encoding.UTF8.GetBytes(attrName);
        HAPI.HAPI_GetAttributeInfo(ref session, _displayGeoInfo.nodeId, _geoPartInfo.id, attrUtf8,
            owner, out var attrInfo).Ok();
        if (attrInfo.storage != HAPI_StorageType.HAPI_STORAGETYPE_FLOAT) return [];
        if (attrInfo.tupleSize != Unsafe.SizeOf<T>() / sizeof(float)) return [];
        var buffer = ArrayPool<float>.Shared.Rent(attrInfo.count * attrInfo.tupleSize);
        HAPI.HAPI_GetAttributeFloatData(ref session, _displayGeoInfo.nodeId, _geoPartInfo.id, attrUtf8,
            ref attrInfo, -1, buffer, 0, attrInfo.count);
        var result = MemoryMarshal.Cast<float, T>(buffer).ToArray();
        ArrayPool<float>.Shared.Return(buffer);
        return result;
    }

    public int[] GetIntBuffer(string attrName, HAPI_AttributeOwner owner)
    {
        ref var session = ref _node.Session.GetRef();
        var attrUtf8 = Encoding.UTF8.GetBytes(attrName);
        HAPI.HAPI_GetAttributeInfo(ref session, _displayGeoInfo.nodeId, _geoPartInfo.id, attrUtf8,
            owner, out var attrInfo).Ok();
        if (attrInfo.storage != HAPI_StorageType.HAPI_STORAGETYPE_INT) return [];
        var buffer = new int[attrInfo.count];
        HAPI.HAPI_GetAttributeIntData(ref session, _displayGeoInfo.nodeId, _geoPartInfo.id, attrUtf8,
            ref attrInfo, -1, buffer, 0, attrInfo.count);
        return buffer;
    }

    public string[] GetStringBuffer(string attrName, HAPI_AttributeOwner owner)
    {
        ref var session = ref _node.Session.GetRef();
        var attrUtf8 = Encoding.UTF8.GetBytes(attrName);
        HAPI.HAPI_GetAttributeInfo(ref session, _displayGeoInfo.nodeId, _geoPartInfo.id, attrUtf8,
            owner, out var attrInfo).Ok();
        if (attrInfo.storage != HAPI_StorageType.HAPI_STORAGETYPE_STRING) return [];
        var buffer = ArrayPool<int>.Shared.Rent(attrInfo.count);
        HAPI.HAPI_GetAttributeStringData(ref session, _displayGeoInfo.nodeId, _geoPartInfo.id, attrUtf8,
            ref attrInfo, buffer, 0, attrInfo.count);
        var result = buffer.Select(handle => _node.Session.GetHString(handle)).ToArray();
        ArrayPool<int>.Shared.Return(buffer);
        return result;
    }
}