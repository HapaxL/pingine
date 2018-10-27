﻿#version 150 core
#extension GL_ARB_explicit_attrib_location: enable

in vec4 vs_color;
in vec2 vs_texcoord;

uniform sampler2D tex;

out vec4 fs_color;

void main(void)
{
	fs_color = texture(tex, vs_texcoord); // *vs_color;
	// fs_color = vs_color;
	// color = vec4(1.0, 0.0, 0.0, 1.0);
}