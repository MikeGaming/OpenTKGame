#version 440 core

out vec4 fragColor;

in vec2 UV0;

uniform float time;
uniform sampler2D screenTexture;
uniform float grainAmount;

vec2 pi = vec2(3.1415926535897932384626433832795, 3.1415926535897932384626433832795);

void main() {
    float amount = grainAmount;

    vec2 texSize  = textureSize(screenTexture, 0).xy;
    vec2 texCoord = UV0.xy / texSize;

    vec4 color = texture(screenTexture, UV0);
    float randomIntensity =
    fract
    ( 10000
    * sin
    (
    ( UV0.x
    + UV0.y
    * time
    )
    * pi.y
    )
    );

    amount *= randomIntensity;

    color.rgb += amount;

    fragColor = color;
}