// Output data ; will be interpolated for each fragment.
varying vec4 ShadowCoord;
varying vec4 color;

// Values that stay constant for the whole scene.
uniform mat4 MVP;
uniform mat4 DepthBiasMVP;

void main(){
	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
	
	ShadowCoord = DepthBiasMVP * gl_Vertex;

	color = gl_Color;
}
