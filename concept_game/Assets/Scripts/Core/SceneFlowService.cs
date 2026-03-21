using MossHarbor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MossHarbor.Core
{
    public sealed class SceneFlowService
    {
        public const string BootSceneName = "Boot";
        public const string MainMenuSceneName = "MainMenu";
        public const string HubSceneName = "Hub";
        public const string ExpeditionSceneName = "Expedition_Runtime";
        public const string ResultsSceneName = "Results";

        public const float DefaultFadeOutDuration = 0.4f;
        public const float DefaultFadeInDuration = 0.35f;

        private readonly GameStateService _gameStateService;
        private readonly SaveService _saveService;
        private bool _isTransitioning;

        public string CurrentSceneName => SceneManager.GetActiveScene().name;
        public bool IsTransitioning => _isTransitioning;

        public event System.Action<string> OnSceneTransitionStarted;
        public event System.Action<string> OnSceneTransitionCompleted;

        public SceneFlowService(GameStateService gameStateService, SaveService saveService)
        {
            _gameStateService = gameStateService;
            _saveService = saveService;
        }

        public void LoadHub()
        {
            _saveService.SetScene(HubSceneName);
            _gameStateService.SetState(GameFlowState.Hub);
            LoadWithFade(HubSceneName);
        }

        public void LoadExpedition()
        {
            _saveService.SetScene(ExpeditionSceneName);
            _gameStateService.SetState(GameFlowState.Expedition);
            LoadWithFade(ExpeditionSceneName);
        }

        public void LoadResults()
        {
            _saveService.SetScene(ResultsSceneName);
            _gameStateService.SetState(GameFlowState.Results);
            LoadWithFade(ResultsSceneName);
        }

        private void LoadWithFade(string sceneName)
        {
            if (_isTransitioning)
                return;

            OnSceneTransitionStarted?.Invoke(sceneName);

            var fade = SceneFadeController.Instance;
            if (fade != null && !fade.IsFading)
            {
                _isTransitioning = true;
                fade.FadeOut(DefaultFadeOutDuration, () =>
                {
                    var op = TryLoadSceneAsync(sceneName);
                    if (op != null)
                    {
                        op.completed += _ =>
                        {
                            fade.FadeIn(DefaultFadeInDuration, () =>
                            {
                                _isTransitioning = false;
                                OnSceneTransitionCompleted?.Invoke(sceneName);
                            });
                        };
                    }
                    else
                    {
                        fade.FadeIn(DefaultFadeInDuration, () =>
                        {
                            _isTransitioning = false;
                            OnSceneTransitionCompleted?.Invoke(sceneName);
                        });
                    }
                });
            }
            else
            {
                TryLoadScene(sceneName);
                OnSceneTransitionCompleted?.Invoke(sceneName);
            }
        }

        private static AsyncOperation TryLoadSceneAsync(string sceneName)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
                return null;

            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                if (path.EndsWith($"{sceneName}.unity"))
                    return SceneManager.LoadSceneAsync(sceneName);
            }

            return null;
        }

        private static void TryLoadScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
                return;

            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                if (path.EndsWith($"{sceneName}.unity"))
                {
                    SceneManager.LoadScene(sceneName);
                    return;
                }
            }
        }
    }
}
