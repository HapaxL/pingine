/*

using OpenTK;
using pingine.Game.Graphics;
using OpenTK.Graphics;

using TexId = System.Int32;

namespace pingine.Game.State
{
    public class Tile : Entity
    {
        public override Vector2 Position { get; protected set; }
        public override float Depth { get; protected set; }
        public override RenderData[] RenderData { get; protected set; }

        public Tile(
            int x,
            int y,
            float depth,
            string spriteSetName,
            int sprite)
            : base(x, y, depth, spriteSetName)
        {
            var spriteBox = SpriteSet.SpriteBoxes[sprite];
            RenderData[0].Update(Position.X, Position.Y, spriteBox.Left, spriteBox.Top); // top-left
            RenderData[1].Update(Position.X + Size.X, Position.Y, spriteBox.Right, spriteBox.Top); // top-right
            RenderData[2].Update(Position.X + Size.X, Position.Y + Size.Y, spriteBox.Right, spriteBox.Bottom); // bottom-right
            RenderData[3].Update(Position.X, Position.Y + Size.Y, spriteBox.Left, spriteBox.Bottom); // bottom-left
        }

        public override void Update() { }
    }
}
*/