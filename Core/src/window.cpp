#include "window.h"
#include <iostream>

Window::Window() : m_window(nullptr), m_width(0), m_height(0) {
}

Window::~Window() {
    Shutdown();
}

bool Window::Initialize(int width, int height, const std::string& title) {
    m_width = width;
    m_height = height;
    m_title = title;
    
    // Tell GLFW not to create an OpenGL context
    glfwWindowHint(GLFW_CLIENT_API, GLFW_NO_API);
    // TODO: Enable resizing once swapchain recreation is implemented
    glfwWindowHint(GLFW_RESIZABLE, GLFW_FALSE);
    // Start maximized
    glfwWindowHint(GLFW_MAXIMIZED, GLFW_TRUE);
    
    m_window = glfwCreateWindow(width, height, title.c_str(), nullptr, nullptr);
    if (!m_window) {
        std::cerr << "Failed to create GLFW window" << std::endl;
        return false;
    }
    
    // Get actual window size after maximization
    glfwGetFramebufferSize(m_window, &m_width, &m_height);
    
    // Set window user pointer to this instance for callbacks
    glfwSetWindowUserPointer(m_window, this);
    
    // Set resize callback to update window size when display configuration changes
    glfwSetFramebufferSizeCallback(m_window, [](GLFWwindow* window, int width, int height) {
        Window* windowInstance = static_cast<Window*>(glfwGetWindowUserPointer(window));
        if (windowInstance) {
            windowInstance->SetSize(width, height);
        }
    });
    
    return true;
}

void Window::Shutdown() {
    if (m_window) {
        glfwDestroyWindow(m_window);
        m_window = nullptr;
    }
}

bool Window::ShouldClose() const {
    return m_window ? glfwWindowShouldClose(m_window) : true;
}

void Window::PollEvents() {
    glfwPollEvents();
}

void Window::SwapBuffers() {
    // Not needed for Vulkan, but kept for interface consistency
}

void Window::SetTitle(const std::string& title) {
    m_title = title;
    if (m_window) {
        glfwSetWindowTitle(m_window, title.c_str());
    }
}

void Window::GetSize(int& width, int& height) const {
    width = m_width;
    height = m_height;
}

void Window::SetSize(int width, int height) {
    m_width = width;
    m_height = height;
} 