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

## Project Structure

- `HoudiniEngine.NET/` Main library source code
- `HoudiniEngine.NET.Example/` Example program
- `Bindings/` Auto-generated C# binding files
- `README.md` This documentation file

## License

MIT License

---

For detailed documentation and API reference, please stay tuned for future updates.


