using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System;

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
            Console.WriteLine("addshader_first " + GL.GetError());

            int shader = GL.CreateShader(type); // create the shader object

            Console.WriteLine("addshader_aftercreateshader " + GL.GetError());

            GL.ShaderSource(shader, File.ReadAllText(file)); // populate it with the contents of a shader file
            
            Console.WriteLine("addshader_aftershadersource " + GL.GetError());

            Shaders.Add(shader);

            Console.WriteLine("addshader_afteraddshader " + GL.GetError());
        }

        public int CompileProgram()
        {
            Console.WriteLine("compileprogram_first " + GL.GetError());

            foreach (int shader in Shaders)
            {
                GL.CompileShader(shader); // compile the shader
                LogHandler.LogDebugInfo($"GL.CompileShader [{shader}]", GL.GetShaderInfoLog(shader)); // log info for the shader
            }

            Console.WriteLine("compileprogram_aftercompileshaders " + GL.GetError());

            /* the program object that will execute all our shader code */
            var program = GL.CreateProgram();

            Console.WriteLine("compileprogram_aftercreateprogram " + GL.GetError());

            /* we attach our shaders to the program */
            foreach (int shader in Shaders)
            {
                GL.AttachShader(program, shader);
            }

            Console.WriteLine("compileprogram_afterattachshaders " + GL.GetError());


            GL.BindAttribLocation(program, 0, "position");
            GL.BindAttribLocation(program, 1, "color");
            
            Console.WriteLine("compileprogram_afterbindattriblocation " + GL.GetError());

            /* we "link" (~= compile/load in memory) the program */
            GL.LinkProgram(program);
            LogHandler.LogDebugInfo($"GL.LinkProgram [{program}]", GL.GetProgramInfoLog(program)); // log info for the program
            
            Console.WriteLine("compileprogram_afterlink " + GL.GetError());

            /* then we can detach and delete the shaders in order to save up memory */
            foreach (int shader in Shaders)
            {
                GL.DetachShader(program, shader);
                GL.DeleteShader(shader);
            }

            Console.WriteLine("compileprogram_afterdetachshader " + GL.GetError());

            return program;
        }
    }
}
