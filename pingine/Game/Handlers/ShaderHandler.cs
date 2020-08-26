using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace pingine.Game.Handlers
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
            Game.LogHandler.LogGLError("addshader_first", GL.GetError());

            int shader = GL.CreateShader(type); // create the shader object

            Game.LogHandler.LogGLError("addshader_aftercreateshader", GL.GetError());

            GL.ShaderSource(shader, File.ReadAllText(file)); // populate it with the contents of a shader file

            Game.LogHandler.LogGLError("addshader_aftershadersource", GL.GetError());

            Shaders.Add(shader);

            Game.LogHandler.LogGLError("addshader_afteraddshader", GL.GetError());
        }

        public int CompileProgram()
        {
            Game.LogHandler.LogGLError("compileprogram_first", GL.GetError());

            foreach (int shader in Shaders)
            {
                GL.CompileShader(shader); // compile the shader
                Game.LogHandler.LogDebugInfo($"GL.CompileShader [{shader}]", GL.GetShaderInfoLog(shader)); // log info for the shader
            }

            Game.LogHandler.LogGLError("compileprogram_aftercompileshaders", GL.GetError());

            /* the program object that will execute all our shader code */
            var program = GL.CreateProgram();

            Game.LogHandler.LogGLError("compileprogram_aftercreateprogram", GL.GetError());

            /* we attach our shaders to the program */
            foreach (int shader in Shaders)
            {
                GL.AttachShader(program, shader);
            }

            Game.LogHandler.LogGLError("compileprogram_afterattachshaders", GL.GetError());


            GL.BindAttribLocation(program, 0, "position");
            GL.BindAttribLocation(program, 1, "color");

            Game.LogHandler.LogGLError("compileprogram_afterbindattriblocation", GL.GetError());

            /* we "link" (~= compile/load in memory) the program */
            GL.LinkProgram(program);
            Game.LogHandler.LogDebugInfo($"GL.LinkProgram [{program}]", GL.GetProgramInfoLog(program)); // log info for the program

            Game.LogHandler.LogGLError("compileprogram_afterlink", GL.GetError());

            /* then we can detach and delete the shaders in order to save up memory */
            foreach (int shader in Shaders)
            {
                GL.DetachShader(program, shader);
                GL.DeleteShader(shader);
            }

            Game.LogHandler.LogGLError("compileprogram_afterdetachshader", GL.GetError());

            return program;
        }
    }
}
