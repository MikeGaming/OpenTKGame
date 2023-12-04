#version 440 core

out vec4 fragColor;

in vec2 UV0;

uniform sampler2D screenTexture;
uniform vec3 tintAmounts;

void main()
{
    //Make a color tint shader
    vec4 color = texture(screenTexture, UV0);
    color.r *= tintAmounts.r;
    color.g *= tintAmounts.g;
    color.b *= tintAmounts.b;
    fragColor = color;
}