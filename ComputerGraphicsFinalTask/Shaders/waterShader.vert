#version 330 core

// Input vertex attributes
layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexNormals;
layout(location = 2) in vec2 UVs;

// Output vertex attributes
out vec3 FragPos;
out vec2 UV0;
out vec3 Normals;

// Uniforms
uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform float time; // Time uniform to animate the waves


float Sine(vec2 direction, vec3 position, float amplitude, float frequency) {
    vec2 d = direction;
    float xz = position.x * d.x + position.z * d.y;
    float t = time;

    return amplitude * sin(xz * frequency + t);
}

float SteepSine(vec2 direction, vec3 position, float amplitude, float frequency, float steepness) {
    vec2 d = direction;
    float xz = position.x * d.x + position.z * d.y;
    float t = time;

    return 2.0 * amplitude * pow((sin(xz * frequency + t) + 1.0) / 2.0, steepness);
}

vec3 Gerstner(vec2 direction, vec3 position, float amplitude, float frequency, float steepness) {
    vec2 d = direction;
    float xz = position.x * d.x + position.z * d.y;
    float t = time;

    vec3 g = vec3(0.0, 0.0, 0.0);
    g.x = steepness * amplitude * d.x * cos(frequency * xz + t);
    g.z = steepness * amplitude * d.y * cos(frequency * xz + t);
    g.y = amplitude * sin(frequency * xz + t);

    return g;
}


void main()
{
    vec3 position = vertexPosition;

    float wave1 = sin(position.x * 5.0 + time * 2.0) * 5;   // Increased amplitude from 0.05 to 0.1
    float wave2 = sin(position.y * 5.0 + time * 1.5) * 2;   // Increased amplitude from 0.1 to 0.2
    float wave3 = sin((position.x + position.y) * 10.0 + time * 1.0) * .6;  // Increased amplitude from 0.03 to 0.06

    position.z = wave1 + wave2 + wave3;
    // Calculate final vertex position
    
    vec4 pos = vec4(position, 1.0) * model;
    
    vec4 finalPosition = pos * view * projection;

    //FragPos = position; // Pass the position to the fragment shader
    gl_Position = finalPosition; // Set final position
    
    UV0 = UVs;

    Normals = vertexNormals;
    FragPos = vec3(pos);
}
