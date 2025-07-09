@echo off
echo ========================================
echo Building 2D Engine
echo ========================================

:: Check if cmake is available
cmake --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: CMake not found. Please install CMake and add it to PATH.
    pause
    exit /b 1
)

:: Check if dotnet is available
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK not found. Please install .NET 8.0 SDK.
    pause
    exit /b 1
)

:: Compile Shaders
echo.
echo Compiling Shaders...
echo ========================================

if not exist "Shaders\compiled" mkdir "Build\shaders"

:: Check if glslc is available
"Extern\VulkanSDK\1.4.313.1\Bin\glslc.exe" --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: glslc not found. Please check Vulkan SDK installation.
    pause
    exit /b 1
)

:: Compile vertex shader
"Extern\VulkanSDK\1.4.313.1\Bin\glslc.exe" "Shaders\triangle.vert" -o "Build\shaders\triangle_vert.spv"
if %errorlevel% neq 0 (
    echo ERROR: Failed to compile vertex shader!
    pause
    exit /b 1
)

:: Compile fragment shader
"Extern\VulkanSDK\1.4.313.1\Bin\glslc.exe" "Shaders\triangle.frag" -o "Build\shaders\triangle_frag.spv"
if %errorlevel% neq 0 (
    echo ERROR: Failed to compile fragment shader!
    pause
    exit /b 1
)

echo Shaders compiled successfully!

:: Build C++ Core (DLL)
echo.
echo Building C++ Core...
echo ========================================

cd Core
if not exist "build" mkdir build
cd build

:: Configure with CMake
cmake .. -G "Visual Studio 17 2022" -A x64
if %errorlevel% neq 0 (
    echo ERROR: CMake configuration failed!
    cd ..\..
    pause
    exit /b 1
)

:: Build the project
cmake --build . --config Release
if %errorlevel% neq 0 (
    echo ERROR: C++ build failed!
    cd ..\..
    pause
    exit /b 1
)

cd ..\..

echo C++ Core built successfully!

:: Build C# Engine
echo.
echo Building C# Engine...
echo ========================================

cd Engine
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: C# build failed!
    cd ..
    pause
    exit /b 1
)

cd ..

echo C# Engine built successfully!

:: Create global Build directory and copy all required files
echo.
echo Setting up global Build directory...
echo ========================================

if not exist "Build" mkdir Build

:: Copy C++ Core DLL and dependencies
copy "Core\build\bin\Release\EngineCore.dll" "Build\" >nul 2>&1
if not exist "Build\EngineCore.dll" (
    echo ERROR: Failed to copy EngineCore.dll
    pause
    exit /b 1
)

copy "Core\build\bin\Release\glfw3.dll" "Build\" >nul 2>&1
if not exist "Build\glfw3.dll" (
    echo ERROR: Failed to copy glfw3.dll
    pause
    exit /b 1
)

:: Copy C# Engine executable and dependencies
copy "Engine\build\Engine.exe" "Build\" >nul 2>&1
if not exist "Build\Engine.exe" (
    echo ERROR: Failed to copy Engine.exe from Engine\build\
    echo Checking if file exists...
    if exist "Engine\build\Engine.exe" (
        echo File exists but copy failed
    ) else (
        echo File does not exist at expected location
        echo Listing Engine\build contents:
        dir "Engine\build" /s
    )
    pause
    exit /b 1
)

copy "Engine\build\Engine.dll" "Build\" >nul 2>&1
copy "Engine\build\Engine.runtimeconfig.json" "Build\" >nul 2>&1
copy "Engine\build\Engine.deps.json" "Build\" >nul 2>&1

echo C# Engine files copied successfully!

echo All files copied to Build directory successfully!

echo.
echo ========================================
echo Build completed successfully!
echo ========================================
echo.
echo You can now run the engine using run.bat