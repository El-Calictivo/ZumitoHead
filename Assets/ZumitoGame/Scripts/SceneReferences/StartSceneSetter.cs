using Payosky.Architecture.SceneManager;
using Payosky.Architecture.Services;
using UnityEngine;

namespace ZumitoGame
{
    public class StartSceneSetter : MonoBehaviour
    {
        [SerializeReference] [SubclassSelector]
        private IGameScene gameScene;

        private void Start()
        {
            ServiceLocator.Add(new SceneManagerService()).SetStartScene(gameScene);
        }
    }
}