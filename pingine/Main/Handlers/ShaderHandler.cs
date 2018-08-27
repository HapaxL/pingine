using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace pingine.Main.Handlers
{
    public class ShaderHandler
    {
        private List<int> Shaders;

        public ShaderHandler()
        {
            Shaders = new List<int>();
        }

        public void AddShader(ShaderType type, string file)
        {
            int shader = GL.CreateShader(type); // create the shader object
            GL.ShaderSource(shader, File.ReadAllText(file)); // populate it with the contents of a shader file
            Shaders.Add(shader);
        }

        public int CompileProgram()
        {
            foreach (int shader in Shaders)
            {
                GL.CompileShader(shader); // compile the shader
                LogHandler.LogDebugInfo($"GL.CompileShader [{shader}]", GL.GetShaderInfoLog(shader)); // log info for the shader
            }

            /* the program object that will execute all our shader code */
            var program = GL.CreateProgram();

            /* we attach our shaders to the program */
            foreach (int shader in Shaders)
            {
                GL.AttachShader(program, shader);
            }

            GL.BindAttribLocation(program, 0, "position");
            GL.BindAttribLocation(program, 1, "color");

            /* we "link" (~= compile/load in memory) the program */
            GL.LinkProgram(program);
            LogHandler.LogDebugInfo($"GL.LinkProgram [{program}]", GL.GetProgramInfoLog(program)); // log info for the program

            /* then we can detach and delete the shaders in order to save up memory */
            foreach (int shader in Shaders)
            {
                GL.DetachShader(program, shader);
                GL.DeleteShader(shader);
            }
            
            return program;
        }
    }
}
