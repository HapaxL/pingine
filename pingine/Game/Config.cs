﻿using pingine.Game.Handlers;

namespace pingine.Game
{
    public enum TextureDisplayMode
    {
        PixelPerfect,
        Smooth,
    }

    public static class Config
    {
        public static string GameName = "Pingine Demo";

        public static int WindowWidth = 1280;
        public static int WindowHeight = 720;

        /* updates (to the game logic) per second
         * we want a fixed amount so that the game never increases/decreases in speed */
        public static int UPS = 60;

        public static string ResourceFolder = @"E:\Code\Projects\pingine\pingine\Resources\";

        public static SceneId StartScene = SceneId.Test;

        public static TextureDisplayMode TextureDisplayMode = TextureDisplayMode.PixelPerfect;
    }
}
