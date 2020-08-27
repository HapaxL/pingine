using OpenTK;
using pingine.Game.Graphics;
using OpenTK.Graphics;

using TexId = System.Int32;

namespace pingine.Game.State
{
    public struct RenderData
    {
        public float x;
        public float y;
        // public Color4 color;
        public int texCoordX;
        public int texCoordY;

        public RenderData(float x, float y, int texCoordX, int texCoordY)
        {
            this.x = x;
            this.y = y;
            // this.color = new Color4(1f, 0f, 0f, 1f);
            this.texCoordX = texCoordX;
            this.texCoordY = texCoordY;
        }

        public void Update(float x, float y, int texCoordX, int texCoordY)
        {
            this.x = x;
            this.y = y;
            // this.color = new Color4(1f, 0f, 0f, 1f);
            this.texCoordX = texCoordX;
            this.texCoordY = texCoordY;
        }

        //public Vector4[] ToVertices()
        //{
        //    return new Vector4[] { position, color };
        //}
    }

    public abstract class Entity
    {
        public ulong Id { get; }

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                DataHasChanged = true;
            }
        }

        // should be equal to spriteset size by default
        // when we want to zoom in/out, do we set this? or do we do something else with the vertices later on? TODO
        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            /* set
            {
                size = value;
                DataHasChanged = true;
            } */
        }

        // render handler needs to use the old depth value to find the old position of the object in the tree
        public float OldDepth { get; private set; }

        /* whenever depth changes, render handler has to update the renderobject tree, so we mark
         * the object in the handler so that when it gets called for a render, it only updates the
         * object's render and updates the position of the object in the tree if it has changed */
        private float depth;
        public float Depth
        {
            get { return depth; }
            set
            {
                depth = value;
                DataHasChanged = true;
                Game.RenderHandler.MustUpdateRender(this);
            }
        }

        // public string SpriteSetName { get; }
        public SpriteSet SpriteSet { get; }

        private int currentSprite;
        public int CurrentSprite
        {
            get { return currentSprite; }
            set
            {
                currentSprite = value;
                DataHasChanged = true;
            }
        }

        public int CurrentAnimation { get; protected set; }

        public TexId? TexId { get; set; }
        private RenderData[] RenderData { get; set; }
        private bool DataHasChanged { get; set; }

        public Entity(
            int baseX,
            int baseY,
            float baseDepth,
            string spriteSetName,
            int baseSprite,
            int baseAnimation)
        {
            Id = Game.IdGen.Get();
            Position = new Vector2(baseX, baseY);
            Depth = baseDepth;
            OldDepth = Depth;
            SpriteSet = Game.SpriteSets[spriteSetName];
            CurrentSprite = baseSprite;
            CurrentAnimation = baseAnimation;
            size = new Vector2(SpriteSet.SpriteWidth, SpriteSet.SpriteHeight);
            TexId = null;
            RenderData = new RenderData[4];
            DataHasChanged = true; // needs to be true so that LastData gets generated once before first render                                            // depth
        }

        public void Load()
        {
            TexId = Game.ResourceHandler.LoadedBitmaps[SpriteSet.BitmapName].TexId;
        }

        public void Unload()
        {
            TexId = null;
        }

        // public abstract Sprite[] InitSprites();

        public abstract void Update();

        public void UpdateDepth()
        {
            OldDepth = Depth;
        }

        /* private readonly Vector2[] defaultTexCoordinates = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1/3f, 0f),
            new Vector2(1/3f, 1/2f),
            new Vector2(0f, 1/2f),
        }; */

        public RenderData[] GetRenderData()
        {
            if (DataHasChanged)
            {
                var spriteBox = SpriteSet.SpriteBoxes[CurrentSprite];
                RenderData[0].Update(position.X, position.Y, spriteBox.Left, spriteBox.Top); // top-left
                RenderData[1].Update(position.X + size.X, position.Y, spriteBox.Right, spriteBox.Top); // top-right
                RenderData[2].Update(position.X + size.X, position.Y + size.Y, spriteBox.Right, spriteBox.Bottom); // bottom-right
                RenderData[3].Update(position.X, position.Y + size.Y, spriteBox.Left, spriteBox.Bottom); // bottom-left
                DataHasChanged = false;
            }

            return RenderData;
        }
    }
}
