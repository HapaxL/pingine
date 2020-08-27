using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using pingine.Game.Graphics;

/*********************************************************\
*                                                         *
*   Simple, cross-platform 2D engine for OpenTK.NetCore   *
*               (C) Hapax - MIT License                   *
*          https://github.com/HapaxL/pingine              *
*                                                         *
\*********************************************************/

/* IMPORTANT NOTE: dependencies include System.Drawing.Common which requires libgdiplus
 * (or mono-libgdiplus?) in order to function correctly on Linux and Mac. 
 * source: https://github.com/dotnet/corefx/issues/20712 
 * more info: https://github.com/mellinoe/corefx/commit/48e96d5ac2e51a7a388aa5d066c9f516faa22e2d */

namespace pingine.Game
{
    /* main game window that also handles game logic */
    public sealed class MainWindow : GameWindow
    {
        /* orthographic projection matrix, sent to the vertex shader on every frame
         * so that it applies it to the vertices */
        Matrix4 OrthographicProjectionMatrix;

        /* (used for tutorial purposes) */
        ColorHSLA BackgroundColor;

        /* (used for tutorial purposes) */
        int ShaderProgramId;

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

            OrthographicProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, Config.WindowWidth, Config.WindowHeight, 0, 0, -100);
            Game.LogHandler.LogGLError("init_aftercreatematrix", GL.GetError());
            
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
            Game.LogHandler.LogGLError("createprog_first", GL.GetError());
            /* the vertex shader is the first step in the rendering pipeline
             * takes each raw vertex attribute data specified beforehand,
             * processes them and generates a vertex as output */
            switch (Config.TextureDisplayMode)
            {
                case TextureDisplayMode.Smooth:
                    Game.ShaderHandler.AddShader(ShaderType.VertexShader, Config.ResourceFolder + @"Shaders\tex_vertex_smooth.shader");
                    break;
                default:
                    Game.ShaderHandler.AddShader(ShaderType.VertexShader, Config.ResourceFolder + @"Shaders\tex_vertex_pixelperfect.shader");
                    break;
            }

            Game.LogHandler.LogGLError("createprog_aftervertexadd", GL.GetError());

            /* the fragment shader is another step in the rendering pipeline
             * takes each fragment from the rasterization step, and outputs
             * a set of possible colors, a depth value, and a stencil (?) value */
            /* this is the shader that handles colors and textures for vertices */
            Game.ShaderHandler.AddShader(ShaderType.FragmentShader, Config.ResourceFolder + @"Shaders\tex_fragment.shader");

            Game.LogHandler.LogGLError("createprog_afterfragmentadd", GL.GetError());

            /* create the pipeline program */
            var compile = Game.ShaderHandler.CompileProgram();

            // Console.Write(compile + " ");
            Game.LogHandler.LogGLError("createprog_aftercompile", GL.GetError());

            return compile;
        }

        /* actions to do on window (game) startup */
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            /* draw both the front and back of the polygon in fill mode,
             * as opposed to only the outlines (line) or the vertices (point) */
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            /* enable the option that makes opengl check the depth (z-azis)
             * of every pixel it draws, it order to not draw things that 
             * are supposed to be far-away on top of things that are supposed
             * to be in the front of the screen */
            /* we are going to use a render queue in order to draw each
             * sprite in the correct order from background to foreground,
             * so we don't need to enable depthtest or alphatest anymore */
            // GL.Enable(EnableCap.DepthTest);
            // GL.Enable(EnableCap.AlphaTest);
            // GL.AlphaFunc(AlphaFunction.Greater, 0);

            /* enable blending, which is notably used to handle transparency */
            GL.Enable(EnableCap.Blend);
            /* this defines what the blending function does, the standard values
             * for regular transparency are (SrcAlpha, OneMinusSrcAlpha) */
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            
            ShaderProgramId = CreateProgram();

            Game.LogHandler.LogGLError("load_afterprogcreate", GL.GetError());
            
            /* we created a "program" earlier with CreateProgram()
             * which describes the contents of the pipeline and what
             * it will do to incoming input */
            GL.UseProgram(ShaderProgramId); // we choose to use our program in our pipeline, we don't need to do it on every frame???
            Game.LogHandler.LogGLError("afterproguse", GL.GetError());
            
            /* we give the shader an orthographic projection matrix,
             * whose characteristics are that things don't get smaller when they're farther away,
             * and distances are in pixels rather than in fractions of the screen's size.
             * the matrix is sent to the vertex shader who will apply it on the vertices
             * (hence why it needs to be called on every frame, after program use but before rendering) */
            /* we get the location of the uniform that will hold our matrix in the vertex shader */
            var u = GL.GetUniformLocation(ShaderProgramId, "projection");
            Game.LogHandler.LogGLError("afteruniform", GL.GetError());
            /* we send the matrix to the shader */
            GL.UniformMatrix4(u, false, ref OrthographicProjectionMatrix);
            Game.LogHandler.LogGLError("afterprojectionmatrix", GL.GetError());

            Game.RenderHandler.Load(ShaderProgramId);

            Game.SceneHandler.LoadScene(Config.StartScene);
        }

        /* actions to do on window resize */
        protected override void OnResize(EventArgs e)
        {
            /* does this line really need to be called on every frame,
             * or only on window resize/other similar situations? */
            GL.Viewport(0, 0, Config.WindowWidth, Config.WindowHeight);
            // TODO: find a way to have the game work well with window resizes (maybe prevent resizing?)
        }

        /* adding keyboard handling logic, we probably don't want to add anything else in here */
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            Game.KeyboardHandler.AddTriggerEvent(e.Key);
        }

        /* adding keyboard handling logic, we probably don't want to add anything else in here */
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            Game.KeyboardHandler.AddReleaseEvent(e.Key);
        }

        /* update logic 
         * runs X times every second, where X is the value given as parameter for the MainWindow.Run method */
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            BackgroundColor.H = (BackgroundColor.H + 0.002f) % 1; // changing background color on every frame for testing purposes

            // do the update logic here
            Game.SceneHandler.Update();

            Game.KeyboardHandler.ResetKeys(); // at the end of the update frame, we reset all keyboard trigger/release events
        }

        /* render logic
         * refreshes the graphics displayed in MainWindow
         * if Vsync is off: this method gets called as often as possible
         * if Vsync is on (default): is locked to the monitor's refresh rate (usually 60 or 120 FPS) */
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            Game.LogHandler.LogGLError("afterbase", GL.GetError());

            /* get the time elapsed since last frame,
             * it's a surprise tool that will help us later */
            totalTimeElapsed += e.Time;

            // Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}"; // for debug purposes

            // background color
            Color4 backColor = BackgroundColor.ToRGBA();

            Game.LogHandler.LogGLError("afterbgset", GL.GetError());

            // applies the background color
            GL.ClearColor(backColor);

            Game.LogHandler.LogGLError("aftercolorclear", GL.GetError());

            // clear the color buffer and the depth buffer (why? who knows lmao)
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Game.LogHandler.LogGLError("aftermaskclear", GL.GetError());

            /* the other entry point of the pipeline: attributes
             * those are sent as input to the vertex shader which needs an "in" attribute with
             * the corresponding index (set with "layout (location = n)" in the vertex shader file)
             * only the vertex shader can take in attributes from the outside, if you want to
             * send information to the next shaders in the pipeline you need to make them go through
             * the vertex shader and do some 1:1 mapping like "fs_color = color" or whatever */
            // GL.VertexAttrib4(0, position);

            /* vertex specification is the entry point of the pipeline
             * we declare things we want to draw, and which kind */
            Game.RenderHandler.Render(ShaderProgramId);

            /* we can apply some modifications to our primitives */
            // GL.PointSize(10); // change point size so that it's bigger than 1 pixel and we can see it
            Game.LogHandler.LogGLError("afterrender", GL.GetError());

            /* the scene is being drawn on the "back" buffer, which is hidden behind the "front" buffer
             * after we're finished drawing the scene on the back buffer, we use SwapBuffers()
             * so that the back buffer replaces the front buffer and displays the scene to the player
             * then the buffer previously in front becomes the back buffer and can be drawn on for the next frame
             * so this call is basically saying "we're done with drawing for this frame,
             * let's display the result to the user" */
            SwapBuffers();

            Game.LogHandler.LogGLError("afterswap", GL.GetError());

            Game.SceneHandler.Render();
        }

        /* actions to do on window close */
        private void OnClosed(object sender, EventArgs eventArgs)
        {
            Exit();
        }

        /* application exit */
        public override void Exit()
        {
            Game.SceneHandler.UnloadScene();

            // save the planet, free your memory
            /* delete the vertex arrays we were using */
            Game.RenderHandler.Dispose();
            /* delete the program we were using */
            GL.DeleteProgram(ShaderProgramId);

            /* exit application */
            base.Exit();
        }
    }
}