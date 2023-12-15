#version 440 core

out vec4 fragColor;

in vec2 UV0;
uniform sampler2D screenTexture;
uniform vec2 resolution;
uniform bool bloomEnabled, exposureEnabled, whiteBalanceEnabled, colorCorrectionEnabled;

//BLOOM
    vec3 bloom(vec3 currentSceneColor, sampler2D tex, vec2 uv, vec2 res, float threshold, float blurSize)
    {
        float screenWidth = res.x;
        float screenHeight = res.y;
    
        vec3 color = currentSceneColor;
    
        // Extract high-intensity areas using a threshold
        vec3 bloomThreshold = max(color - threshold, 0.0);
    
        // Blur the extracted bloom areas
        vec2 texelSize = 1.0 / vec2(screenWidth, screenHeight);
        vec3 blur = vec3(0.0);
    
        for (float x = -blurSize; x <= blurSize; x++) {
            for (float y = -blurSize; y <= blurSize; y++) {
                vec2 offset = vec2(x, y) * texelSize;
                blur += texture(tex, uv + offset).rgb;
            }
        }
    
        blur /= pow((2.0 * blurSize + 1.0), 2.0);
    
        // Combine the bloom effect with the original scene
        vec3 bloomColor = color + bloomThreshold * blur;
        return bloomColor;
    }
//END OF BLOOM

//EXPOSURE
    vec3 exposure(vec3 currentSceneColor, sampler2D tex, vec2 uv, float exposureAmount)
    {
        vec3 color = currentSceneColor; // Current color of pixel
        color = color * exposureAmount; // Multiply color by exposure amount
        return color;
    }
//END OF EXPOSURE

//WHITE BALANCE
    vec3 whiteBalance(vec3 currentSceneColor, sampler2D tex, vec2 uv, float temperature, float tint)
    {
        float t1 = temperature * 10.0 / 6.0;
        float t2 = tint * 10.0 / 6.0;
        
        float x = 0.31271 - t1 * (t1 < 0.0 ? 0.1 : 0.05);
        float standardIlluminantY = 2.87 * x - 3.0 * x * x - 0.27509507;
        float y = standardIlluminantY + t2 * 0.05;
        
        vec3 w1 = vec3(0.949237, 1.03542, 1.08728);
        
        float Y = 1;
        float X = Y * x / y;
        float Z = Y * (1 - x - y) / y;
        float L = 0.7328 * X + 0.4296 * Y - 0.1624 * Z;
        float M = -0.7036 * X + 1.6975 * Y + 0.0061 * Z;
        float S = 0.0030 * X + 0.0136 * Y + 0.9834 * Z;
        vec3 w2 = vec3(L, M, S);
        
        vec3 balance = vec3(w1.x / w2.x, w1.y / w2.y, w1.z / w2.z);
        
        mat3 LIN_2_LMS_MAT =
        {
        vec3(0.3904, 0.5499, 0.0892),
        vec3(0.0708, 0.9631, 0.0013),
        vec3(0.0231, 0.1280, 0.9362)
        };

        mat3 LMS_2_LIN_MAT =
        {
        vec3(2.8584, -1.6287, -0.0248),
        vec3(-0.2101, 1.1582, 0.0003),
        vec3(-0.0418, 0.1280, 1.0686)
        };
        
        vec3 lms = currentSceneColor * LIN_2_LMS_MAT;
        lms *= balance;
        vec3 a = lms * LMS_2_LIN_MAT;
        return a;
    }

//COLOR CORRECTION
    vec3 colorCorrection(vec3 currentSceneColor, sampler2D tex, vec2 uv, float contrast, float brightness, float saturation, float gamma)
    {
        vec3 color = currentSceneColor; // Current color of pixel
    
        color = pow(color, vec3(1.0/2.2)); // Convert from gamma (sRGB) to linear (RGB)
    
        color = contrast * (color - 0.5) + 0.5 + brightness; // Apply contrast and brightness
        color = max(vec3(0.0), color); //Clamp between 0
        color = min(vec3(1.0), color); //and 1

        vec3 desaturated = vec3(dot(color, vec3(0.299, 0.587, 0.114))); // Convert to grayscale
        color = mix(desaturated, color, saturation); // Apply custom saturation to grayscale
        color = max(vec3(0.0), color); //Clamp between 0
        color = min(vec3(1.0), color); //and 1
    
        color = pow(color, vec3(gamma)); // Convert back from linear (RGB) to gamma (sRGB) (aka. gamma correction)
        color = max(vec3(0.0), color); //Clamp between 0
        color = min(vec3(1.0), color); //and 1
        
        return color;
    }
//END OF COLOR CORRECTION

void main()
{
    vec4 texColor = texture(screenTexture, UV0);
    vec3 initialColor = texColor.rgb;
    if(!bloomEnabled && !exposureEnabled && !whiteBalanceEnabled && !colorCorrectionEnabled)
    {
        fragColor = texColor;
        return;
    }
    
    float threshold = 0.65; // Threshold for bloom - lower = more bloom
    float blurSize = 8.0; // Size of bloom blur - larger = more blur
    vec3 bloomColor;
    if(bloomEnabled) bloomColor = bloom(initialColor,  screenTexture, UV0, resolution, threshold, blurSize);
    else bloomColor = initialColor;
    
    float exposureAmount = 3.0; // Amount of exposure - larger = brighter
    vec3 exposureColor;
    if(exposureEnabled) exposureColor = exposure(bloomColor, screenTexture, UV0, exposureAmount);
    else exposureColor = bloomColor;
    
    float temperature = 0.1; // Temperature of white balance - larger = warmer
    float tint = 0.0; // Tint of white balance - larger = cooler
    vec3 whiteBalanceColor;
    if(whiteBalanceEnabled) whiteBalanceColor = whiteBalance(exposureColor, screenTexture, UV0, temperature, tint);
    else whiteBalanceColor = exposureColor;
    
    float contrast = 1.0; // Contrast of color correction - larger = more contrast
    float brightness = 0.0; // Brightness of color correction - larger = brighter
    float saturation = 1.0; // Saturation of color correction - larger = more saturation
    float gamma = 1; // Gamma of color correction - larger = brighter
    vec3 correctedColor;
    if(colorCorrectionEnabled) correctedColor = colorCorrection(whiteBalanceColor, screenTexture, UV0, contrast, brightness, saturation, 1 / gamma);
    else correctedColor = whiteBalanceColor;
    
    fragColor = vec4(correctedColor, texColor.a);
}