#pragma once

#ifdef ENGINE_CORE_EXPORTS
#define ENGINE_API __declspec(dllexport)
#else
#define ENGINE_API __declspec(dllimport)
#endif

extern "C" {
    // Engine initialization and cleanup
    ENGINE_API bool InitializeEngine(int width, int height, const char* title);
    ENGINE_API void ShutdownEngine();
    
    // Main loop
    ENGINE_API bool ShouldClose();
    ENGINE_API void PollEvents();
    ENGINE_API void Render();
    
    // Window management
    ENGINE_API void SetWindowTitle(const char* title);
    ENGINE_API void GetWindowSize(int* width, int* height);
    
    // Time management
    ENGINE_API void UpdateDeltaTime();
    ENGINE_API float GetDeltaTime();
    ENGINE_API double GetTime();
    ENGINE_API void ResetTime();
    
    // Sprite rendering
    ENGINE_API void RenderSprite(
        const char* texturePath,
        float* worldMatrix,
        float* color,
        float* size,
        int sortingOrder,
        bool flipX,
        bool flipY
    );
    
    // Camera
    ENGINE_API void SetCameraMatrices(float* viewMatrix, float* projectionMatrix);
} 