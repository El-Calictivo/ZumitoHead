using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Payosky.Architecture.Services;
using Payosky.Utilities.Logging;
using UnityEngine;

namespace Payosky.Architecture.SceneManager
{
    public class SceneManagerService : IGameService
    {
        public IGameScene StartScene { get; private set; }
        public Dictionary<string, IGameScene> LoadedScenes { get; } = new();

        public void SetStartScene(IGameScene scene)
        {
            if (StartScene == null)
            {
                StartScene = scene;
                OnSceneLoaded(scene);
            }
        }

        public void OnSceneLoaded(IGameScene scene)
        {
            Debug.Log($"{this.GetLoggingTag()} Scene {scene.Name.GetLoggingTag(false)} loaded");
            if (LoadedScenes.TryAdd(scene.Name, scene))
            {
                scene.OnLoaded();
            }
        }

        public void OnSceneUnloaded(IGameScene scene)
        {
            Debug.Log($"{this.GetLoggingTag()} Scene {scene.Name.GetLoggingTag(false)} unloaded");
            if (LoadedScenes.Remove(scene.Name))
            {
                scene.OnUnloaded();
                if (StartScene?.Name == scene.Name)
                {
                    StartScene = null;
                }
            }
        }

        public async UniTask LoadScene(IGameScene scene, LoadSceneParameters parameters = default)
        {
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene.Name, parameters).ToUniTask();
            OnSceneLoaded(scene);
        }

        public async UniTask UnloadScene(IGameScene scene)
        {
            await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene.Name).ToUniTask();
            OnSceneUnloaded(scene);
        }
    }
}