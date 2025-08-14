#version 450

// This is the uniform buffer that contains all of the settings we sent over from the cpu in _render_callback. Must match with the one in the vertex shader, they're technically the same thing occupying the same spot in memory this is just duplicate code required for compilation.
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 lightSpaceMatrix;
uniform vec3 viewPos;
uniform float _GradientRotation;
uniform float _NoiseRotation;
uniform float _TerrainHeight;
uniform vec2 _AngularVariance;
uniform float _Scale;
uniform float _Octaves;
uniform float _AmplitudeDecay;
uniform vec3 _Offset;
uniform float _Seed;
uniform float _InitialAmplitude;
uniform float _Lacunarity;
uniform vec2 _SlopeRange;
uniform float _FrequencyVarianceLowerBound;
uniform float _FrequencyVarianceUpperBound;
uniform float _SlopeDamping;
uniform float _GrassThreshold;
uniform float _RockThreshold;
uniform int numPointLights;

// These are the variables that we expect to receive from the vertex shader
//in vec4 v_Color;
in vec2 UV0;
in vec3 pos;
in vec3 Normals;
in vec4 FragPosLightSpace;

vec3 normals;


// This is what the fragment shader will output, usually just a pixel color
out vec4 fragColor;

#define PI 3.141592653589793238462

struct PointLight
{
    vec3 lightColor;
    vec3 lightPos;
    float lightIntensity;

};


uniform vec3 _directionalLightColor;
uniform vec3 _directionalLightPos;
uniform float _directionalLightIntensity;

uniform sampler2D waterTexture;
uniform sampler2D grassTexture;
uniform sampler2D rockTexture;

uniform sampler2D shadowMap;

uniform PointLight pointLights[10];

vec4 colorTex;
vec3 lightDir;

float calculateShadow()
{
    // 1. Perform perspective divide
    vec3 projCoords = FragPosLightSpace.xyz / FragPosLightSpace.w;

    // 2. Transform from [-1,1] range to [0,1] texture coordinate range
    projCoords = projCoords * 0.5 + 0.5;

    // 3. Get the closest depth from the light's perspective (from the shadow map)
    float closestDepth = texture(shadowMap, projCoords.xy).r;

    // 4. Get the current fragment's depth from the light's perspective
    float currentDepth = projCoords.z;

    // 5. Add a small bias to prevent "shadow acne"
    float bias = max(0.05 * (1.0 - dot(normals, lightDir)), 0.005);

    // 6. Check if the current fragment is further away than the closest depth
    // If it is, then it's in shadow. PCF can be used here for soft shadows.
    float shadow = currentDepth - bias > closestDepth ? 1.0 : 0.0;

    //Prevent shadows on back faces when light is behind object
    if(projCoords.z > 1.0) shadow = 0.0;

    return shadow;
}

vec3 HandleLighting()
{
    vec3 outCol;

    // for(int i = 0; i < numPointLights; i++)
    // {

    //     const PointLight currentLight = pointLights[i];

    //     float ambientStrength = 0.2; //does not change
    //     vec3 ambient = ambientStrength * currentLight.lightColor;

    //     vec3 norms = normalize(normals); //no magnitude to them, cant make the light brighter
    //     lightDir = normalize(currentLight.lightPos - pos); //direction of light

    //     float diff = max(dot(norms, lightDir), 0); //diffuse light - max of the dot product (angle between normal and light direction). Value between -1 and 1. 1 is facing, -1 is nto facing and 0 is perpendicular.
    //     vec3 diffuse = diff * currentLight.lightColor * vec3(colorTex); //diffuse light color

    //     diffuse = vec3(min(diffuse.x, 1), min(diffuse.y, 1), min(diffuse.z, 1));

    //     const float shininess = 128;
    //     float specularStrength = 0.5; // between 0 - 1
    //     vec3 viewDir = normalize(viewPos - pos); //direction of view.  if eye is 90 degrees from reflection of light then it reflects right back. basicaly any angle of the object 90 from your eye you will see shine
    //     vec3 reflectionDirection = reflect(-lightDir, norms); //reflects the light direction off the normal
    //     float spec = pow(max(dot(viewDir, reflectionDirection), 0), shininess); //specular lightce
    //     vec3 specular = specularStrength * spec * currentLight.lightColor; //specular light color

    //     outCol += (ambient + diffuse + specular) * currentLight.lightIntensity;
    // }

	// Handle directional light
	float ambientStrength = 0.15; // Ambient light strength
	vec3 ambient = ambientStrength * _directionalLightColor;
	vec3 norms = normalize(Normals); // Normalized normals
	lightDir = normalize(_directionalLightPos - pos); // Direction of directional light (pointing towards the light source)
	float diff = max(dot(lightDir, norms), 0.0); // Diffuse light
	vec3 diffuse = diff * _directionalLightColor; // Diffuse light color
	//diffuse = vec3(min(diffuse.x, 1), min(diffuse.y, 1), min(diffuse.z, 1));
	const float shininess = 64;
	float specularStrength = 0.5; // Specular light strength
	vec3 viewDir = normalize(viewPos - pos); // Direction of view
	vec3 halfwayDir = normalize(lightDir + viewDir); // Reflects the light direction off the normal
	float spec = pow(max(dot(viewDir, halfwayDir), 0), shininess); // Specular light
	vec3 specular = specularStrength * spec * vec3(1,1,1); // Specular light color
	
	float shadow = calculateShadow();
	outCol += (ambient + (1.0 - shadow) * (diffuse + specular)) * _directionalLightIntensity * _directionalLightColor;

    return outCol;
}

// UE4's PseudoRandom function
		// https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Shaders/Private/Random.ush
		float pseudo(vec2 v) {
			v = fract(v/128.)*128. + vec2(-64.340622, -72.465622);
			return fract(dot(v.xyx * v.xyy, vec3(20.390625, 60.703125, 2.4281209)));
		}

		// Takes our xz positions and turns them into a random number between 0 and 1 using the above pseudo random function
		float HashPosition(vec2 pos) {
			return pseudo(pos * vec2(_Seed, _Seed + 4));
		}

		// Generates a random gradient vector for the perlin noise lattice points, watch my perlin noise video for a more in depth explanation
		vec2 RandVector(float seed) {
			float theta = seed * 360 * 2 - 360;
			theta += _GradientRotation;
			theta = theta * PI / 180.0;
			return normalize(vec2(cos(theta), sin(theta)));
		}

		// Normal smoothstep is cubic -- to avoid discontinuities in the gradient, we use a quintic interpolation instead as explained in my perlin noise video
		vec2 quinticInterpolation(vec2 t) {
			return t * t * t * (t * (t * vec2(6) - vec2(15)) + vec2(10));
		}

		// Derivative of above function
		vec2 quinticDerivative(vec2 t) {
			return vec2(30) * t * t * (t * (t - vec2(2)) + vec2(1));
		}

		// it's perlin noise that returns the noise in the x component and the derivatives in the yz components as explained in my perlin noise video
		vec3 perlin_noise2D(vec2 pos) {
			vec2 latticeMin = floor(pos);
			vec2 latticeMax = ceil(pos);

			vec2 remainder = fract(pos);

			// Lattice Corners
			vec2 c00 = latticeMin;
			vec2 c10 = vec2(latticeMax.x, latticeMin.y);
			vec2 c01 = vec2(latticeMin.x, latticeMax.y);
			vec2 c11 = latticeMax;

			// Gradient Vectors assigned to each corner
			vec2 g00 = RandVector(HashPosition(c00));
			vec2 g10 = RandVector(HashPosition(c10));
			vec2 g01 = RandVector(HashPosition(c01));
			vec2 g11 = RandVector(HashPosition(c11));

			// Directions to position from lattice corners
			vec2 p0 = remainder;
			vec2 p1 = p0 - vec2(1.0);

			vec2 p00 = p0;
			vec2 p10 = vec2(p1.x, p0.y);
			vec2 p01 = vec2(p0.x, p1.y);
			vec2 p11 = p1;
			
			vec2 u = quinticInterpolation(remainder);
			vec2 du = quinticDerivative(remainder);

			float a = dot(g00, p00);
			float b = dot(g10, p10);
			float c = dot(g01, p01);
			float d = dot(g11, p11);

			// Expanded interpolation freaks of nature from https://iquilezles.org/articles/gradientnoise/
			float noise = a + u.x * (b - a) + u.y * (c - a) + u.x * u.y * (a - b - c + d);

			vec2 gradient = g00 + u.x * (g10 - g00) + u.y * (g01 - g00) + u.x * u.y * (g00 - g10 - g01 + g11) + du * (u.yx * (a - b - c + d) + vec2(b, c) - a);
			return vec3(noise, gradient);
		}

		// The fractional brownian motion that sums many noise values as explained in the video accompanying this project
		vec3 fbm(vec2 pos) {
			float lacunarity = _Lacunarity;
			float amplitude = _InitialAmplitude;

			// height sum
			float height = 0.0;

			// derivative sum
			vec2 grad = vec2(0.0);

			// accumulated rotations
			mat2 m = mat2(1.0, 0.0,
						  0.0, 1.0);

			// generate random angle variance if applicable
			float angle_variance = mix(_AngularVariance.x, _AngularVariance.y, HashPosition(vec2(_Seed, 827)));
			float theta = (_NoiseRotation + angle_variance) * PI / 180.0;

			// rotation matrix
			mat2 m2 = mat2(cos(theta), -sin(theta),
					  	   sin(theta),  cos(theta));
				
			mat2 m2i = inverse(m2);

			for(int i = 0; i < int(_Octaves); ++i) {
				vec3 n = perlin_noise2D(pos);
				
				// add height scaled by current amplitude
				height += amplitude * n.x;	
				
				// add gradient scaled by amplitude and transformed by accumulated rotations
				grad += amplitude * m * n.yz;
				
				// apply amplitude decay to reduce impact of next noise layer
				amplitude *= _AmplitudeDecay;
				
				// generate random angle variance if applicable
				angle_variance = mix(_AngularVariance.x, _AngularVariance.y, HashPosition(vec2(i * 419, _Seed)));
				theta = (_NoiseRotation + angle_variance) * PI / 180.0;

				// reconstruct rotation matrix, kind of a performance stink since this is technically expensive and doesn't need to be done if no random angle variance but whatever it's 2025
				m2 = mat2(cos(theta), -sin(theta),
					  	  sin(theta),  cos(theta));
				
				m2i = inverse(m2);

				// generate frequency variance if applicable
				float freq_variance = mix(_FrequencyVarianceLowerBound, _FrequencyVarianceUpperBound, HashPosition(vec2(i * 422, _Seed)));

				// apply frequency adjustment to sample position for next noise layer
				pos = (lacunarity + freq_variance) * m2 * pos;
				m = (lacunarity + freq_variance) * m2i * m;
			}

			return vec3(height, grad);
		}

void main()
{

    // Recalculate initial noise sampling position same as vertex shader
    vec3 noise_pos = (pos + vec3(_Offset.x, 0, _Offset.z)) / _Scale;

    // Calculate fbm, we don't care about the height just the derivatives here for the normal vector so the ` + _TerrainHeight - _Offset.y` drops off as it isn't relevant to the derivative
    vec3 n = _TerrainHeight * fbm(noise_pos.xz);

    // To more easily customize the color slope blending this is a separate normal vector with its horizontal gradients significantly reduced so the normal points upwards more
    vec3 slope_normal = normalize(vec3(-n.y, 1, -n.z) * vec3(_SlopeDamping, 1, _SlopeDamping));

    // Use the slope of the above normal to create the blend value between the two terrain colors
    float material_blend_factor = smoothstep(_SlopeRange.x, _SlopeRange.y, 1 - slope_normal.y);

    
    
    vec4 waterColor = texture(waterTexture, UV0);
    vec4 grassColor = texture(grassTexture, UV0);
    vec4 rockColor = texture(rockTexture, UV0);

    // vec4 waterColor = vec4(1,0,0,1); 
    // vec4 grassColor =  vec4(0,1,0,1); 
    // vec4 rockColor = vec4(0, 0, 1, 1); 

    float waterLevel = 0.0; 
    float grassLevel = _TerrainHeight * _GrassThreshold * 0.5;  
    float rockLevel = _TerrainHeight * _RockThreshold * 0.5; 
    
    float blend1 = smoothstep(waterLevel, grassLevel, pos.y);
    float blend2 = smoothstep(grassLevel, rockLevel, pos.y); 

    
    vec4 baseColor = mix(waterColor, grassColor, blend1);
    baseColor = mix(baseColor, rockColor, blend2);

    colorTex = baseColor;

	normals = Normals;

    //fragColor = vec4(normalize(normals), 1.0);
    fragColor = vec4(colorTex.rgb * HandleLighting(), colorTex.a); 
}