#version 440 core

out vec4 fragColor;

in vec2 UV0;

uniform sampler2D modelTex;
uniform float time;

const vec2 dimensions = vec2(5, 5);
const float speed = 24;

void main()
{
    
    const float totalFrames = dimensions.x * dimensions.y;
    float currentFrameIndex = floor(mod(time * speed + dimensions.x, totalFrames));
    
    vec2 uv = UV0;
    
    uv.x = (uv.x + mod(currentFrameIndex, dimensions.x)) / dimensions.x;
    uv.y = (uv.y - floor(currentFrameIndex / dimensions.x)) / dimensions.y;
    
    vec4 textureColor = texture(modelTex, uv);
    
    fragColor = textureColor;
    
}