using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using HoudiniEngine.NET;
using HoudiniEngineCSharp;
using HAPI = HoudiniEngineCSharp.HECSharp_Functions;

namespace HoudiniEngine.NET;

public static class HAPIExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Ok(this HAPI_Result result)
    {
        if (result == HAPI_Result.HAPI_RESULT_SUCCESS) return true;
        throw new Exception(result.ToString());
    }
    
    
    public static string GetHString(this ref HAPI_Session session, int handle)
    {
        HAPI.HAPI_GetStringBufLength(ref session, handle, out var bufferLength);
        var buffer = ArrayPool<byte>.Shared.Rent(bufferLength);
        HAPI.HAPI_GetString(ref session, handle, buffer, bufferLength);
        var decoded = Encoding.UTF8.GetString(buffer);
        ArrayPool<byte>.Shared.Return(buffer);
        return decoded;
    }
}

