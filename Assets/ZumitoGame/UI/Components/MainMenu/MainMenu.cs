using System;
using Cysharp.Threading.Tasks;
using Payosky.Architecture.SceneManager;
using Payosky.Architecture.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using ZumitoGame.Scenes;

namespace ZumitoGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenu : MonoBehaviour
    {
        private VisualElement _root;

        private Button _playButton;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _playButton = _root.Q<Button>("PlayButton");
        }

        private void PlayButtonClicked()
        {
            _playButton.clicked -= PlayButtonClicked;
            if (ServiceLocator.TryGet(out SceneManagerService sceneManagerService))
            {
                sceneManagerService.UnloadScene(new Scenes.MainMenu()).Forget();
                sceneManagerService.LoadScene(new LevelSelection(), new LoadSceneParameters(LoadSceneMode.Additive)).Forget();
            }
        }

        private void OnEnable()
        {
            _playButton.clicked += PlayButtonClicked;
        }

        private void OnDisable()
        {
            _playButton.clicked -= PlayButtonClicked;
        }
    }
}