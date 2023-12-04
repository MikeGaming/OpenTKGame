#version 440 core

layout (location = 1) in vec3 vertexPosition;
layout (location = 2) in vec2 UVs;

out vec2 UV0;

uniform vec2 screenSize;

uniform mat4 view;

void main()
{
    
    float nx = (vertexPosition.x / screenSize.x) * 2.0 - 1.0;
    float ny = (vertexPosition.y / screenSize.y) * 2.0 - 1.0;

    vec4 pos = vec4(vertexPosition, 1.0);

    gl_Position = pos;
    UV0 = UVs;
}