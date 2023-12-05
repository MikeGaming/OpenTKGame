#version 440 core

out vec4 fragColor;

in vec2 UV0;

uniform sampler2D screenTexture;
uniform vec2 resolution;

vec2 Coord()
{
vec2 Pixels = resolution * 1.3;
float dx = 15.0 * (1.0 / Pixels.x);
float dy = 10.0 * (1.0 / Pixels.y);
return vec2(dx * floor(UV0.x / dx),
dy * floor(UV0.y / dy));
}

void main()
{
    fragColor = texture(screenTexture, Coord());
}