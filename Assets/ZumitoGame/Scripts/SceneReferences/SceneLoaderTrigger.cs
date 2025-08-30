using Cysharp.Threading.Tasks;
using Payosky.Architecture.SceneManager;
using Payosky.Architecture.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZumitoGame.Scenes;

namespace ZumitoGame
{
    public class SceneLoaderTrigger : MonoBehaviour
    {
        [SubclassSelector] [SerializeReference]
        private IGameLevelScene gameLevelScene;

        public void Load()
        {
            ServiceLocator.Get<SceneManagerService>().UnloadScene(new LevelSelection()).Forget();
            ServiceLocator.Get<SceneManagerService>().LoadScene(gameLevelScene, new LoadSceneParameters() { loadSceneMode = LoadSceneMode.Additive }).Forget();
        }
    }
}