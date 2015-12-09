//variables passed to fragment shader
varying vec4 vColor;
varying vec3 vNormal;
varying vec4 vColorAmb;

uniform vec3 direction;
void main(void)  
{    
	//varying variables are shared between vertex and fragment shader
	//in fragment shader they get interpolated correctly! 
	//v = vec3(gl_ModelViewMatrix * gl_Vertex);


	//Normal Vector = normalize(gl_Normal)
	//Light Direction = vec3(0.7, -1.0, 0.3) change in fragment shader too!!!
	//Angle = dot(normalDir, Light Direction);

	vNormal = normalize(gl_Normal);
	vColor = gl_Color * max(dot(vNormal, direction), 0.0);
	vColorAmb = gl_Color * 0.15;

	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;  
}