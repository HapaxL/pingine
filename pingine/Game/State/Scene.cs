using System.Collections.Generic;
using System.Linq;

namespace pingine.Game.State
{
    public abstract class Scene
    {
        public Dictionary<ulong, Entity> Entities { get; }
        public HashSet<string> BitmapSet { get; }

        public Scene()
        {
            Entities = InitEntities();
            // SpriteSets = GetSpriteSets();
            BitmapSet = Entities.Select(e => e.Value.SpriteSet.BitmapName).ToHashSet();
        }

        //public HashSet<string> GetSpriteSets()
        //{
        //    return Entities.Select(e => e.Value.SpriteSet).ToHashSet();
        //}

        public abstract Dictionary<ulong, Entity> InitEntities();

        public virtual void Load()
        {
            foreach (var e in Entities)
            {
                e.Value.Load();
                Game.RenderHandler.AddEntity(e.Value);
            }
        }

        public virtual void Unload()
        {
            foreach (var e in Entities)
            {
                Game.RenderHandler.RemoveEntity(e.Value);
                e.Value.Unload();
            }
        }

        public abstract void ProcessKeys();

        public virtual void Update()
        {
            foreach (var entity in Entities.Values)
            {
                entity.Update();
            }
        }

    //    public void Render()
    //    {
    //        foreach (var entity in Entities.Values)
    //        {
    //            entity.Render();
    //        }
    //    }
    }
}
