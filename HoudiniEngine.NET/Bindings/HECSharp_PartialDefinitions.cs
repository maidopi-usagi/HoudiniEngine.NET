using HoudiniEngine.NET;

namespace HoudiniEngineCSharp;
public partial struct HAPI_CookOptions
{
    public static HAPI_CookOptions Default => new()
    {
        curveRefineLOD = 8.0f,
        clearErrorsAndWarnings = false,
        maxVerticesPerPrimitive = 3,
        splitGeosByGroup = false,
        refineCurveToLinear = true,
        handleBoxPartTypes = false,
        handleSpherePartTypes = false,
        splitPointsByVertexAttributes = false,
        packedPrimInstancingMode = HAPI_PackedPrimInstancingMode.HAPI_PACKEDPRIM_INSTANCING_MODE_FLAT
    };
}

public partial struct HAPI_ParmInfo
{
}