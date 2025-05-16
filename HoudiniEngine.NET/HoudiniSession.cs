using System.Text;
using HoudiniEngineCSharp;

namespace HoudiniEngine.NET;

public class HoudiniSession : IDisposable
{
    private readonly HAPI_SessionInfo _sessionInfo;
    private HAPI_Session _session;

    private readonly Dictionary<string, int> _houdiniStringCache = [];
    private readonly Dictionary<int, string> _stringMap = [];
    
    private readonly List<HoudiniAssetLibrary> _loadedLibraries = [];
    
    public IEnumerable<HoudiniAssetLibrary> LoadedLibraries => _loadedLibraries;

    public HoudiniSession() : this(new HAPI_SessionInfo()) { }

    public HoudiniSession(HAPI_SessionInfo sessionInfo)
    {
        HECSharp_Functions.HAPI_ClearConnectionError();
        _sessionInfo = sessionInfo;
        HECSharp_Functions.HAPI_CreateInProcessSession(out _session, ref _sessionInfo).Ok();
        var dummy = Array.Empty<byte>();
        var cookOptions = HAPI_CookOptions.Default;
        HECSharp_Functions.HAPI_Initialize(ref _session, ref cookOptions, true, -1, dummy, dummy, dummy, dummy, dummy).Ok();

    }

    public HoudiniAssetLibrary LoadAssetLibrary(string otlPath)
    {
        var filePath = Encoding.UTF8.GetBytes(otlPath);
        HECSharp_Functions.HAPI_LoadAssetLibraryFromFile(ref _session, filePath, false, out var libraryId).Ok();
        var lib = new HoudiniAssetLibrary(this, libraryId, otlPath);
        _loadedLibraries.Add(lib);
        return lib;
    }

    public int GetHStringHandle(string fmt)
    {
        if (_houdiniStringCache.TryGetValue(fmt, out var handle)) return handle;
        HECSharp_Functions.HAPI_SetCustomString(ref _session, Encoding.UTF8.GetBytes(fmt), out var newHandle).Ok();
        handle = newHandle;
        _houdiniStringCache[fmt] = handle;
        _stringMap[handle] = fmt;
        return handle;
    }

    public string GetHString(int handle)
    {
        if (_stringMap.TryGetValue(handle, out var result)) return result;
        result = _session.GetHString(handle);
        _stringMap[handle] = result;
        return result;
    }

    internal ref HAPI_Session GetRef() => ref _session;

    public void Dispose()
    {
        if (!HECSharp_Functions.HAPI_IsSessionValid(ref _session).Ok()) return;
        Console.WriteLine("Session valid, Closing...");
        HECSharp_Functions.HAPI_Cleanup(ref _session);
        HECSharp_Functions.HAPI_Shutdown(ref _session);
        HECSharp_Functions.HAPI_CloseSession(ref _session);
    }

    public HoudiniNode CreateNode(string opName, string label, HoudiniNode? parent = null, bool cookOnCreation = false)
    {
        HECSharp_Functions.HAPI_CreateNode(ref _session, parent?.Id ?? -1,
            Encoding.UTF8.GetBytes(opName),
            Encoding.UTF8.GetBytes(label),
            cookOnCreation, out var id).Ok();
        return new HoudiniNode(this, id);
    }

    public void SetToHip(string path)
    {
        
    }
}