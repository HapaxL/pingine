using System;
using OpenTK;

namespace pingine.Game.State.Entities
{
    public class TestEntity : Entity
    {
        public Vector2 vector;

        public TestEntity() : base(
            Game.RNG.Next(Game.Window.Size.Width),
            Game.RNG.Next(Game.Window.Size.Height),
            Game.RNG.Next(100) + 10,
            "cube",
            0,
            0)
        {
            vector = new Vector2((float) Game.RNG.NextDouble() * 2 - 1, (float) Game.RNG.NextDouble() * 2 - 1);
        }

        public override void Update()
        {
            if (Position.X <= 0)
            {
                vector.X = Math.Abs(vector.X);
            }

            if (Position.X + Size.X >= Config.WindowWidth)
            {
                vector.X = -Math.Abs(vector.X);
            }

            if (Position.Y <= 0)
            {
                vector.Y = Math.Abs(vector.Y);
            }

            if (Position.Y + Size.Y >= Config.WindowHeight)
            {
                vector.Y = -Math.Abs(vector.Y);
            }

            Position = Vector2.Add(Position, vector);

            if (Game.RNG.Next(10) == 0)
            {
                Depth = Game.RNG.Next(100) + 10;
            }
        }
    }
}
