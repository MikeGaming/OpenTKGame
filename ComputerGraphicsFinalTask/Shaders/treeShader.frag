#version 440 core

out vec4 fragColor;

uniform sampler2D modelTex;

in vec2 UV0;

void main()
{
    vec2 uv = UV0;
    //vec3 col = texture(modelTex, uv).xyz;
    fragColor = texture(modelTex, uv);
}