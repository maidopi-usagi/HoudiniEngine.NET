using System.Buffers;
using System.Collections.Immutable;
using HoudiniEngineCSharp;

namespace HoudiniEngine.NET;

public class HoudiniAssetLibrary
{
    private readonly int _assetLibraryId;
    private readonly HoudiniSession _session;
    public string LibraryPath { get; private set; }

    public readonly ImmutableArray<string> Assets;

    internal HoudiniAssetLibrary(HoudiniSession session, int assetLibraryId, string name)
    {
        LibraryPath = name;
        _assetLibraryId = assetLibraryId;
        _session = session;
        HECSharp_Functions.HAPI_GetAvailableAssetCount(ref _session.GetRef(), _assetLibraryId, out var assetCount).Ok();
        var assetNames = ArrayPool<int>.Shared.Rent(assetCount);
        HECSharp_Functions.HAPI_GetAvailableAssets(ref _session.GetRef(), _assetLibraryId, assetNames, assetCount);
        var list = new List<string>();
        foreach (var nameHandle in assetNames.AsSpan()[..assetCount])
        {
            list.Add(_session.GetHString(nameHandle));
        }

        Assets = [..list];
        ArrayPool<int>.Shared.Return(assetNames);
    }
}