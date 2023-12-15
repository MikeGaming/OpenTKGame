#version 440 core

out vec4 fragColor;

in vec3 FragPos;

const float squareSize = 0.1;
const vec3 color_l1 = vec3(1.0, 0.0, 0.0);

void main()
{
    // calculate deriviatives
    // (must be done at the start before conditionals)
    float dXy = abs(dFdx(FragPos.z)) / 2.0;
    float dYy = abs(dFdy(FragPos.z)) / 2.0;
    float dXx = abs(dFdx(FragPos.x)) / 2.0;
    float dYx = abs(dFdy(FragPos.x)) / 2.0;

    // find and fill horizontal lines
    int roundPos = int(FragPos.z / squareSize);
    float remainder = FragPos.z - float(roundPos)*squareSize;
    float width = max(dYy, dXy) * 2.0;

    if (remainder <= width)
    {
        float diff = (width - remainder) / width;
        fragColor = vec4(color_l1, diff);
        return;
    }

    if (remainder >= (squareSize - width))
    {
        float diff = (remainder - squareSize + width) / width;
        fragColor = vec4(color_l1, diff);
        return;
    }

    // find and fill vertical lines
    roundPos = int(FragPos.x / squareSize);
    remainder = FragPos.x - float(roundPos)*squareSize;
    width = max(dYx, dXx) * 2.0;

    if (remainder <= width)
    {
        float diff = (width - remainder) / width;
        fragColor = vec4(color_l1, diff);
        return;
    }

    if (remainder >= (squareSize - width))
    {
        float diff = (remainder - squareSize + width) / width;
        fragColor = vec4(color_l1, diff);
        return;
    }

    // fill base color
    fragColor = vec4(0,0,0, 0);
    return;
}