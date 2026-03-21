using MossHarbor.Core;
using MossHarbor.Data;
using UnityEngine;

namespace MossHarbor.UI
{
    public sealed class ResultsManager : MonoBehaviour
    {
        [ContextMenu("Return To Hub")]
        public void ReturnToHub()
        {
            if (GameBootstrap.Instance == null)
            {
                return;
            }

            var save = GameBootstrap.Instance.SaveService;
            if (save.GetTutorialStage() == TutorialStage.ReviewFirstResults)
            {
                save.SetTutorialStage(save.Current.lastRunSummary.completed
                    ? TutorialStage.InstallFirstUpgrade
                    : TutorialStage.StartFirstExpedition);
            }

            GameBootstrap.Instance.SceneFlowService.LoadHub();
        }
    }
}
