namespace pingine.Game.Graphics
{
    public struct SpriteBox
    {
        // public 
        /*
        public Vector2 TopLeftCoord { get; private set; }
        public Vector2 TopRightCoord { get; private set; }
        public Vector2 BottomLeftCoord { get; private set; }
        public Vector2 BottomRightCoord { get; private set; }

        public SpriteBox(int x, int y, int w, int h)
        {
            TopLeftCoord = new Vector2(x, y);
            TopRightCoord = new Vector2(x + w, y);
            BottomLeftCoord = new Vector2(x, y + h);
            BottomRightCoord = new Vector2(x + w, y + h);
        } */

        public int Left { get; }
        public int Top { get; }
        public int Right { get; }
        public int Bottom { get; }

        public SpriteBox(int l, int t, int r, int b)
        {
            Left = l;
            Top = t;
            Right = r;
            Bottom = b;
        }
    }
}
