#pragma once

#include <GLFW/glfw3.h>
#include <string>

class Window {
public:
    Window();
    ~Window();
    
    bool Initialize(int width, int height, const std::string& title);
    void Shutdown();
    
    bool ShouldClose() const;
    void PollEvents();
    void SwapBuffers();
    
    void SetTitle(const std::string& title);
    void GetSize(int& width, int& height) const;
    void SetSize(int width, int height);
    
    GLFWwindow* GetHandle() const { return m_window; }
    
private:
    GLFWwindow* m_window;
    std::string m_title;
    int m_width, m_height;
}; 