#version 150 core

// layout(location = 0) in vec4 position;
// layout(location = 1) in vec4 color;
// out vec4 vs_color;

void main(void)
{
    // gl_Position = position;
    // vs_color = color;

	gl_Position = vec4(0.25, -0.25, 0.5, 1.0);
}