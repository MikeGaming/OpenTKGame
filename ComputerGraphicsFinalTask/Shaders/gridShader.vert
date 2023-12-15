#version 440 core

layout (location = 0) in vec3 vertexPosition;

out vec3 FragPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{

    vec3 FragPos = vec3((vertexPosition.x+3.0), vertexPosition.y, (vertexPosition.z + 3.0));

    gl_Position = vec4(pos, 1.0) * model * view * projection;
}