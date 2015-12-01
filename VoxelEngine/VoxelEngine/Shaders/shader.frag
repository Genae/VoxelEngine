//variables passed to fragment shader
varying vec4 vColor;  
varying vec3 vNormal;

uniform vec3 viewDirection;

void main (void)  
{  

	// light direction = vec3(0.7, -1.0, 0.3)
	//vec3 reflectedLight = reflect(vec3(0.7, -1.0, 0.3), vNormal);
	//vec4 specularLight = gl_Color * pow(max(0.0, dot(reflectedLight, normalize(viewDirection))), 3);
	


	//0.15 = ambient Light
	gl_FragColor = 0.15 + vColor;
}