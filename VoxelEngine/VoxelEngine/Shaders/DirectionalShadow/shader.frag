// Interpolated values from the vertex shaders
varying vec4 ShadowCoord;
varying vec4 color;

// Values that stay constant for the whole mesh.
uniform sampler2D shadowMap;

void main(){

	float visibility = 1.0;
 	if ( texture( shadowMap, ShadowCoord.xy ).z  <  ShadowCoord.z){
      visibility = 0.5;
 	}

	gl_FragColor = color*visibility;
}