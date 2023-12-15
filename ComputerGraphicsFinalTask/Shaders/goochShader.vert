#version 440 core

layout (location = 0) in vec3 vertexPosition;
layout (location = 1) in vec3 vertexNormals;
layout (location = 2) in vec2 UVs;

out vec2 UV0;

out vec3 FragPos;
out vec3 Normals;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{

    vec4 pos = vec4(vertexPosition, 1.0) * model;

    gl_Position = pos * view * projection;
    UV0 = UVs;

    Normals = vertexNormals * mat3(transpose(inverse(model)));
    FragPos = vec3(pos);
}