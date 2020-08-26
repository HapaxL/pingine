using OpenTK;

namespace pingine.Game.State
{
    public static partial class EntityBehaviours
    {
        public static void TestMoving(Entity entity)
        {
            entity.Position = Vector2.Add(entity.Position, new Vector2(1, 1));
        }
    }
}
