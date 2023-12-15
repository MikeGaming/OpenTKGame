#version 440 core

out vec4 fragColor;

in vec2 UV0;

uniform sampler2D screenTexture;
uniform float contrast;
uniform float brightness;
uniform float saturation;
uniform float gamma;

void main()
{
    vec4 color = texture(screenTexture, UV0); // Current color of pixel
    
    color.rgb = pow(color.rgb, vec3(1.0/2.2)); // Convert from gamma (sRGB) to linear (RGB)
    
    color = contrast * (color - 0.5) + 0.5 + brightness; // Apply contrast and brightness
    color = max(vec4(0.0), color); //Clamp between 0
    color = min(vec4(1.0), color); //and 1
    
    vec4 desaturated = vec4(dot(color.rgb, vec3(0.299, 0.587, 0.114))); // Convert to grayscale
    color = mix(desaturated, color, saturation); // Apply custom saturation to grayscale
    color = max(vec4(0.0), color); //Clamp between 0
    color = min(vec4(1.0), color); //and 1
    
    color = pow(color, vec4(gamma)); // Convert back from linear (RGB) to gamma (sRGB) (aka. gamma correction)
    color = max(vec4(0.0), color); //Clamp between 0
    color = min(vec4(1.0), color); //and 1
    
    fragColor = color; //Set color of pixel to corrected color
}