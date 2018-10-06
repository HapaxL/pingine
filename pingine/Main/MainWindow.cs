using System;
using System.Collections.Generic;
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
        ShaderHandler ShaderHandler;
        KeyboardHandler KeyboardHandler;

        /* (used for tutorial purposes) */
        ColorHSLA BackgroundColor;

        /* (used for tutorial purposes) */
        int ShaderProgramID;

        /* (used for tutorial purposes) */
        List<RenderObject> RenderObjects = new List<RenderObject>();

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

            Console.WriteLine("init_beforeshaderhandler " + GL.GetError());
            ShaderHandler = new ShaderHandler();
            Console.WriteLine("init_aftershaderhandler " + GL.GetError());
            KeyboardHandler = new KeyboardHandler(false); // we don't want repeat enabled for a video game (except in menus or when writing something)
            BackgroundColor = new ColorHSLA
            {
                H = 0, // any hue
                S = 1.0f, // full saturation
                L = 0.5f, // half luminosity (not completely dark (black), not completely bright (white))
                A = 1.0f // full alpha channel (100% opaque)
            };

            /* bind our OnClosed actions to the closing event */
            Closed += OnClosed;
        }

        private int CreateProgram()
        {
            Console.WriteLine("createprog_first " + GL.GetError());
            /* the vertex shader is the first step in the rendering pipeline
             * takes each raw vertex attribute data specified beforehand,
             * processes them and generates a vertex as output */
            ShaderHandler.AddShader(ShaderType.VertexShader, Config.ResourceFolder + @"Shaders\vertex.shader");

            Console.WriteLine("createprog_aftervertexadd " + GL.GetError());

            /* the fragment shader is another step in the rendering pipeline
             * takes each fragment from the rasterization step, and outputs
             * a set of possible colors, a depth value, and a stencil (?) value
             * (i don't think we actually need a fragment shader? the only
             * one we need now is the one that applies texture on surfaces) */
            ShaderHandler.AddShader(ShaderType.FragmentShader, Config.ResourceFolder + @"Shaders\fragment.shader");

            Console.WriteLine("createprog_afterfragmentadd " + GL.GetError());

            /* create the pipeline program */
            var compile = ShaderHandler.CompileProgram();

            Console.Write(compile + " ");
            Console.WriteLine("createprog_aftercompile " + GL.GetError());

            return compile;
        }

        /* actions to do on window (game) startup */
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            Vertex[] vertices =
            {
                new Vertex(new Vector4(0f, 0f, 0f, 1.0f), new Color4(1.0f, 0f, 0f, 1.0f)),
                new Vertex(new Vector4(0.5f, 0f, 0f, 1.0f), new Color4(0f, 1.0f, 0f, 1.0f)),
                new Vertex(new Vector4(0.5f, 0.5f, 0f, 1.0f), new Color4(0f, 0f, 1.0f, 1.0f)),
                new Vertex(new Vector4(0f, 0.5f, 0f, 1.0f), new Color4(1.0f, 0f, 1.0f, 1.0f)),

                new Vertex(new Vector4(0f - 0.2f, 0f - 0.2f, 0.5f, 1.0f), new Color4(1.0f, 0f, 0f, 1.0f)),
                new Vertex(new Vector4(0.5f - 0.2f, 0f - 0.2f, 0.5f, 1.0f), new Color4(0f, 1.0f, 0f, 1.0f)),
                new Vertex(new Vector4(0.5f - 0.2f, 0.5f - 0.2f, 0.5f, 1.0f), new Color4(0f, 0f, 1.0f, 1.0f)),
                new Vertex(new Vector4(0f - 0.2f, 0.5f - 0.2f, 0.5f, 1.0f), new Color4(1.0f, 0f, 1.0f, 1.0f)),
            };
            
            RenderObjects.Add(new RenderObject(vertices));

            Console.WriteLine("load_afterrenderinit " + GL.GetError());

            ShaderProgramID = CreateProgram();
            
            Console.WriteLine("load_afterprogcreate " + GL.GetError());

            /* draw both the front and back of the polygon in fill mode,
             * as opposed to only the outlines (line) or the vertices (point) */
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            /* enable the option that makes opengl check the depth (z-azis)
             * of every pixel it draws, it order to not draw things that 
             * are supposed to be far-away on top of things that are supposed
             * to be in the front of the screen */
            GL.Enable(EnableCap.DepthTest);

            /* it seems we don't need to explicitly create a projection
             * matrix because it's in orthographic projection by default
             * (position on the z-azis doesn't affect x and y coordinates) */
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
            BackgroundColor.H = (BackgroundColor.H + 0.002f) % 1; // changing background color o nevery frame for testing purposes
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
                Title += $": and E was finally released!";
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
            
            Console.WriteLine("afterbase " + GL.GetError());

            /* get the time elapsed since last frame,
             * it's a surprise tool that will help us later */
            totalTimeElapsed += e.Time;

            // Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}"; // for debug purposes

            // background color
            Color4 backColor = BackgroundColor.ToRGBA();

            Console.WriteLine("afterbgset " + GL.GetError());

            // applies the background color
            GL.ClearColor(backColor);
            
            Console.WriteLine("aftercolorclear " + GL.GetError());

            // clear the color buffer and the depth buffer (why? who knows lmao)
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            Console.WriteLine("aftermaskclear " + GL.GetError());

            // tutorial stuff
            /* we created a "program" earlier with CreateProgram()
             * which describes the contents of the pipeline and what
             * it will do to incoming input */
            GL.UseProgram(ShaderProgramID); // we choose to use our program in our pipeline

            /* the other entry point of the pipeline: attributes
             * those are sent as input to the vertex shader which needs an "in" attribute with
             * the corresponding index (set with "layout (location = n)" in the vertex shader file)
             * only the vertex shader can take in attributes from the outside, if you want to
             * send information to the next shaders in the pipeline you need to make them go through
             * the vertex shader and do some 1:1 mapping like "fs_color = color" or whatever */
            // GL.VertexAttrib4(0, position);

            Console.WriteLine("afterproguse " + GL.GetError());

            /* vertex specification is the entry point of the pipeline
             * we declare things we want to draw, and which kind */
            foreach (var renderObject in RenderObjects)
            {
                renderObject.Render();
            }

            /* we can apply some modifications to our primitives */
            // GL.PointSize(10); // change point size so that it's bigger than 1 pixel and we can see it

            Console.WriteLine("afterrender " + GL.GetError());

            /* the scene is being drawn on the "back" buffer, which is hidden behind the "front" buffer
             * after we're finished drawing the scene on the back buffer, we use SwapBuffers()
             * so that the back buffer replaces the front buffer and displays the scene to the player
             * then the buffer previously in front becomes the back buffer and can be drawn on for the next frame
             * so this call is basically saying "we're done with drawing for this frame,
             * let's display the result to the user" */
            SwapBuffers();

            Console.WriteLine("afterswap " + GL.GetError());
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
            foreach (var renderObject in RenderObjects)
                renderObject.Dispose();
            /* delete the program we were using */
            GL.DeleteProgram(ShaderProgramID);

            /* exit application */
            base.Exit();
        }
    }
}