using System;
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

        /* scene background color (used for tutorial purposes) */
        ColorHSLA BackgroundColor;

        /* shader program ID (used for tutorial purposes) */
        int shaderProgramID;

        /* vertex array ID (used for tutorial purposes) */
        int vertexArrayID;

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
            BackgroundColor.H = 0;
            BackgroundColor.S = 1.0f;
            BackgroundColor.L = 0.5f;
            BackgroundColor.A = 1.0f;
        }

        /* actions to do on window (game) startup */
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // tutorial stuff
            /* the vertex shader is step 2 in the rendering pipeline
             * takes each raw vertex attribute data specified in step 1,
             * processes them and generates a vertex as output */
            var vertexShader = GL.CreateShader(ShaderType.VertexShader); // create the vertex shader object
            GL.ShaderSource(vertexShader, File.ReadAllText(Config.ResourceFolder + @"Shaders\VertexShader.vert")); // populate it with the contents of the vertex shader file
            GL.CompileShader(vertexShader); // compile the shader DUH

            // tutorial stuff
            /* the fragment shader is step 8 in the rendering pipeline
             * takes each fragment from the rasterization step, and outputs
             * a set of possible colors, a depth value, and a stencil (?) value */
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText(Config.ResourceFolder + @"Shaders\FragmentShader.frag"));
            GL.CompileShader(fragmentShader);

            var programID = GL.CreateProgram();
            GL.AttachShader(shaderProgramID, vertexShader);
            GL.AttachShader(shaderProgramID, fragmentShader);
            GL.LinkProgram(shaderProgramID);

            GL.DetachShader(shaderProgramID, vertexShader);
            GL.DetachShader(shaderProgramID, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            shaderProgramID = programID;
            
            GL.GenVertexArrays(1, out vertexArrayID);
            GL.BindVertexArray(vertexArrayID);

            Closed += OnClosed;
        }

        /* actions to do on window close */
        private void OnClosed(object sender, EventArgs eventArgs)
        {
            Exit();
        }

        /* application exit */
        public override void Exit()
        {
            GL.DeleteVertexArrays(1, ref vertexArrayID);
            GL.DeleteProgram(shaderProgramID);
            base.Exit();
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
                Exit();
            }
        }

        /* render logic
         * refreshes the graphics displayed in MainWindow
         * if Vsync is off: this method gets called as often as possible
         * if Vsync is on (default): is locked to the monitor's refresh rate (usually 60 or 120 FPS) */
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}"; // for debug purposes

            // background color
            Color4 backColor = BackgroundColor.ToRGBA();

            // clear the color buffers, replacing them with the background color
            GL.ClearColor(backColor);

            // clear the color buffer and the depth buffer (why? who knows lmao)
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            /* tutorial stuff
             * this outputs some point or whatever thanks to
             * the shader and the array data that we created on load */
            GL.UseProgram(shaderProgramID); // apply the chosen shader program
            GL.DrawArrays(PrimitiveType.Points, 0, 1); // draw array data as point(s)
            GL.PointSize(100); // change point size so that it's bigger than 1 pixel and we can see it

            /* we're working with quads (squares/rectangles) on which we will apply different textures
             * in order to obtain individual sprites */
            // GL.Begin(BeginMode.Quads); // <- deprecated, use VBOs/VAOs and "primitiveType overload" instead
            // GL.DrawElements(BeginMode.Quads, 4, DrawElementsType.UnsignedInt, IntPtr.Zero); // <- also obsolete??? use "primitiveType overload" instead
            // GL.Begin(PrimitiveType.Quads); // not marked as deprecated????? but it still breaks everything......
            // GL.DrawElements(PrimitiveType.Quads, 4, DrawElementsType.UnsignedInt, IntPtr.Zero);

            /* the scene is being drawn on the "back" buffer, which is hidden behind the "front" buffer
             * after we're finished drawing the scene on the back buffer, we use SwapBuffers()
             * so that the back buffer replaces the front buffer and displays the scene to the player
             * then the buffer previously in front becomes the back buffer and can be drawn on for the next frame */
            SwapBuffers();
        }
    }
}