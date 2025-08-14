using System;
using Payosky.Architecture.SceneManager;

namespace ZumitoGame.Scenes
{
    [Serializable]
    public class MainMenuScene : IGameScene
    {
        public const string SceneName = "MainMenu";

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