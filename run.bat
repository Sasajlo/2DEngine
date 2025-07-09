@echo off
echo ========================================
echo Running 2D Engine
echo ========================================

:: Check if the engine executable exists
if not exist "Build\Engine.exe" (
    echo ERROR: Engine.exe not found in Build directory.
    echo Please run build.bat first to compile the engine.
    pause
    exit /b 1
)

:: Check if the core DLL exists
if not exist "Build\EngineCore.dll" (
    echo ERROR: EngineCore.dll not found in Build directory.
    echo Please run build.bat first to compile the engine.
    pause
    exit /b 1
)

:: Check if GLFW DLL exists
if not exist "Build\glfw3.dll" (
    echo ERROR: glfw3.dll not found in Build directory.
    echo This should be copied automatically during build.
    pause
    exit /b 1
)

echo Starting engine...
echo Close the window to exit.
echo.

:: Run the engine
cd Build
Engine.exe
set exit_code=%errorlevel%
cd ..

echo.
if %exit_code% equ 0 (
    echo Engine exited normally.
) else (
    echo Engine exited with error code: %exit_code%
)

pause 