using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace pingine.Main.Graphics
{
    public struct Sprite
    {
        public struct SpritePoint
        {
            public Vector2 position;
            public Color4 color;
            public Vector2 texCoordinates;

            public SpritePoint(Vector2 position, Vector2 texCoordinates)
            {
                this.position = position;
                this.color = new Color4(1f, 0f, 0f, 1f);
                this.texCoordinates = texCoordinates;
            }

            //public Vector4[] ToVertices()
            //{
            //    return new Vector4[] { position, color };
            //}
        }

        public System.Drawing.Bitmap Bitmap { get; private set; }
        public SpritePoint[] Points { get; private set; }
        public int Depth { get; private set; }

        public Sprite(System.Drawing.Bitmap bitmap, Vector2 position, Vector2 size, int depth)
        {
            Bitmap = bitmap;
            Depth = depth;
            Points = new SpritePoint[4];
            Points[0] = new SpritePoint(position,                                                   new Vector2(0f, 0f)); // top-left
            Points[1] = new SpritePoint(new Vector2(position.X + size.X,    position.Y),            new Vector2(1f, 0f)); // top-right
            Points[2] = new SpritePoint(new Vector2(position.X + size.X,    position.Y + size.Y),   new Vector2(1f, 1f)); // bottom-right
            Points[3] = new SpritePoint(new Vector2(position.X,             position.Y + size.Y),   new Vector2(0f, 1f)); // bottom-left
        }
    }
}
