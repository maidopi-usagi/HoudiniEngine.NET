# HoudiniEngine.NET

WIP!

HoudiniEngine.NET is a C# binding wrapper library for Houdini Engine on the .NET platform, supporting multiple platforms (Windows, Linux, macOS). It enables loading and manipulating Houdini Digital Assets (HDA).

## Features

- Multi-platform support (win-x64, linux-x64, osx-x64, osx-arm64)
- Wraps Houdini Engine C API for easier .NET usage
- Supports loading OTL/HDA assets, node operations, parameter access, geometry data reading, and more

## Getting Started

1. **Configure Houdini Path**

   Before first use, make sure to set the `HOUDINI_PATH` property in [HoudiniEngine.NET/HoudiniEngine.NET.csproj](HoudiniEngine.NET/HoudiniEngine.NET.csproj) to the path of your local Houdini installation. For example:

   ```xml
   <PropertyGroup>
       <HOUDINI_PATH>/Applications/Houdini/Houdini20.5.487/Frameworks/Houdini.framework/Versions/20.5</HOUDINI_PATH>
       <ENGINE_PATH>$(HOUDINI_PATH)/Resources</ENGINE_PATH>
       <HOUDINI_LIB_PATH>$(HOUDINI_PATH)/Libraries</HOUDINI_LIB_PATH>
   </PropertyGroup>
   ```

   > ⚠️ If `HOUDINI_PATH` is not set correctly, the build or runtime will fail to locate Houdini libraries.

2. **Build the Project**

   ```sh
   dotnet build
   ```

3. **Run Example**

   ```sh
   dotnet run --project HoudiniEngine.NET.Example
   ```

## Basic Usage

Below is a minimal example of how to use HoudiniEngine.NET to load a Houdini Digital Asset (HDA), create a node from it, and access geometry information and its buffer:

```csharp
using HoudiniEngine.NET;

BindingResolver.Initialize();

using var session = new HoudiniSession();
var library = session.LoadAssetLibrary("/path/to/your_asset.hda");
var assetName = library.Assets.First();
var node = session.CreateNode(assetName, assetName);

// Get geometry from the node
var geo = node.GetGeometry();

// Access geometry buffer (e.g., positions)
var indices = geo.GetVertices();
var posBuffer = geo.GetFloatBuffer<Vector3>("P", HAPI_AttributeOwner.HAPI_ATTROWNER_POINT);
var colorBuffer = geo.GetFloatBuffer<Vector3>("Cd", HAPI_AttributeOwner.HAPI_ATTROWNER_POINT);
var normalBuffer = geo.GetFloatBuffer<Vector3>("N", HAPI_AttributeOwner.HAPI_ATTROWNER_VERTEX);
```

Replace `/path/to/your_asset.hda` with the actual path to your HDA file. This code demonstrates initializing the session, loading an asset library, creating a node

## Project Structure

- `HoudiniEngine.NET/` Main library source code
- `HoudiniEngine.NET.Example/` Example program
- `Bindings/` Auto-generated C# binding files
- `README.md` This documentation file

## License

MIT License

---

For detailed documentation and API reference, please stay tuned for future updates.


