#version 440 core

layout (location = 0) in vec3 vertexPosition;
layout (location = 2) in vec2 UVs;

//out vec3 FragPos;
//out vec3 Normals;

out vec2 UV0;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{

    UV0 = UVs;

    vec3 billboardNormal = vec3(0.0, 1.0, 0.0); // Billboard normal in its local space
    vec3 billboardRight = vec3(1.0, 0.0, 0.0); // Billboard right vector in its local space

    // Combine billboard normal and right vectors with the view matrix
    vec3 rotatedNormal = mat3(view) * billboardNormal;
    vec3 rotatedRight = mat3(view) * billboardRight;

    // Calculate final position (assuming the billboard is centered at 'position')
    vec3 finalPosition = vertexPosition.x * rotatedRight + vertexPosition.y * rotatedNormal + vertexPosition.z * gl_Position.xyz;

    vec4 worldPosition = vec4(finalPosition, 1.0) * model;
    
    gl_Position = worldPosition * view * projection;
    
}