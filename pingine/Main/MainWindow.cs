using System;
using System.Diagnostics;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using pingine.Main.Graphics;
using pingine.Main.Handlers;

namespace pingine.Main
{
    /* main game window that also handles game logic */
    public sealed class MainWindow : GameWindow
    {
        KeyboardHandler KeyboardHandler;

        /* (used for tutorial purposes) */
        ColorHSLA BackgroundColor;

        /* (used for tutorial purposes) */
        int shaderProgramID;

        /* (used for tutorial purposes) */
        int vertexArrayID;

        /* total time elapsed since the application started
         * (used for tutorial purposes) */
        double totalTimeElapsed;

        /* initialization options */
		public MainWindow() : base(Config.WindowWidth, Config.WindowHeight, // window size
            GraphicsMode.Default,
            Config.GameName, // window title, gets overwritten by updating the Title attribute
            GameWindowFlags.Default,
            DisplayDevice.Default,
            /* we want version 3.2 for compatibility with Mac */
            3, // OpenGL major version
            2, // OpenGL minor version
            /* we need forward compatibility for Mac but it removes all deprecated features
             * so if something breaks at some point while coding, it's probably because it's
             * deprecated and we can't use it (like glBegin, glEnd, and Texture2D) */
            GraphicsContextFlags.ForwardCompatible) 
        {
            // Title += " - OpenGL Version: " + GL.GetString(StringName.Version); // "4.6.0 NVIDIA 397.64" on my PC

            KeyboardHandler = new KeyboardHandler(false); // we don't want repeat enabled for a video game (except in menus or when writing something)
            BackgroundColor = new ColorHSLA();
            BackgroundColor.H = 0; // any hue
            BackgroundColor.S = 1.0f; // full saturation
            BackgroundColor.L = 0.5f; // half luminosity (not completely dark (black), not completely bright (white))
            BackgroundColor.A = 1.0f; // full alpha channel (100% opaque)

            /* bind our OnClosed actions to the closing event */
            Closed += OnClosed;
        }

        /* actions to do on window (game) startup */
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // TODO: REFACTOR
            // tutorial stuff
            /* the vertex shader is the entry point in the rendering pipeline
             * takes each raw vertex attribute data specified in step 1,
             * processes them and generates a vertex as output */
            var vertexShader = GL.CreateShader(ShaderType.VertexShader); // create the vertex shader object
            GL.ShaderSource(vertexShader, File.ReadAllText(Config.ResourceFolder + @"Shaders\VertexShader.vert")); // populate it with the contents of the vertex shader file
            GL.CompileShader(vertexShader); // compile the shader DUH
            var info = GL.GetShaderInfoLog(vertexShader); // log info for the shader
            if (!string.IsNullOrWhiteSpace(info))
                Debug.WriteLine($"GL.CompileShader [{ShaderType.VertexShader}] had info log: {info}");

            // tutorial stuff
            /* the fragment shader is step 8 in the rendering pipeline
             * takes each fragment from the rasterization step, and outputs
             * a set of possible colors, a depth value, and a stencil (?) value */
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText(Config.ResourceFolder + @"Shaders\FragmentShader.frag"));
            GL.CompileShader(fragmentShader);
            info = GL.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(info))
                Debug.WriteLine($"GL.CompileShader [{ShaderType.FragmentShader}] had info log: {info}");

            // tutorial stuff
            /* the program object that will execute all our shader code */
            var programID = GL.CreateProgram();
            
            /* we attach our vertex and fragment shaders to the program */
            GL.AttachShader(programID, vertexShader);
            GL.AttachShader(programID, fragmentShader);

            /* we "link" (~= compile and load) the program */
            GL.LinkProgram(programID);
            info = GL.GetProgramInfoLog(programID); // log info for the program
            if (!string.IsNullOrWhiteSpace(info))
                Debug.WriteLine($"GL.LinkProgram had info log: {info}");

            /* then we can detach and delete the shaders in order to save up memory */
            GL.DetachShader(programID, vertexShader);
            GL.DetachShader(programID, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            /* save the program ID somewhere */
            shaderProgramID = programID;
            
            /* generate vertex array objects (VAOs) whose IDs aren't already in use */
            GL.GenVertexArrays(1, out vertexArrayID);

            /* bind the VAOs to our OpenGL context for them to be used in the OpenGL process */
            GL.BindVertexArray(vertexArrayID);
        }

        /* actions to do on window resize */
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height); // we reset the viewport which i'm not sure is something we want? to be investigated
        }

        /* adding keyboard handling logic, we probably don't want to add anything else in here */
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            KeyboardHandler.AddTriggerEvent(e.Key);
        }

        /* adding keyboard handling logic, we probably don't want to add anything else in here */
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            KeyboardHandler.AddReleaseEvent(e.Key);
        }

        /* update logic 
         * runs X times every second, where X is the value given as parameter for the MainWindow.Run method */
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            ProcessKeys(); // temporary (?) function for testing purposes?
            BackgroundColor.H = (BackgroundColor.H + 0.002f) % 1;
            // do the update logic here

            KeyboardHandler.ResetKeys(); // at the end of the update frame, we reset all keyboard trigger/release events
        }

        private void ProcessKeys()
        {
            Title = $"waiting for a press on E (R enables repeat, T disables it)";

            if (KeyboardHandler.IsHeld(Key.E))
            {
                Title += $": E is being held";
            }

            if (KeyboardHandler.IsTriggered(Key.E))
            {
                Title += $": E was triggered";
            }

            if (KeyboardHandler.IsReleased(Key.E))
            {
                Title += $": E was released";
            }

            if (KeyboardHandler.IsTriggered(Key.R))
            {
                KeyboardHandler.EnableRepeat(true);
            }

            if (KeyboardHandler.IsTriggered(Key.T))
            {
                KeyboardHandler.EnableRepeat(false);
            }

            /* temporary: use Escape to instantly quit the game */
            if (KeyboardHandler.IsTriggered(Key.Escape))
            {
                Close();
            }
        }

        /* render logic
         * refreshes the graphics displayed in MainWindow
         * if Vsync is off: this method gets called as often as possible
         * if Vsync is on (default): is locked to the monitor's refresh rate (usually 60 or 120 FPS) */
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            /* get the time elapsed since last frame,
             * it's a surprise tool that will help us later */
            totalTimeElapsed += e.Time;

            // Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}"; // for debug purposes

            // background color
            Color4 backColor = BackgroundColor.ToRGBA();

            // applies the background color
            GL.ClearColor(backColor);

            // clear the color buffer and the depth buffer (why? who knows lmao)
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // tutorial stuff
            /* this outputs some point or whatever thanks to
             * the shader and the array data that we created on load */
            GL.UseProgram(shaderProgramID); // we choose to use our program in our pipeline

            /* the other entry point of the pipeline: attributes
             * those are sent as input to the vertex shader which needs an "in" attribute with
             * the corresponding index (set with "layout (location = n)" in the vertex shader file)
             * only the vertex shader can take in attributes from the outside, if you want to
             * send information to the next shaders in the pipeline you need to make them go through
             * the vertex shader and do some 1:1 mapping like "fs_color = color" or whatever */
            // GL.VertexAttrib4(0, position);

            /* vertex specification is the entry point of the pipeline
             * we declare things we want to draw, and which kind */
            /* we specify that we want to take <count> VAO from our (active) vertex array(s),
             * starting at index <first>, and treat it as a <mode>
             * this gives us a primitive of the specified type that will be processed by the pipeline
             * or something like that idk */
            GL.DrawArrays(PrimitiveType.Points, 0, 1); 

            /* we can apply some modifications to our primitives */
            GL.PointSize(100); // change point size so that it's bigger than 1 pixel and we can see it

            /* the scene is being drawn on the "back" buffer, which is hidden behind the "front" buffer
             * after we're finished drawing the scene on the back buffer, we use SwapBuffers()
             * so that the back buffer replaces the front buffer and displays the scene to the player
             * then the buffer previously in front becomes the back buffer and can be drawn on for the next frame
             * so this call is basically saying "we're done with drawing for this frame,
             * let's display the result to the user */
            SwapBuffers();
        }

        /* actions to do on window close */
        private void OnClosed(object sender, EventArgs eventArgs)
        {
            Exit();
        }

        /* application exit */
        public override void Exit()
        {
            // save the planet, free your memory
            /* delete the vertex arrays we were using */
            GL.DeleteVertexArrays(1, ref vertexArrayID);
            /* delete the program we were using */
            GL.DeleteProgram(shaderProgramID);

            /* exit application */
            base.Exit();
        }
    }
}