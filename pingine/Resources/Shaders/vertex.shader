#version 150 core
#extension GL_ARB_explicit_attrib_location: enable

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 texcoord;

uniform mat4 projection;

out vec4 vs_color;
out vec2 vs_texcoord;

void main(void)
{
    gl_Position = projection * position;
	vs_color = color;
	vs_texcoord = texcoord;

	// gl_Position = vec4(0.55, 0.25, 0.5, 1.0);
}