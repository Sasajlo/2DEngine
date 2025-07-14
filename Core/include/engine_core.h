#pragma once
#include <cstdint>

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
    
    // Input handling
    ENGINE_API bool IsKeyPressed(int keyCode);
    ENGINE_API bool IsKeyHeld(int keyCode);
    ENGINE_API bool IsKeyReleased(int keyCode);
    ENGINE_API float GetMouseScrollDelta();
    ENGINE_API void ResetMouseScrollDelta();
    ENGINE_API void GetMousePosition(float* x, float* y);
    
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
    
    // Efficient sprite rendering with texture index
    ENGINE_API void RenderSpriteWithIndex(
        uint32_t textureIndex,
        float* worldMatrix,
        float* color,
        float* size,
        int sortingOrder,
        bool flipX,
        bool flipY
    );
    
    // Chunk mesh rendering (new)
    ENGINE_API void RenderChunkMesh(
        float* vertices,           // Array of vertex positions (x, y, z)
        uint32_t vertexCount,      // Number of vertices
        uint32_t* indices,         // Array of vertex indices
        uint32_t indexCount,       // Number of indices
        uint32_t* textureIndices,  // Array of texture indices per quad (6 indices = 1 quad)
        float* colors,             // Array of colors per quad (rgba for each quad)
        uint32_t quadCount,        // Number of quads in the mesh
        float tileSize             // Size of each tile in world units
    );
    
    // Camera
    ENGINE_API void SetCameraMatrices(float* viewMatrix, float* projectionMatrix);
    
    // Texture management
    ENGINE_API uint32_t LoadTexture(const char* texturePath);
} 