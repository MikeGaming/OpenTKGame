#version 440 core

out vec4 fragColor;

in vec3 UV0;

uniform samplerCube skybox;

void main()
{
    
    fragColor = texture(skybox, UV0);
}