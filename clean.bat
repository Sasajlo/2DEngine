@echo off
echo ========================================
echo Cleaning 2D Engine Build Directories
echo ========================================

:: Remove global Build directory
if exist "Build" (
    echo Removing global Build directory...
    rmdir /s /q "Build"
    if %errorlevel% equ 0 (
        echo Global Build directory removed successfully!
    ) else (
        echo Warning: Failed to remove global Build directory
    )
) else (
    echo Global Build directory does not exist
)

:: Remove C++ Core build directory
if exist "Core\build" (
    echo Removing C++ Core build directory...
    rmdir /s /q "Core\build"
    if %errorlevel% equ 0 (
        echo C++ Core build directory removed successfully!
    ) else (
        echo Warning: Failed to remove C++ Core build directory
    )
) else (
    echo C++ Core build directory does not exist
)

:: Remove C# Engine build directory
if exist "Engine\build" (
    echo Removing C# Engine build directory...
    rmdir /s /q "Engine\build"
    if %errorlevel% equ 0 (
        echo C# Engine build directory removed successfully!
    ) else (
        echo Warning: Failed to remove C# Engine build directory
    )
) else (
    echo C# Engine build directory does not exist
)

:: Remove C# Engine obj directory
if exist "Engine\obj" (
    echo Removing C# Engine obj directory...
    rmdir /s /q "Engine\obj"
    if %errorlevel% equ 0 (
        echo C# Engine obj directory removed successfully!
    ) else (
        echo Warning: Failed to remove C# Engine obj directory
    )
) else (
    echo C# Engine obj directory does not exist
)

:: Remove compiled shaders
if exist "Shaders\compiled" (
    echo Removing compiled shaders directory...
    rmdir /s /q "Shaders\compiled"
    if %errorlevel% equ 0 (
        echo Compiled shaders directory removed successfully!
    ) else (
        echo Warning: Failed to remove compiled shaders directory
    )
) else (
    echo Compiled shaders directory does not exist
)

echo.
echo ========================================
echo Clean completed!
echo ========================================
echo.
echo You can now run build.bat to rebuild everything
pause 