shader_type spatial;
render_mode unshaded, fog_disabled;
uniform sampler2D depth_texture : hint_depth_texture, repeat_disable, filter_nearest;
void vertex() {
	// Called for every vertex the material is visible on.
	POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {
	// Called for every pixel the material is visible on.
	float depth = texture(depth_texture, SCREEN_UV).x;
    //vec4 upos = INV_PROJECTION_MATRIX * vec4(SCREEN_UV * 2.0 - 1.0, depth, 1.0);
    //vec3 pixel_position = upos.xyz / upos.w;
	//ALBEDO = pixel_position;
	DEPTH = depth;
	//ALBEDO = vec3(depth,depth,depth);
	ALBEDO = vec3(0,0,0);
	ALPHA = 1.0;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
