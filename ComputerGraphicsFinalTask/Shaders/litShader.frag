#version 440 core

out vec4 fragColor;

uniform sampler2D modelTex[10];

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

vec3 HandleLighting()
{
    vec3 outCol;

    for(int i = 0; i < numPointLights; i++)
    {

        const PointLight currentLight = pointLights[i];

        float ambientStrength = 0.2f; //does not change
        vec3 ambient = ambientStrength * currentLight.lightColor;

        vec3 norms = normalize(Normals); //no magnitude to them, cant make the light brighter
        vec3 lightDir = normalize(currentLight.lightPos - FragPos); //direction of light

        float diff = max(dot(norms, lightDir), 0); //diffuse light - max of the dot product (angle between normal and light direction). Value between -1 and 1. 1 is facing, -1 is nto facing and 0 is perpendicular.
        vec3 diffuse = diff * currentLight.lightColor * vec3(colorTex); //diffuse light color

        diffuse = vec3(min(diffuse.x, 1), min(diffuse.y, 1), min(diffuse.z, 1));

        const float shininess = 128;
        float specularStrength = 0.5f; // between 0 - 1
        vec3 viewDir = normalize(viewPos - FragPos); //direction of view.  if eye is 90 degrees from reflection of light then it reflects right back. basicaly any angle of the object 90 from your eye you will see shine
        vec3 reflectionDirection = reflect(-lightDir, norms); //reflects the light direction off the normal
        float spec = pow(max(dot(viewDir, reflectionDirection), 0), shininess); //specular lightce
        vec3 specular = specularStrength * spec * currentLight.lightColor; //specular light color

        outCol += (ambient + diffuse + specular) * currentLight.lightIntensity;
    }

    return outCol;
}

void main()
{
    colorTex = vec4(1.0f, 1.0f, 1.0f, 1.0f);

    //Texture applying
    for(int i = 0; i < modelTex.length(); i++)
    {
        if(modelIndex == i)
        {
            colorTex = texture(modelTex[i], UV0);
        }
    }
    
    fragColor = vec4(colorTex.rgb * HandleLighting(), colorTex.a);

}
