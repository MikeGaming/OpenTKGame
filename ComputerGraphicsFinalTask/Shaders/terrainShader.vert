#version 440 core

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexNormals;
layout(location = 2) in vec2 UVs;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out float vHeight;
out vec3 FragPos;
out vec2 UV0;
out vec3 Normals;

float random (in vec2 st) {
    return fract(sin(dot(st.xy,
    vec2(12.9898,78.233)))*
    43758.5453123);
}

float noise (in vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);

    // Four corners in 2D of a tile
    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(a, b, u.x) +
    (c - a)* u.y * (1.0 - u.x) +
    (d - b) * u.x * u.y;
}

#define OCTAVES 1
float fbm (in vec2 st) {
    // Initial values
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 0.;
    //
    // Loop of octaves
    for (int i = 0; i < OCTAVES; i++) {
        value += amplitude * noise(st);
        st *= 2.;
        amplitude *= .7;
    }
    return value;
}

void main()
{
    
    
    vec2 st = vertexPosition.xy/UVs.xy;
    st.x *= UVs.x/UVs.y;
    
    vHeight = 0;
    vHeight += fbm(st * 3);
    
    vec3 newPosition = vec3(vertexPosition.x, vertexPosition.y, vHeight * 10);
    
    vec4 pos = vec4(newPosition, 1.0) * model;
    
    gl_Position = pos * view * projection;
    UV0 = UVs * 10;
    
    Normals = vertexNormals  * mat3(transpose(inverse(model)));
    FragPos = vec3(pos);
}