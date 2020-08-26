namespace pingine.Game.Graphics
{
    public class SpriteSet
    {
        public string BitmapName { get; }
        public int SpriteWidth { get; }
        public int SpriteHeight { get; }
        public SpriteBox[] SpriteBoxes { get; }

        // TODO make load/unload methods for spriteboxes etc.

        public SpriteSet(string bitmapName, int topLeftCornerX, int topLeftCornerY, int nbColumns, int nbRows, int spriteWidth, int spriteHeight)
        {
            BitmapName = bitmapName;
            var nbSprites = nbColumns * nbRows;
            SpriteWidth = spriteWidth;
            SpriteHeight = spriteHeight;
            SpriteBoxes = new SpriteBox[nbSprites];
            var i = 0;
            var bottomRightCornerX = topLeftCornerX + (nbColumns * spriteWidth);
            var bottomRightCornerY = topLeftCornerY + (nbRows * spriteHeight);
            for (int y = topLeftCornerY; y < bottomRightCornerY; y += spriteHeight)
            {
                for (int x = topLeftCornerX; x < bottomRightCornerX; x += spriteWidth)
                {
                    var box = new SpriteBox(x, y, x + spriteWidth, y + spriteHeight);
                    SpriteBoxes[i] = box;
                    i++;
                }
            }
        }

        public SpriteSet(string bitmapName, int nbColumns, int nbRows, int spriteWidth, int spriteHeight)
            : this(bitmapName, 0, 0, nbColumns, nbRows, spriteWidth, spriteHeight) { }

        public SpriteSet(string bitmapName, int spriteWidth, int spriteHeight)
            : this(bitmapName, 0, 0, 1, 1, spriteWidth, spriteHeight) { }
    }

}
