shader_type spatial;
//render_mode unshaded;
uniform vec4 color : source_color;
void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	
	
	ALBEDO = color.rgb;
	ALPHA = 1.0;
	DEPTH = FRAGCOORD.z;
	//ALBEDO = vec3(DEPTH, DEPTH, DEPTH);
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
