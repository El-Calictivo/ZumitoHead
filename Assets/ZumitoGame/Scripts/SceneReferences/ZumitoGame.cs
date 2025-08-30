using System;
using Cysharp.Threading.Tasks;
using Payosky.Architecture.SceneManager;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;
using Payosky.CoreMechanics.Inventory;
using UnityEngine.SceneManagement;

namespace ZumitoGame.Scenes
{
    [Serializable]
    public class ZumitoGame : BaseGameScene<ZumitoGame>, IGameScene
    {
        public void OnLoaded()
        {
            ServiceLocator.Add(new GameEntityService());
            ServiceLocator.Add(new InventoryService());
            if (ServiceLocator.TryGet(out SceneManagerService sceneManagerService))
            {
                sceneManagerService.LoadScene(new MainMenu(), new LoadSceneParameters(LoadSceneMode.Additive)).Forget();
            }
        }

        public void OnUnloaded()
        {
        }
    }
}