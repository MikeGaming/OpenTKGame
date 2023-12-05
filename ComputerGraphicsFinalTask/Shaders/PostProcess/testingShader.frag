#version 440 core

out vec4 fragColor;


in vec2 UV0;

uniform sampler2D screenTexture;
uniform vec2 resolution;

vec2 Coord()
{
    vec2 Pixels = resolution * 2;
    float dx = 15.0 * (1.0 / Pixels.x);
    float dy = 10.0 * (1.0 / Pixels.y);
    return vec2(dx * floor(UV0.x / dx),
    dy * floor(UV0.y / dy));
}

int radius = 10;

precision highp float;

const vec2 src_size = vec2 (resolution.x, resolution.y);

void main ()
{
    vec2 uv = Coord();
    float n = float((radius + 1) * (radius + 1));

    vec3 m[4];
    vec3 s[4];
    for (int k = 0; k < 4; ++k) {
        m[k] = vec3(0.0);
        s[k] = vec3(0.0);
    }

    for (int j = -radius; j <= 0; ++j)  {
        for (int i = -radius; i <= 0; ++i)  {
            vec3 c = texture2D(screenTexture, uv + vec2(i,j) / src_size).rgb;
            m[0] += c;
            s[0] += c * c;
        }
    }

    for (int j = -radius; j <= 0; ++j)  {
        for (int i = 0; i <= radius; ++i)  {
            vec3 c = texture2D(screenTexture, uv + vec2(i,j) / src_size).rgb;
            m[1] += c;
            s[1] += c * c;
        }
    }

    for (int j = 0; j <= radius; ++j)  {
        for (int i = 0; i <= radius; ++i)  {
            vec3 c = texture2D(screenTexture, uv + vec2(i,j) / src_size).rgb;
            m[2] += c;
            s[2] += c * c;
        }
    }

    for (int j = 0; j <= radius; ++j)  {
        for (int i = -radius; i <= 0; ++i)  {
            vec3 c = texture2D(screenTexture, uv + vec2(i,j) / src_size).rgb;
            m[3] += c;
            s[3] += c * c;
        }
    }


    float min_sigma2 = 1e+2;
    for (int k = 0; k < 4; ++k) {
        m[k] /= n;
        s[k] = abs(s[k] / n - m[k] * m[k]);

        float sigma2 = s[k].r + s[k].g + s[k].b;
        if (sigma2 < min_sigma2) {
            min_sigma2 = sigma2;
            fragColor = vec4(m[k], 1.0);
        }
    }
}