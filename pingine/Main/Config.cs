using System;
using System.Collections.Generic;
using System.Text;

namespace pingine.Main
{
    public static class Config
    {
        public static string GameName = "Ghost Corridor";

        public static int WindowWidth = 1280;
        public static int WindowHeight = 720;

        /* updates (to the game logic) per second
         * we want a fixed amount so that the game never increases/decreases in speed */
        public static int UPS = 60; 
    }
}
