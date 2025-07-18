#version 440 core

out vec4 fragColor;

in vec2 UV0;

uniform sampler2D screenTexture;
uniform sampler2D depthTexture;
uniform vec2 resolution;


void main()
{
    float depthValue = texture(depthTexture, UV0).r;
    // Enhance depth contrast for visualization
    float visualizedDepth = pow(depthValue, 100); // Gamma correction for better contrast
    fragColor = vec4(vec3(visualizedDepth), 1.0);
}