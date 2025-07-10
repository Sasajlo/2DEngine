#include "engine_core.h"
#include "window.h"
#include "vulkan_renderer.h"
#include <memory>

static std::unique_ptr<Window> g_window;
static std::unique_ptr<VulkanRenderer> g_renderer;

// Time management variables
static double g_currentTime = 0.0;
static double g_lastTime = 0.0;
static float g_deltaTime = 0.0f;
static double g_startTime = 0.0;

extern "C" {
    ENGINE_API bool InitializeEngine(int width, int height, const char* title) {
        try {
            // Initialize GLFW
            if (!glfwInit()) {
                return false;
            }
            
            // Create window
            g_window = std::make_unique<Window>();
            if (!g_window->Initialize(width, height, title)) {
                glfwTerminate();
                return false;
            }
            
            // Create Vulkan renderer
            g_renderer = std::make_unique<VulkanRenderer>();
            if (!g_renderer->Initialize(g_window.get())) {
                g_window->Shutdown();
                g_window.reset();
                glfwTerminate();
                return false;
            }
            
            return true;
        } catch (...) {
            return false;
        }
    }
    
    ENGINE_API void ShutdownEngine() {
        // Prevent double shutdown
        static bool alreadyShuttingDown = false;
        if (alreadyShuttingDown) {
            return;
        }
        alreadyShuttingDown = true;
        
        try {
            if (g_renderer) {
                g_renderer->Shutdown();
                g_renderer.reset();
            }
            
            if (g_window) {
                g_window->Shutdown();
                g_window.reset();
            }
            
            glfwTerminate();
        } catch (...) {
            // Swallow any exceptions during shutdown to prevent crashes
        }
        
        alreadyShuttingDown = false;
    }
    
    ENGINE_API bool ShouldClose() {
        try {
            return g_window ? g_window->ShouldClose() : true;
        } catch (...) {
            return true; // If there's an error, assume we should close
        }
    }
    
    ENGINE_API void PollEvents() {
        try {
            if (g_window) {
                g_window->PollEvents();
            }
        } catch (...) {
            // Ignore polling errors
        }
    }
    
    ENGINE_API void Render() {
        try {
            if (g_renderer) {
                g_renderer->Render();
            }
        } catch (...) {
            // Ignore render errors to prevent crashes
        }
    }
    
    ENGINE_API void SetWindowTitle(const char* title) {
        if (g_window && title) {
            g_window->SetTitle(title);
        }
    }
    
    ENGINE_API void GetWindowSize(int* width, int* height) {
        if (g_window && width && height) {
            g_window->GetSize(*width, *height);
        }
    }
    
    ENGINE_API void RenderSprite(
        const char* texturePath,
        float* worldMatrix,
        float* color,
        float* size,
        int sortingOrder,
        bool flipX,
        bool flipY
    ) {
        try {
            if (g_renderer && texturePath && worldMatrix && color && size) {
                g_renderer->RenderSprite(texturePath, worldMatrix, color, size, sortingOrder, flipX, flipY);
            }
        } catch (...) {
            // Swallow exceptions
        }
    }
    
    ENGINE_API void SetCameraMatrices(float* viewMatrix, float* projectionMatrix) {
        try {
            if (g_renderer && viewMatrix && projectionMatrix) {
                g_renderer->SetCameraMatrices(viewMatrix, projectionMatrix);
            }
        } catch (...) {
            // Swallow exceptions
        }
    }
    
    // Time management functions
    ENGINE_API void UpdateDeltaTime() {
        try {
            g_currentTime = glfwGetTime();
            if (g_lastTime == 0.0) {
                g_lastTime = g_currentTime;
                g_startTime = g_currentTime;
            }
            g_deltaTime = static_cast<float>(g_currentTime - g_lastTime);
            g_lastTime = g_currentTime;
        } catch (...) {
            // Fallback to default deltaTime if timing fails
            g_deltaTime = 1.0f / 60.0f; // Default to 60 FPS
        }
    }
    
    ENGINE_API float GetDeltaTime() {
        return g_deltaTime;
    }
    
    ENGINE_API double GetTime() {
        try {
            return glfwGetTime() - g_startTime;
        } catch (...) {
            return 0.0;
        }
    }
    
    ENGINE_API void ResetTime() {
        try {
            g_startTime = glfwGetTime();
            g_currentTime = g_startTime;
            g_lastTime = g_startTime;
            g_deltaTime = 0.0f;
        } catch (...) {
            // Ignore errors in reset
        }
    }
} 