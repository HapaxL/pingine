namespace pingine.Game.State.Entities
{
    public class TestTile : Entity
    {
        public TestTile(int baseX, int baseY, int depth, int spriteId)
            : base(baseX, baseY, depth, "tileset1", Game.RNG.Next(6), 0)
        { }

        public override void Update()
        {
            // CurrentSprite = Game.RNG.Next(6); // stress/animation test
        }
    }
}
