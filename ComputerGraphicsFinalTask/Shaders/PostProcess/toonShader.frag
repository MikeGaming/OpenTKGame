#version 440 core

in vec2 UV0;
out vec4 fragColor;

uniform sampler2D screenTexture;
uniform vec2 resolution; // Screen resolution

void main()
{
    vec2 texelSize = 1.0 / resolution;
    vec3 sceneColor = texture(screenTexture, UV0).rgb;

    // Calculate the intensity of the pixel's grayscale value
    float intensity = dot(sceneColor.rgb, vec3(0.2126, 0.7152, 0.0722));

    // Apply quantization to create the toon effect
    float threshold = 0.2;
    float edge = 0.4;

    vec3 toonColor;
    if (intensity < threshold)
    toonColor = sceneColor * 0.5;
    else if (intensity < edge)
    toonColor = vec3(sceneColor * 0.75);
    else
    toonColor = vec3(sceneColor);

    fragColor = vec4(toonColor * 1.25, 1.0);
}