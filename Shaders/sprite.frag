#version 450

layout(location = 0) in vec2 fragTexCoord;
layout(location = 1) in vec4 fragColor;

layout(location = 0) out vec4 outColor;

layout(binding = 1) uniform sampler2D texSamplers[1000]; // Array of textures

layout(push_constant) uniform PushConstants {
    mat4 worldMatrix;
    vec4 color;
    vec2 size;
    uint textureIndex;
    float padding;
} pushConstants;

void main() {
    // Sample the texture using the texture index and multiply by the color
    outColor = texture(texSamplers[pushConstants.textureIndex], fragTexCoord) * fragColor;
} 