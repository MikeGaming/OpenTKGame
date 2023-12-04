#version 440 core

out vec4 fragColor;

in vec2 UV0;

uniform sampler2D screenTexture;
uniform float vignetteStrength; // Strength of the vignette effect
uniform float falloff = .4; // Falloff of the vignette effect

void main() {

    vec4 color = texture2D(screenTexture, UV0);

    float dist = distance(UV0, vec2(0.5, 0.5));
    color.rgb *= smoothstep(0.8, falloff * 0.799, dist * (vignetteStrength + falloff));

    fragColor = color;

}