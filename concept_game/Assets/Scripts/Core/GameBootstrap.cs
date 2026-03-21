using MossHarbor.UI;
using UnityEngine;

namespace MossHarbor.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        public static GameBootstrap Instance { get; private set; }

        public GameStateService GameStateService { get; private set; }
        public SaveService SaveService { get; private set; }
        public SceneFlowService SceneFlowService { get; private set; }

        [SerializeField] private bool dontDestroyOnLoad = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            GameStateService = new GameStateService();
            SaveService = new SaveService();
            SaveService.Initialize();
            SceneFlowService = new SceneFlowService(GameStateService, SaveService);
            EnsureSceneFadeController();
            GameStateService.SetState(GameFlowState.Boot);
        }

        private static void EnsureSceneFadeController()
        {
            if (SceneFadeController.Instance != null)
            {
                return;
            }

            var fadeGo = new GameObject("SceneFadeController");
            fadeGo.AddComponent<SceneFadeController>();
        }

        private void Start()
        {
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentScene == SceneFlowService.BootSceneName || currentScene == "SampleScene")
            {
                var fade = SceneFadeController.Instance;
                if (fade != null)
                {
                    fade.SetOpaque();
                    fade.FadeIn(SceneFlowService.DefaultFadeInDuration);
                }
                GameStateService.SetState(GameFlowState.Hub);
            }
        }
    }
}
