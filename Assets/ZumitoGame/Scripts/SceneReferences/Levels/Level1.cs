using System;
using Cysharp.Threading.Tasks;
using Payosky.Architecture.EventManager;
using Payosky.Architecture.SceneManager;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.Runtime.GameEvents;

namespace ZumitoGame.Scenes.Levels
{
    [Serializable]
    public class Level1 : BaseGameLevelScene<Level1>, IGameLevelScene
    {
        public void OnLoaded()
        {
            SpawnPlayers().Forget();
        }

        public void OnUnloaded()
        {
        }

        public async UniTaskVoid SpawnPlayers()
        {
            await UniTask.WaitForSeconds(1f);
            if (ServiceLocator.TryGet(out GameEventService gameEventService))
            {
                gameEventService.Dispatch(new SpawnPlayersGameEvent());
            }
        }
    }
}