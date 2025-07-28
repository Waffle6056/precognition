shader_type canvas_item;
//render_mode blend_add;
uniform float transparency;
void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	if (COLOR == vec4(0,0,0,1))
		COLOR = vec4(0,0,0,0);
	else
		COLOR = vec4(1,COLOR.gbr);
	
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
