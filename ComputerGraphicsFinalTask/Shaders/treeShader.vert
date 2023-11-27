#version 440 core

layout (location = 0) in vec3 vertexPosition;
layout (location = 2) in vec2 UVs;

out vec2 UV0;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    UV0 = UVs;

    vec3 billboardNormal = vec3(0.0, -1.0, 0.0); // Billboard normal in its local space
    vec3 billboardRight = vec3(1.0, 0.0, 0.0); // Billboard right vector in its local space

    // Calculate the camera's forward vector in world space (assuming view matrix inverse)
    vec3 camForward = normalize((view * vec4(0.0, 0.0, -1.0, 0.0)).xyz);

    // Project camera forward vector onto the horizontal plane (set Y component to 0)
    vec3 camOnPlane = normalize(vec3(camForward.x, 0.0, camForward.z));

    // Calculate the right vector (horizontal) based on the projected camera forward vector
    billboardRight = normalize(cross(billboardNormal, camOnPlane));

    // Calculate the final normal vector (up) based on the billboard right vector and normal
    billboardNormal = cross(billboardRight, camOnPlane);

    // Calculate final position (assuming the billboard is centered at 'position')
    vec3 finalPosition = vertexPosition.x * billboardRight + vertexPosition.y * billboardNormal + vertexPosition.z * gl_Position.xyz;

    vec4 worldPosition = vec4(finalPosition, 1.0) * model;

    gl_Position = worldPosition * view * projection;
    
/*
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
    */
}