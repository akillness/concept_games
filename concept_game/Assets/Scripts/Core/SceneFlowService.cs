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

        private readonly GameStateService _gameStateService;
        private readonly SaveService _saveService;

        public SceneFlowService(GameStateService gameStateService, SaveService saveService)
        {
            _gameStateService = gameStateService;
            _saveService = saveService;
        }

        public void LoadHub()
        {
            _saveService.SetScene(HubSceneName);
            _gameStateService.SetState(GameFlowState.Hub);
            TryLoadScene(HubSceneName);
        }

        public void LoadExpedition()
        {
            _saveService.SetScene(ExpeditionSceneName);
            _gameStateService.SetState(GameFlowState.Expedition);
            TryLoadScene(ExpeditionSceneName);
        }

        public void LoadResults()
        {
            _saveService.SetScene(ResultsSceneName);
            _gameStateService.SetState(GameFlowState.Results);
            TryLoadScene(ResultsSceneName);
        }

        private static void TryLoadScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                return;
            }

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
