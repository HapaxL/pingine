using OpenTK;
using OpenTK.Graphics;

namespace pingine.Main.Graphics
{
    public struct Vertex
    {
        public static int Size = Vector4.SizeInBytes + (sizeof(float) * 4); // size of struct in bytes

        private Vector4 position;
        private Color4 color;

        public Vertex(Vector4 position, Color4 color)
        {
            this.position = position;
            this.color = color;
        }
    }
}
