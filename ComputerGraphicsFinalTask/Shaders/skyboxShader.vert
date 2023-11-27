#version 440 core

layout (location = 0) in vec3 vertexPosition;

out vec3 UV0;

uniform mat4 projection;
uniform mat4 view;

void main()
{

    UV0 = vertexPosition;
    vec4 pos = vec4(vertexPosition, 1.0) * view * projection;
    gl_Position = pos.xyww;
}