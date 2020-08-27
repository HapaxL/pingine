/*

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
        
        public abstract Vector2 Position { get; protected set; }

        // should be equal to spriteset size by default
        // when we want to zoom in/out, do we set this? or do we do something else with the vertices later on? TODO
        // private Vector2 size;
        public Vector2 Size
        {
            get; // get { return size; }
            //set
            //{
            //    size = value;
            //    DataHasChanged = true;
            //}
        }

        public abstract float Depth { get; protected set; }
        // public string SpriteSetName { get; }
        public SpriteSet SpriteSet { get; }
        public TexId? TexId { get; protected set; }
        public abstract RenderData[] RenderData { get; protected set; }

        public Entity(
            int baseX,
            int baseY,
            float baseDepth,
            string spriteSetName)
        {
            Id = Game.IdGen.Get();
            Position = new Vector2(baseX, baseY);
            Depth = baseDepth;
            SpriteSet = Game.SpriteSets[spriteSetName];
            Size = new Vector2(SpriteSet.SpriteWidth, SpriteSet.SpriteHeight);
            TexId = null;
            RenderData = new RenderData[4];
        }

        public void Load()
        {
            TexId = Game.ResourceHandler.LoadedBitmaps[SpriteSet.BitmapName].TexId;
        }

        public void Unload()
        {
            TexId = null;
        }

        public abstract void Update();
    }
}
*/