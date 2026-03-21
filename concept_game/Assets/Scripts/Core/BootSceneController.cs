using UnityEngine;

namespace MossHarbor.Core
{
    public sealed class BootSceneController : MonoBehaviour
    {
        [SerializeField] private bool autoLoadHubOnStart = true;

        private void Start()
        {
            if (!autoLoadHubOnStart)
            {
                return;
            }

            if (GameBootstrap.Instance == null)
            {
                Debug.LogWarning("BootSceneController requires GameBootstrap in the scene.");
                return;
            }

            GameBootstrap.Instance.SceneFlowService.LoadHub();
        }
    }
}
