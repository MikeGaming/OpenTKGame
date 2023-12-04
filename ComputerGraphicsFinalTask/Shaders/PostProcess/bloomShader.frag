#version 440 core

out vec4 fragColor;

in vec2 UV0;

uniform sampler2D screenTexture;

// Additional uniforms for blur
uniform vec2 resolution;
float screenWidth;
float screenHeight;
const float blurSize = 10.0; // Adjust blur size

void main()
{
    
    screenWidth = resolution.x;
    screenHeight = resolution.y;
    
    vec3 sceneColor = texture(screenTexture, UV0).rgb;

    // Extract high-intensity areas using a threshold
    float threshold = 0.2; // Adjust threshold as needed
    vec3 bloomThreshold = max(sceneColor - threshold, 0.0);

    // Blur the extracted bloom areas
    vec2 texelSize = 1.0 / vec2(screenWidth, screenHeight);
    vec3 blur = vec3(0.0);

    for (float x = -blurSize; x <= blurSize; x++) {
        for (float y = -blurSize; y <= blurSize; y++) {
            vec2 offset = vec2(x, y) * texelSize;
            blur += texture(screenTexture, UV0 + offset).rgb;
        }
    }

    blur /= pow((2.0 * blurSize + 1.0), 2.0);

    // Combine the bloom effect with the original scene
    vec3 finalColor = sceneColor + bloomThreshold * blur;

    fragColor = vec4(finalColor, 1.0);
}