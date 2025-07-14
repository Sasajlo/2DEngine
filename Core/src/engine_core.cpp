#include "engine_core.h"
#include "window.h"
#include "vulkan_renderer.h"
#include <memory>
#include <unordered_map>
#include <unordered_set>
#include <cstdint> // For uint32_t

static std::unique_ptr<Window> g_window;
static std::unique_ptr<VulkanRenderer> g_renderer;

// Time management variables
static double g_currentTime = 0.0;
static double g_lastTime = 0.0;
static float g_deltaTime = 0.0f;
static double g_startTime = 0.0;

// Input state tracking
static std::unordered_set<int> g_keysPressed;     // Keys pressed this frame
static std::unordered_set<int> g_keysHeld;        // Keys currently held down
static std::unordered_set<int> g_keysReleased;    // Keys released this frame
static std::unordered_map<int, bool> g_keyStates; // Current state of all keys

// Mouse scroll tracking
static float g_mouseScrollDelta = 0.0f;

// GLFW scroll callback
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset) {
    g_mouseScrollDelta += static_cast<float>(yoffset);
}

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
            
            // Set up input callbacks
            glfwSetScrollCallback(g_window->GetHandle(), scroll_callback);
            
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
                // Clear frame-specific input states
                g_keysPressed.clear();
                g_keysReleased.clear();
                
                // Poll GLFW events
                g_window->PollEvents();
                
                // Update input states for common keys
                static const int commonKeys[] = {
                    GLFW_KEY_W, GLFW_KEY_A, GLFW_KEY_S, GLFW_KEY_D,
                    GLFW_KEY_Q, GLFW_KEY_E, GLFW_KEY_H, GLFW_KEY_SPACE, GLFW_KEY_LEFT_SHIFT,
                    GLFW_KEY_LEFT_CONTROL, GLFW_KEY_ESCAPE, GLFW_KEY_ENTER,
                    GLFW_KEY_UP, GLFW_KEY_DOWN, GLFW_KEY_LEFT, GLFW_KEY_RIGHT,
                    GLFW_KEY_F12
                };
                
                for (int keyCode : commonKeys) {
                    bool currentState = glfwGetKey(g_window->GetHandle(), keyCode) == GLFW_PRESS;
                    bool previousState = g_keyStates[keyCode];
                    
                    if (currentState && !previousState) {
                        // Key was just pressed
                        g_keysPressed.insert(keyCode);
                        g_keysHeld.insert(keyCode);
                    } else if (!currentState && previousState) {
                        // Key was just released
                        g_keysReleased.insert(keyCode);
                        g_keysHeld.erase(keyCode);
                    }
                    
                    g_keyStates[keyCode] = currentState;
                }
                
                // Note: Mouse scroll delta is NOT reset here as it needs to be 
                // read by the game code before being reset
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
    
    // Input handling functions
    ENGINE_API bool IsKeyPressed(int keyCode) {
        return g_keysPressed.count(keyCode) > 0;
    }
    
    ENGINE_API bool IsKeyHeld(int keyCode) {
        return g_keysHeld.count(keyCode) > 0;
    }
    
    ENGINE_API bool IsKeyReleased(int keyCode) {
        return g_keysReleased.count(keyCode) > 0;
    }
    
    ENGINE_API float GetMouseScrollDelta() {
        return g_mouseScrollDelta;
    }
    
    ENGINE_API void ResetMouseScrollDelta() {
        g_mouseScrollDelta = 0.0f;
    }
    
    ENGINE_API void GetMousePosition(float* x, float* y) {
        if (g_window && x && y) {
            double mouseX, mouseY;
            glfwGetCursorPos(g_window->GetHandle(), &mouseX, &mouseY);
            *x = static_cast<float>(mouseX);
            *y = static_cast<float>(mouseY);
        } else {
            if (x) *x = 0.0f;
            if (y) *y = 0.0f;
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
    
    ENGINE_API void RenderSpriteWithIndex(
        uint32_t textureIndex,
        float* worldMatrix,
        float* color,
        float* size,
        int sortingOrder,
        bool flipX,
        bool flipY
    ) {
        try {
            if (g_renderer && worldMatrix && color && size) {
                g_renderer->RenderSpriteWithIndex(textureIndex, worldMatrix, color, size, sortingOrder, flipX, flipY);
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
            // Ignore errors
        }
    }
    
    ENGINE_API uint32_t LoadTexture(const char* texturePath) {
        try {
            if (g_renderer && texturePath) {
                return g_renderer->LoadTexture(texturePath);
            }
            return UINT32_MAX;
        } catch (...) {
            return UINT32_MAX;
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
    
    ENGINE_API void RenderChunkMesh(
        float* vertices,
        uint32_t vertexCount,
        uint32_t* indices,
        uint32_t indexCount,
        uint32_t* textureIndices,
        float* colors,
        uint32_t quadCount,
        float tileSize
    ) {
        try {
            if (g_renderer && vertices && indices && textureIndices && colors) {
                g_renderer->RenderChunkMesh(vertices, vertexCount, indices, indexCount, textureIndices, colors, quadCount, tileSize);
            }
        } catch (...) {
            // Swallow exceptions
        }
    }
} 