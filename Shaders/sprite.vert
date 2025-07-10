#version 450

layout(location = 0) in vec2 inPosition;
layout(location = 1) in vec2 inTexCoord;

layout(location = 0) out vec2 fragTexCoord;
layout(location = 1) out vec4 fragColor;

layout(binding = 0) uniform UniformBufferObject {
    mat4 view;
    mat4 projection;
} ubo;

layout(push_constant) uniform PushConstants {
    mat4 model;
    vec4 color;
    vec2 size;
    uint textureIndex;
    float padding;
} pc;

void main() {
    // Apply size scaling to the vertex position
    vec2 scaledPos = inPosition * pc.size;
    
    // Transform vertex position: projection * view * model * vertex
    vec4 worldPos = pc.model * vec4(scaledPos, 0.0, 1.0);
    gl_Position = ubo.projection * ubo.view * worldPos;
    
    fragTexCoord = inTexCoord;
    fragColor = pc.color;
} 