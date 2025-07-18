#version 440 core

out vec4 fragColor;

in vec2 UV0;

uniform sampler2D screenTexture;
uniform sampler2D depthTexture;
uniform vec2 resolution;
uniform vec4 fogColor;

vec4 screenColor;
vec4 depthColor;

void main()
{
    screenColor = texture(screenTexture, UV0);
    float depthValue = pow(texture(depthTexture, UV0).r, 100);
    fragColor = screenColor * (1 - depthValue) + fogColor * depthValue;
    fragColor.a = screenColor.a; // Preserve alpha from the screen texture
}