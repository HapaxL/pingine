namespace pingine.Game.State.Entities
{
    public class TestTile : Entity
    {
        public TestTile(int baseX, int baseY, int spriteId)
            : base(baseX, baseY, 0, "tileset1", spriteId, 0)
        { }

        public override void Update()
        {
            // CurrentSprite = Game.RNG.Next(6); // stress/animation test
        }
    }
}
