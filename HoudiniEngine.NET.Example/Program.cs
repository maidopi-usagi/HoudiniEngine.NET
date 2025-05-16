using HoudiniEngine.NET;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using HoudiniEngineCSharp;

BindingResolver.Initialize();

var cts = new CancellationTokenSource();
var spinnerTask = Task.Run(() =>
{
    var spinner = new[] { '|', '/', '-', '\\' };
    int idx = 0;
    Console.Write("[Info] Loading HoudiniSession ");
    while (!cts.Token.IsCancellationRequested)
    {
        Console.Write($"\r[Info] Loading HoudiniSession {spinner[idx++ % spinner.Length]}");
        Thread.Sleep(100);
    }
    Console.Write("\r[Info] HoudiniSession loaded.         \n");
});

HoudiniSession session = null;
await Task.Run(() =>
{
    session = new HoudiniSession();
});

cts.Cancel();
await spinnerTask;

if (session != null)
{
    HoudiniAssetLibrary library = null;
    while (true)
    {
        Console.Write("Please input the path to a .hda file to load: ");
        var hdaPath = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(hdaPath) && File.Exists(hdaPath))
        {
            try
            {
                library = session.LoadAssetLibrary(hdaPath);
                Console.WriteLine($"Successfully loaded asset library: {hdaPath}");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load asset library: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("File not found or invalid input.");
        }
    }
    if (library != null)
    {
        Console.WriteLine($"Loaded library: {library.LibraryPath}");
        var assets = library.Assets.ToArray();
        for (int i = 0; i < assets.Length; i++)
        {
            Console.WriteLine($"[{i}] {assets[i]}");
        }
        HoudiniNode node = null;
        while (true)
        {
            Console.Write("Please input the asset name or index to load: ");
            var input = Console.ReadLine();
            string assetName = null;
            if (int.TryParse(input, out int idx) && idx >= 0 && idx < assets.Length)
            {
                assetName = assets[idx];
            }
            else if (!string.IsNullOrWhiteSpace(input))
            {
                assetName = assets.FirstOrDefault(a => a.Equals(input, StringComparison.OrdinalIgnoreCase));
            }
            if (assetName != null)
            {
                try
                {
                    node = session.CreateNode(assetName, $"Node_{assetName}");
                    Console.WriteLine($"Successfully created node for asset: {assetName}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create node: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Asset not found. Please input a valid asset name or index.");
            }
        }
        if (node != null)
        {
            Console.WriteLine($"Node ID: {node.Id}");
            node.RefreshParameters();
            foreach (var parameter in node.Parameters)
            {
                Console.WriteLine(parameter);
            }
            var geo = node.GetGeometry();
            var indices = geo.GetVertices();
            var posBuffer = geo.GetFloatBuffer<System.Numerics.Vector3>("P", HAPI_AttributeOwner.HAPI_ATTROWNER_POINT);
            var colorBuffer = geo.GetFloatBuffer<System.Numerics.Vector3>("Cd", HAPI_AttributeOwner.HAPI_ATTROWNER_POINT);
            var normalBuffer = geo.GetFloatBuffer<System.Numerics.Vector3>("N", HAPI_AttributeOwner.HAPI_ATTROWNER_VERTEX);
            Console.WriteLine($"Vertices: {indices.Length}");
            Console.WriteLine($"Positions: {posBuffer.Length}");
            Console.WriteLine($"Colors: {colorBuffer.Length}");
        }
    }
    session.Dispose();
}

return 0;
