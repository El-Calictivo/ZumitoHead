using System;
using Payosky.Architecture.SceneManager;

namespace ZumitoGame.Scenes
{
    [Serializable]
    public class SandoxScene : IGameScene
    {
        public const string SceneName = "Sandbox";

        public string GetName()
        {
            return SceneName;
        }

        public void OnLoaded()
        {
        }

        public void OnUnloaded()
        {
        }
    }
}