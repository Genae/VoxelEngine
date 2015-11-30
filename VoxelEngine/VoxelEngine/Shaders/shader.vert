//varying vec3 v;

//passed variables
varying vec4 vColor;


void main(void)  
{    
	//varying variables are shared between vertex and fragment shader
	//in fragment shader they get interpolated correctly! 
	//v = vec3(gl_ModelViewMatrix * gl_Vertex);


	//normalDir = normalize(gl_NormalMatrix * gl_Normal);
	//angle = dot(normalDir, vec3(-1.0, -1.0, -1.0););
	//lightDir = vec3(-1.0, -1.0, -1.0);

	//vec4 for test purposes, gl_Color correct!
	vColor = vec4(1.0, 0.0, 0.0, 1.0) * pow(dot(normalize(gl_NormalMatrix * gl_Normal), vec3(0.0, -1.0, 0.0)), 2.0);

	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;  
}