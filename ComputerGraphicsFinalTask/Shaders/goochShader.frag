#version 440 core

out vec4 fragColor;

uniform sampler2D modelTex[10];
uniform vec2 resolution;

struct PointLight
{
    vec3 lightColor;
    vec3 lightPos;
    float lightIntensity;

};

uniform PointLight pointLights[10];
uniform int numPointLights;

uniform vec3 viewPos;

uniform int modelIndex;

//in vec4 myFragColor;
in vec2 UV0;
in vec3 FragPos;
in vec3 Normals;

vec4 colorTex;

const float LINE_WEIGHT = 1.0;

vec3 HandleLighting()
{
    vec3 outCol;

    for(int i = 0; i < numPointLights; i++)
    {
        
        vec3 albedo = colorTex.rgb; //color of the object
        
        const float smoothness = 0.214; //between 0 - 1
        const vec3 coolColor = vec3(0.0, 0.0, 0.2); //cool color
        const float coolIntensity = 0.5; //between 0 - 1
        const vec3 warmColor = vec3(0.2, 0.2, 0.0); //warm color
        const float warmIntensity = 0.4; //between 0 - 1

        const PointLight currentLight = pointLights[i]; //current light
        const float shininess = 96; //between 0 - 256 
        const float specularStrength = 0.1f; // between 0 - 1
        
        vec3 norms = normalize(Normals); //normal of the pixel
        vec3 lightDir = normalize(currentLight.lightPos - FragPos); //direction of the light
        vec3 viewDir = normalize(viewPos - FragPos); //direction of the camera
        
        float ambientStrength = 0.75f; //between 0 - 1
        vec3 ambient = ambientStrength * currentLight.lightColor * albedo.rgb; //ambient color
        
        float goochDiffuse = (1.0f + dot(lightDir, norms)) / 2.0; // Calculate the diffuse term
        vec3 kCool = coolColor.rgb + coolIntensity * albedo.rgb; //mix cool color with pixel color
        vec3 kWarm = warmColor.rgb + warmIntensity * albedo.rgb; //mix warm color with pixel color
        vec3 gooch = (goochDiffuse * kWarm) + ((1.0f - goochDiffuse) * kCool); //Blend between cool and warm (-1 to 1) instead of black and light (0 to 1)

        vec3 reflectionDirection = reflect(-lightDir, norms); //reflection direction
        float spec = pow(max(dot(viewDir, reflectionDirection), 0), shininess); //specular light
        vec3 specular = specularStrength * spec * currentLight.lightColor; //specular color

        outCol = (gooch + specular) * currentLight.lightIntensity; //final color
    }

    return outCol;
}

void main()
{
    colorTex = vec4(1.0f, 0.75f, 0.79f, 1.0f);

    //Texture applying
    for(int i = 0; i < modelTex.length(); i++)
    {
        if(modelIndex == i)
        {
            colorTex = texture(modelTex[i], UV0);
        }
    }
    
    fragColor = vec4(HandleLighting(), colorTex.a);

}
