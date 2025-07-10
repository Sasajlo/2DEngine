#pragma once

#define GLFW_INCLUDE_VULKAN
#include <GLFW/glfw3.h>
#include <vulkan/vulkan.h>
#include <vector>
#include <optional>
#include <string>
#include <unordered_map>

class Window;

struct QueueFamilyIndices {
    std::optional<uint32_t> graphicsFamily;
    std::optional<uint32_t> presentFamily;
    
    bool isComplete() {
        return graphicsFamily.has_value() && presentFamily.has_value();
    }
};

struct SwapChainSupportDetails {
    VkSurfaceCapabilitiesKHR capabilities;
    std::vector<VkSurfaceFormatKHR> formats;
    std::vector<VkPresentModeKHR> presentModes;
};

class VulkanRenderer {
public:
    VulkanRenderer();
    ~VulkanRenderer();
    
    bool Initialize(Window* window);
    void Shutdown();
    void Render();
    
    // Sprite rendering methods
    void RenderSprite(
        const char* texturePath,
        float* worldMatrix,
        float* color,
        float* size,
        int sortingOrder,
        bool flipX,
        bool flipY
    );
    void SetCameraMatrices(float* viewMatrix, float* projectionMatrix);
    
private:
    Window* m_window;
    
    VkInstance m_instance;
    VkPhysicalDevice m_physicalDevice;
    VkDevice m_device;
    VkQueue m_graphicsQueue;
    VkQueue m_presentQueue;
    VkSurfaceKHR m_surface;
    
    VkSwapchainKHR m_swapChain;
    std::vector<VkImage> m_swapChainImages;
    VkFormat m_swapChainImageFormat;
    VkExtent2D m_swapChainExtent;
    std::vector<VkImageView> m_swapChainImageViews;
    
    VkRenderPass m_renderPass;
    VkPipelineLayout m_pipelineLayout;
    VkPipeline m_graphicsPipeline;
    std::vector<VkFramebuffer> m_swapChainFramebuffers;
    
    VkCommandPool m_commandPool;
    std::vector<VkCommandBuffer> m_commandBuffers;
    
    std::vector<VkSemaphore> m_imageAvailableSemaphores;
    std::vector<VkSemaphore> m_renderFinishedSemaphores;
    std::vector<VkFence> m_inFlightFences;
    
    size_t m_currentFrame;
    static const int MAX_FRAMES_IN_FLIGHT = 2;
    static const uint32_t MAX_TEXTURES = 1000;
    
    // Sprite rendering data
    struct SpriteVertex {
        float pos[2];
        float texCoord[2];
    };
    
    struct SpriteData {
        std::string texturePath;
        uint32_t textureIndex;
        float worldMatrix[16];
        float color[4];
        float size[2];
        int sortingOrder;
        bool flipX;
        bool flipY;
    };
    
    std::vector<SpriteData> m_spriteQueue;
    
    // Camera matrices
    float m_viewMatrix[16];
    float m_projectionMatrix[16];
    
    // Texture management with descriptor indexing
    std::unordered_map<std::string, VkImage> m_textures;
    std::unordered_map<std::string, VkImageView> m_textureViews;
    std::unordered_map<std::string, VkDeviceMemory> m_textureMemory;
    std::unordered_map<std::string, uint32_t> m_textureIndices;
    std::vector<VkImageView> m_textureArray;
    VkSampler m_textureSampler;
    uint32_t m_nextTextureIndex;
    
    // Sprite rendering pipeline
    VkPipeline m_spritePipeline;
    VkPipelineLayout m_spritePipelineLayout;
    VkDescriptorSetLayout m_spriteDescriptorSetLayout;
    VkDescriptorPool m_spriteDescriptorPool;
    std::vector<VkDescriptorSet> m_spriteDescriptorSets;
    
    // Uniform buffers
    VkBuffer m_uniformBuffer;
    VkDeviceMemory m_uniformBufferMemory;
    void* m_uniformBufferMapped;
    
    // Vertex buffer for sprites
    VkBuffer m_spriteVertexBuffer;
    VkDeviceMemory m_spriteVertexBufferMemory;
    VkBuffer m_spriteIndexBuffer;
    VkDeviceMemory m_spriteIndexBufferMemory;
    
    // Helper functions
    bool CreateInstance();
    bool CreateSurface();
    bool PickPhysicalDevice();
    bool CreateLogicalDevice();
    bool CreateSwapChain();
    bool CreateImageViews();
    bool CreateRenderPass();
    bool CreateGraphicsPipeline();
    bool CreateFramebuffers();
    bool CreateCommandPool();
    bool CreateCommandBuffers();
    bool CreateSyncObjects();
    
    void RecordCommandBuffer(VkCommandBuffer commandBuffer, uint32_t imageIndex);
    
    QueueFamilyIndices FindQueueFamilies(VkPhysicalDevice device);
    SwapChainSupportDetails QuerySwapChainSupport(VkPhysicalDevice device);
    VkSurfaceFormatKHR ChooseSwapSurfaceFormat(const std::vector<VkSurfaceFormatKHR>& availableFormats);
    VkPresentModeKHR ChooseSwapPresentMode(const std::vector<VkPresentModeKHR>& availablePresentModes);
    VkExtent2D ChooseSwapExtent(const VkSurfaceCapabilitiesKHR& capabilities);
    
    std::vector<char> ReadFile(const std::string& filename);
    VkShaderModule CreateShaderModule(const std::vector<char>& code);
    
    // Texture loading functions
    uint32_t LoadTexture(const std::string& texturePath);
    bool CreateTextureImage(const std::string& texturePath, VkImage& textureImage, VkDeviceMemory& textureImageMemory);
    bool CreateTextureImageView(VkImage textureImage, VkImageView& textureImageView);
    bool CreateTextureSampler();
    
    // Sprite rendering functions
    bool CreateSpriteRenderingPipeline();
    bool CreateSpriteVertexBuffer();
    bool CreateDescriptorSetLayout();
    bool CreateUniformBuffer();
    bool CreateDescriptorSets();
    void UpdateUniformBuffer(uint32_t currentImage);
    void RenderSpriteQueue(VkCommandBuffer commandBuffer, uint32_t imageIndex);
    void UpdateTextureDescriptorSet();
    
    // Helper methods for buffer operations
    uint32_t FindMemoryType(uint32_t typeFilter, VkMemoryPropertyFlags properties);
    void CopyBuffer(VkBuffer srcBuffer, VkBuffer dstBuffer, VkDeviceSize size);
    
    // Helper methods for image operations
    void TransitionImageLayout(VkImage image, VkFormat format, VkImageLayout oldLayout, VkImageLayout newLayout);
    void CopyBufferToImage(VkBuffer buffer, VkImage image, uint32_t width, uint32_t height);
    VkCommandBuffer BeginSingleTimeCommands();
    void EndSingleTimeCommands(VkCommandBuffer commandBuffer);
}; 