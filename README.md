# 2D Game Engine

A simple 2D game engine using C# with a C++ core powered by Vulkan for graphics rendering.

## Architecture

- **Engine/**: C# interface layer using .NET 8.0
- **Core/**: C++ core engine with Vulkan and GLFW
- **Extern/**: External libraries (GLFW, Vulkan SDK)

## Prerequisites

- Visual Studio 2022 (with C++ and .NET workloads)
- CMake 3.20+
- .NET 8.0 SDK
- Vulkan-capable graphics card and drivers

## Quick Start

### Option 1: Using Batch Files

1. **Build the engine:**
   ```cmd
   build.bat
   ```

2. **Run the engine:**
   ```cmd
   run.bat
   ```

3. **Clean build directories:**
   ```cmd
   clean.bat
   ```

### Option 2: Using VS Code

1. Open the project in VS Code
2. Use Ctrl+Shift+P and run:
   - "Tasks: Run Task" → "Build" (to build)
   - "Tasks: Run Task" → "Clean" (to clean)
   - "Debug: Start Debugging" → "Build and Run" (to run)

### Option 3: Manual Build

1. **Build C++ Core:**
   ```cmd
   cd Core
   mkdir build
   cd build
   cmake .. -G "Visual Studio 17 2022" -A x64
   cmake --build . --config Release
   cd ../..
   ```

2. **Build C# Engine:**
   ```cmd
   cd Engine
   dotnet build --configuration Release
   cd ..
   ```

3. **Run:**
   ```cmd
   cd Build
   Engine.exe
   ```

## Project Structure

```
2DEngine/
├── Core/                   # C++ core engine
│   ├── include/           # Header files
│   ├── src/              # Source files
│   ├── build/            # Local C++ build folder
│   └── CMakeLists.txt    # CMake configuration
├── Engine/               # C# engine interface
│   ├── build/            # Local C# build folder
│   ├── EngineCore.cs     # DLL interop
│   ├── SimpleEngine.cs   # High-level engine class
│   ├── Program.cs        # Main entry point
│   └── Engine.csproj     # C# project file
├── Build/                # Global build output (all files copied here)
│   ├── Engine.exe        # C# executable
│   ├── EngineCore.dll    # C++ core DLL
│   ├── glfw3.dll         # GLFW dependency
│   ├── shaders/          # Compiled shaders (SPIR-V)
│   └── [other runtime files]
├── Shaders/              # GLSL shaders and compiled output
│   ├── triangle.vert     # Vertex shader source
│   ├── triangle.frag     # Fragment shader source
│   └── compiled/         # Compiled SPIR-V shaders
├── Extern/               # External libraries
│   ├── glfw-3.4.bin.WIN64/
│   └── VulkanSDK/
├── .vscode/              # VS Code configuration
├── build.bat             # Build script
├── run.bat               # Run script
├── clean.bat             # Clean script
└── README.md
```

## Features

- ✅ GLFW window creation
- ✅ Vulkan initialization
- ✅ Basic render loop
- ✅ C# to C++ interop
- ✅ Triangle rendering with colored vertices
- ✅ GLSL shader compilation (SPIR-V)
- ✅ Graphics pipeline with proper shaders
- ❌ Texture support
- ❌ 2D sprite rendering
- ❌ Input handling

## Notes

This is a minimal engine setup focused on getting a Vulkan window running with C# interface. Features a complete triangle rendering pipeline with:
- GLSL shader compilation to SPIR-V
- Vulkan graphics pipeline with vertex/fragment shaders  
- Colored triangle rendering with interpolated vertex colors

## Build Process

The build process works as follows:
1. **Shader Compilation** - GLSL shaders compiled to SPIR-V using `glslc`
2. **C++ Core** builds to `Core/build/` locally
3. **C# Engine** builds to `Engine/build/` locally  
4. **build.bat** copies all required files to the global `Build/` directory
5. **run.bat** executes from the `Build/` directory

## Troubleshooting

- **CMake not found**: Install CMake and add it to your PATH
- **dotnet not found**: Install .NET 8.0 SDK
- **Vulkan errors**: Ensure you have Vulkan-compatible drivers installed
- **Build errors**: Check that all prerequisites are installed and Visual Studio 2022 is available
- **Missing files in Build/**: Run `build.bat` to ensure all files are copied correctly
- **Weird build issues**: Try running `clean.bat` then `build.bat` for a fresh build 