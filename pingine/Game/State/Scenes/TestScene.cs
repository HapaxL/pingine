using System.Collections.Generic;
using OpenTK.Input;
using pingine.Game.State.Entities;

namespace pingine.Game.State.Scenes
{
    public class TestScene : Scene
    {
        private Dictionary<ulong, TestEntity> movingEntities;

        public TestScene() : base()
        {
            // var example2 = new System.Drawing.Bitmap(@"E:\Code\Projects\pingine\pingine\Resources\Graphics\bitch_of_an_earth.png");
            // new RenderObject(example2, new Vector2(180, 150), new Vector2(example2.Size.Width, example2.Size.Height), 1),
        }

        public override Dictionary<ulong, Entity> InitEntities()
        {
            var entities = new Dictionary<ulong, Entity>();
            movingEntities = new Dictionary<ulong, TestEntity>();

            for (int i = 0; i < 750; i++)
            {
                var cube = new TestEntity();
                entities.Add(cube.Id, cube);
                movingEntities.Add(cube.Id, cube);
            }

            for (int j = 0; j < 45; j++)
            {
                for (int i = 0; i < 80; i++)
                {
                    var tile = new TestTile(i * 16, j * 16, Game.RNG.Next(6));
                    entities.Add(tile.Id, tile);
                }
            }

            return entities;
        }

        public override void ProcessKeys()
        {
            /* for keyboard key repeat tests */
            /*
            GameWindow.Title = $"waiting for a press on E (R enables repeat, T disables it)";

            if (keyboard.IsHeld(Key.E))
            {
                GameWindow.Title += $": E is being held";
            }

            if (keyboard.IsTriggered(Key.E))
            {
                GameWindow.Title += $": E was triggered";
            }

            if (keyboard.IsReleased(Key.E))
            {
                GameWindow.Title += $": and E was finally released!";
            }

            if (keyboard.IsTriggered(Key.R))
            {
                keyboard.EnableRepeat(true);
            }

            if (keyboard.IsTriggered(Key.T))
            {
                keyboard.EnableRepeat(false);
            }
            */

            /* temporary: use Escape to instantly quit the game */
            if (Game.KeyboardHandler.IsTriggered(Key.Escape))
            {
                Game.Window.Close(); // TODO call a Game.Stop() method that calls Window.Close() instead?
            }
        }
        
        public override void Update()
        {
            base.Update();
            
            foreach (var cube in movingEntities)
            {
                foreach (var cube2 in movingEntities)
                {
                    if ((cube.Key != cube2.Key)
                        && (cube.Value.Position.X <= (cube2.Value.Position.X + 1))
                        && (cube.Value.Position.X >= (cube2.Value.Position.X - 1))
                        && (cube.Value.Position.Y <= (cube2.Value.Position.Y + 1))
                        && (cube.Value.Position.Y >= (cube2.Value.Position.Y - 1)))
                    {
                        cube.Value.vector = -cube.Value.vector;
                    }
                }
            }
        }
    }
}
