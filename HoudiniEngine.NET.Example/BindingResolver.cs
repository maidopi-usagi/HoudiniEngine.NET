using System.Reflection;
using System.Runtime.InteropServices;
using HoudiniEngine.NET;
using HoudiniEngineCSharp;

public static class BindingResolver
{
    private static IntPtr _hapiLibHandle;
    private static IntPtr _harcLibHandle;

    public static void Initialize()
    {
        var libExt = string.Empty;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            libExt = "dylib";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            libExt = "so";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            libExt = "dll";
        }

        _hapiLibHandle = NativeLibrary.Load($"{PathConstants.HOUDINI_LIB_PATH}/{HECSharp_HoudiniVersion.HAPI_LIBRARY}.{libExt}");
        _harcLibHandle = NativeLibrary.Load($"{PathConstants.HOUDINI_LIB_PATH}/{HECSharp_HoudiniVersion.HARC_LIBRARY}.{libExt}");
        
        NativeLibrary.SetDllImportResolver(typeof(HECSharp_HoudiniVersion).Assembly, Resolve);
    }

    private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        return libraryName switch
        {
            HECSharp_HoudiniVersion.HAPI_LIBRARY => _hapiLibHandle,
            HECSharp_HoudiniVersion.HARC_LIBRARY => _harcLibHandle,
            _ => IntPtr.Zero
        };
    }
}