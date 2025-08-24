using System;
using Cysharp.Threading.Tasks;
using Payosky.Architecture.EventManager;
using Payosky.Architecture.SceneManager;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.Runtime.GameEvents;

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
            SpawnPlayers().Forget();
        }

        public void OnUnloaded()
        {
        }

        public async UniTaskVoid SpawnPlayers()
        {
            await UniTask.WaitForSeconds(3f);
            if (ServiceLocator.TryGet(out GameEventService gameEventService))
            {
                gameEventService.Dispatch(new SpawnPlayersGameEvent());
            }
        }
    }
}