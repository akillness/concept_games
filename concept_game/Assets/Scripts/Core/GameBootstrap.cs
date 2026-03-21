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
            GameStateService.SetState(GameFlowState.Boot);
        }

        private void Start()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SampleScene")
            {
                GameStateService.SetState(GameFlowState.Hub);
            }
        }
    }
}
