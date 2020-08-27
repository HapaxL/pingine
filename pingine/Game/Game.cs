using pingine.Game.Graphics;
using pingine.Game.Handlers;
using pingine.Game.State;
using pingine.Game.Util;
using System;
using System.Collections.Generic;

namespace pingine.Game
{
    public class Game
    {
        public static Random RNG { get; private set; }
        public static IdGen IdGen { get; private set; }

        public static LogHandler LogHandler { get; private set; }
        /* (used for tutorial purposes) */
        public static ShaderHandler ShaderHandler { get; private set; }
        public static KeyboardHandler KeyboardHandler { get; private set; }
        public static ResourceHandler ResourceHandler { get; private set; }
        /* (used for tutorial purposes) */
        public static RenderHandler RenderHandler { get; private set; }
        public static SceneHandler SceneHandler { get; private set; }

        public static Camera Camera { get; private set; }

        public static Dictionary<string, SpriteSet> SpriteSets { get; private set; }

        public static MainWindow Window { get; private set; }

        [STAThread]
        static void Main()
        {
            RNG = new Random();
            IdGen = new IdGen();

            LogHandler = new LogHandler();
            ShaderHandler = new ShaderHandler();
            KeyboardHandler = new KeyboardHandler(false); // we don't want repeat enabled for a video game (except in menus or when writing something)
            ResourceHandler = new ResourceHandler();
            switch (Config.TextureDisplayMode)
            {
                case TextureDisplayMode.Smooth:
                    RenderHandler = new RenderHandlerSmooth();
                    break;
                default:
                    RenderHandler = new RenderHandlerPixelPerfect();
                    break;
            }
            SceneHandler = new SceneHandler();

            Camera = new Camera(50, 100);

            SpriteSets = new Dictionary<string, SpriteSet>
            {
                { "cube", new SpriteSet("cube.png", 79, 71) },
                { "tileset1", new SpriteSet("tileset.png", 3, 2, 16, 16) },
            }; // TODO do an enum and a GetSpriteSet like for Scenes?

            Window = new MainWindow();
            Window.Run(Config.UPS); // option sets the amount of times OnUpdateFrame(e) is called every second
        }
    }
}
