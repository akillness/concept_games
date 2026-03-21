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

            // Save star rating
            var summary = save.Current.lastRunSummary;
            var bundle = DistrictContentCatalog.LoadByDistrictId(summary.districtId);
            var stars = StarRatingCalculator.Calculate(bundle.District, summary, save.Current.selectedDifficulty);
            if (stars > 0)
            {
                save.SetDistrictStars(summary.districtId, stars);
            }

            // Existing tutorial logic
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
