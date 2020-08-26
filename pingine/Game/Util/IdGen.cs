namespace pingine.Game.Util
{
    public class IdGen
    {
        private ulong currentId;

        public IdGen()
        {
            currentId = 0;
        }

        public ulong Get()
        {
            return currentId++;
        }
    }
}
