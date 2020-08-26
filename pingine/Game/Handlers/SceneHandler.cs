using System.Diagnostics;
using pingine.Game.State;
using pingine.Game.State.Scenes;

namespace pingine.Game.Handlers
{
    public enum SceneId
    {
        Test,
    }

    public class SceneHandler
    {
        private readonly Stopwatch FpsWatch;
        private int ups;
        private int fps;
        
        public Scene CurrentScene { get; private set; }

        public SceneHandler()
        {
            FpsWatch = new Stopwatch();
            ups = 0;
            fps = 0;
            FpsWatch.Start();
        }

        private Scene GetScene(SceneId scene)
        {
            switch (scene)
            {
                case SceneId.Test:
                    return new TestScene();
                default:
                    throw new Util.OhNoException("um that's not a valid scene ID");
            }
        }

        public void LoadScene(SceneId startSceneId)
        {
            CurrentScene = GetScene(startSceneId);
            Game.ResourceHandler.Load(CurrentScene.BitmapSet);
            CurrentScene.Load();
        }

        public void UnloadScene()
        {
            CurrentScene.Unload();
            Game.ResourceHandler.Unload(CurrentScene.BitmapSet);
        }

        public void ChangeScene(SceneId newSceneId)
        {
            CurrentScene.Unload();
            var newScene = GetScene(newSceneId);
            Game.ResourceHandler.UnloadAndLoad(CurrentScene.BitmapSet, newScene.BitmapSet);
            newScene.Load();
            CurrentScene = newScene;
        }

        public void Update()
        {
            CurrentScene.ProcessKeys(); // temporary (?) function for testing purposes?
            CurrentScene.Update();

            ups++;

            if (FpsWatch.ElapsedMilliseconds >= 1000)
            {
                Game.Window.Title = $"UPS: {ups}, FPS: {fps}";
                ups = 0;
                fps = 0;
                FpsWatch.Restart();
            }
        }

        public void Render()
        {
            fps++;
        }
    }
}
