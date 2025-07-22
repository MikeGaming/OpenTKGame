#version 440 core
layout (location = 0) in vec3 vertexPosition;

out vec2 UV0;

uniform mat4 model;
uniform mat4 lightSpaceMatrix;

void main()
{

    gl_Position = vec4(vertexPosition, 1.0) * model * lightSpaceMatrix;
}  