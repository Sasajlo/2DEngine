#include "engine_core.h"
#include "window.h"
#include "vulkan_renderer.h"
#include <memory>

static std::unique_ptr<Window> g_window;
static std::unique_ptr<VulkanRenderer> g_renderer;

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
} 