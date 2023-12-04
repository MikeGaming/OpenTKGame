#version 440 core

in vec2 UV0;
out vec4 fragColor;

uniform sampler2D screenTexture;
uniform vec2 resolution; // Screen resolution


void main() {
    float redOffset   =  0.009;
    float greenOffset =  0.006;
    float blueOffset  = -0.006;

    vec2 texSize  = textureSize(screenTexture, 0).xy;
    vec2 texCoord = UV0.xy;

    vec2 direction = texCoord;

    fragColor = texture(screenTexture, texCoord);

    fragColor.r = texture(screenTexture, texCoord + (direction * vec2(redOffset  ))).r;
    fragColor.g = texture(screenTexture, texCoord + (direction * vec2(greenOffset))).g;
    fragColor.b = texture(screenTexture, texCoord + (direction * vec2(blueOffset ))).b;
}