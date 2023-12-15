#version 440 core

out vec4 fragColor;

in vec2 UV0;

uniform sampler2D screenTexture;
uniform sampler2D depthTexture;

void main()
{
    
    //Write depth texture to screen
    float depth = texture(depthTexture, UV0).r;
    fragColor = vec4(depth, depth, depth, 1.0);
}