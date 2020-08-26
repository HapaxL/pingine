using System.Collections.Generic;
using System.Drawing;

using TexId = System.Int32;

namespace pingine.Game.Handlers
{
    public struct BitmapInfo
    {
        public string Name { get; set; }
        public TexId TexId { get; set; }
        public int BitmapWidth { get; set; }
        public int BitmapHeight { get; set; }
    }

    public class ResourceHandler
    {
        public Dictionary<string, BitmapInfo> LoadedBitmaps { get; }

        public ResourceHandler()
        {
            LoadedBitmaps = new Dictionary<string, BitmapInfo>();
        }

        public Bitmap GetBitmap(string fileName)
        {
            return new Bitmap(Config.ResourceFolder + @"\Graphics\" + fileName);
        }

        public void Load(HashSet<string> toLoad)
        {
            toLoad.ExceptWith(LoadedBitmaps.Keys);
            foreach (var name in toLoad)
            {
                var bitmap = GetBitmap(name);
                var texId = Game.RenderHandler.LoadBitmap(bitmap);
                var bitmapInfo = new BitmapInfo()
                {
                    Name = name,
                    TexId = texId,
                    BitmapWidth = bitmap.Width,
                    BitmapHeight = bitmap.Height,
                };
                LoadedBitmaps.Add(name, bitmapInfo);
            }
        }

        public void Unload(HashSet<string> toUnload)
        {
            foreach (var name in toUnload)
            {
                Game.RenderHandler.UnloadBitmap(LoadedBitmaps[name].TexId);
                LoadedBitmaps.Remove(name);
            }
        }

        public void UnloadAndLoad(HashSet<string> toUnload, HashSet<string> toLoad)
        {
            toUnload.ExceptWith(toLoad);
            Unload(toUnload);
            Load(toLoad);
        }
    }
}
