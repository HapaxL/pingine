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
            public float depth;
            public Color4 color;
            public Vector2 texCoordinates;
            // add color?

            public SpritePoint(Vector2 position, float depth, Vector2 texCoordinates)
            {
                this.position = position;
                this.depth = depth;
                this.color = new Color4(1.0f, 0f, 0f, 1.0f);
                this.texCoordinates = texCoordinates;
            }

            //public Vector4[] ToVertices()
            //{
            //    return new Vector4[] { position, color };
            //}
        }

        public System.Drawing.Bitmap Bitmap { get; private set; }
        public SpritePoint[] Points { get; private set; }

        public Sprite(System.Drawing.Bitmap bitmap, Vector2 position, Vector2 size, float depth)
        {
            Bitmap = bitmap;
            Points = new SpritePoint[4];
            //Points[0] = new SpritePoint(position,                                                   depth, new Vector2(0.0f, 1.0f)); // bottom-left
            //Points[1] = new SpritePoint(new Vector2(position.X + size.X,    position.Y),            depth, new Vector2(1.0f, 1.0f)); // bottom-right
            //Points[2] = new SpritePoint(new Vector2(position.X + size.X,    position.Y + size.Y),   depth, new Vector2(1.0f, 0.0f)); // top-right
            //Points[3] = new SpritePoint(new Vector2(position.X,             position.Y + size.Y),   depth, new Vector2(0.0f, 0.0f)); // top-left
            Points[0] = new SpritePoint(position,                                                   depth, new Vector2(0.0f, 0.0f)); // top-left
            Points[1] = new SpritePoint(new Vector2(position.X + size.X,    position.Y),            depth, new Vector2(1.0f, 0.0f)); // top-right
            Points[2] = new SpritePoint(new Vector2(position.X + size.X,    position.Y + size.Y),   depth, new Vector2(1.0f, 1.0f)); // bottom-right
            Points[3] = new SpritePoint(new Vector2(position.X,             position.Y + size.Y),   depth, new Vector2(0.0f, 1.0f)); // bottom-left
        }
    }
}
