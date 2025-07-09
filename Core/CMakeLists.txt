cmake_minimum_required(VERSION 3.20)
project(EngineCore)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Set output directory to local build folder
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

# Find Vulkan
set(VULKAN_SDK_PATH "${CMAKE_CURRENT_SOURCE_DIR}/../Extern/VulkanSDK/1.4.313.1")
set(Vulkan_INCLUDE_DIRS "${VULKAN_SDK_PATH}/Include")
set(Vulkan_LIBRARIES "${VULKAN_SDK_PATH}/Lib/vulkan-1.lib")

# Find GLFW
set(GLFW_PATH "${CMAKE_CURRENT_SOURCE_DIR}/../Extern/glfw-3.4.bin.WIN64")
set(GLFW_INCLUDE_DIRS "${GLFW_PATH}/include")
set(GLFW_LIBRARIES "${GLFW_PATH}/lib-vc2022/glfw3.lib")

# Include directories
include_directories(${Vulkan_INCLUDE_DIRS})
include_directories(${GLFW_INCLUDE_DIRS})

# Source files
set(SOURCES
    src/engine_core.cpp
    src/vulkan_renderer.cpp
    src/window.cpp
)

set(HEADERS
    include/engine_core.h
    include/vulkan_renderer.h
    include/window.h
)

# Create the DLL
add_library(EngineCore SHARED ${SOURCES} ${HEADERS})

# Link libraries
target_link_libraries(EngineCore ${Vulkan_LIBRARIES} ${GLFW_LIBRARIES})

# Include directories for the target
target_include_directories(EngineCore PRIVATE include)

# Export symbols on Windows
if(WIN32)
    target_compile_definitions(EngineCore PRIVATE ENGINE_CORE_EXPORTS)
endif()

# Copy GLFW DLL to local build output directory
add_custom_command(TARGET EngineCore POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
    "${GLFW_PATH}/lib-vc2022/glfw3.dll"
    $<TARGET_FILE_DIR:EngineCore>) 