using System;
using System.Collections.Generic;
using System.Text;

namespace pingine.Game.State
{
    public class Camera
    {
        public float X { get; set; }
        public float Y { get; set; }

        // public Vector2 Coords { get; set; }

        public Camera(float baseX, float baseY)
        {
            X = baseX;
            Y = baseY;
            //Coords = new Vector2(baseX, baseY);
        }
    }
}
