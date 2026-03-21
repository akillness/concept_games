using MossHarbor.Core;
using MossHarbor.Data;
using TMPro;
using UnityEngine;

namespace MossHarbor.UI
{
    public sealed class ResultsHudController : MonoBehaviour
    {
        private const string RouteScannerUpgradeId = "route_scanner";
        [SerializeField] private ResultsManager resultsManager;
        private TMP_Text _body;
        private TMP_Text _nextActionText;
        private TMP_Text _sceneHint;

        private void Start()
        {
            if (resultsManager == null)
            {
                resultsManager = GetComponent<ResultsManager>();
            }

            BuildUi();
            RefreshUi();
        }

        private void BuildUi()
        {
            var canvas = RuntimeUiFactory.CreateCanvas("ResultsCanvas");

            var title = RuntimeUiFactory.CreateLabel(canvas.transform, "ResultsTitle", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-240f, -92f), new Vector2(240f, -28f), 36, TextAlignmentOptions.Center);
            title.text = "Expedition Results";

            _nextActionText = RuntimeUiFactory.CreateLabel(canvas.transform, "ResultsNextAction", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-320f, -154f), new Vector2(320f, -110f), 18, TextAlignmentOptions.Center);
            RuntimeUiFactory.CreatePanel(canvas.transform, "ResultsBodyPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-330f, -160f), new Vector2(330f, 110f), new Color(0.05f, 0.1f, 0.14f, 0.76f));
            _body = RuntimeUiFactory.CreateLabel(canvas.transform, "ResultsBody", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-280f, -120f), new Vector2(280f, 70f), 21, TextAlignmentOptions.Center);

            var button = RuntimeUiFactory.CreateButton(canvas.transform, "Return To Hub", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-150f, 44f), new Vector2(150f, 98f));
            button.onClick.AddListener(() => resultsManager?.ReturnToHub());

            _sceneHint = RuntimeUiFactory.CreateLabel(canvas.transform, "ResultsHint", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-260f, 18f), new Vector2(-18f, 46f), 16, TextAlignmentOptions.BottomRight);
        }

        private void RefreshUi()
        {
            if (GameBootstrap.Instance == null)
            {
                return;
            }

            var summary = GameBootstrap.Instance.SaveService.Current.lastRunSummary;
            var bundle = DistrictContentCatalog.LoadByDistrictId(summary.districtId);
            if (_body != null)
            {
                var districtName = bundle.District != null ? bundle.District.displayName : summary.districtId;
                var questName = bundle.Quest != null ? bundle.Quest.displayName : "No Quest";
                var zoneName = bundle.HubZone != null ? bundle.HubZone.displayName : districtName;
                _body.text =
                    $"District: {districtName}\n" +
                    $"Zone: {zoneName}\n" +
                    $"Quest: {questName}\n" +
                    $"Objective\n{summary.GetObjectiveSummary(bundle)}\n\n" +
                    $"Rewards\n{summary.GetRewardSummary()}\n" +
                    $"Pickups: {summary.pickupsCollected}\n" +
                    $"Duration: {summary.durationSeconds:0.0}s";
            }

            if (_sceneHint != null)
            {
                _sceneHint.text = $"Current Save Scene: {GameBootstrap.Instance.SaveService.Current.currentScene}";
            }

            if (_nextActionText != null)
            {
                var save = GameBootstrap.Instance.SaveService;
                _nextActionText.text = BuildNextActionText(save, summary, bundle);
            }
        }

        private static string BuildNextActionText(SaveService save, RunSummary summary, DistrictContentBundle bundle)
        {
            var districtName = bundle.District != null ? bundle.District.displayName : summary.districtId;
            var zoneName = bundle.HubZone != null ? bundle.HubZone.displayName : districtName;
            var objectiveType = summary.ResolveObjectiveType(bundle);
            var resourceType = summary.ResolveObjectiveResourceType(bundle);

            if (save.GetTutorialStage() == TutorialStage.ReviewFirstResults)
            {
                return summary.completed
                    ? $"Next Action\nReturn to hub and install Harbor Pump for {zoneName}."
                    : $"Next Action\nReturn to hub, restock BloomDust, and run {districtName} again.";
            }

            if (!summary.completed)
            {
                switch (objectiveType)
                {
                    case ExpeditionObjectiveType.CollectResource:
                        return $"Next Action\nRestock BloomDust, tighten the route, and recover {resourceType} from {districtName} again.";
                    case ExpeditionObjectiveType.HoldOut:
                        return $"Next Action\nReturn to hub, stabilize the route, and survive the next hold in {districtName}.";
                    default:
                        return $"Next Action\nReturn to hub, restock BloomDust, and clear pickups in {districtName} faster.";
                }
            }

            switch (objectiveType)
            {
                case ExpeditionObjectiveType.CollectResource:
                    return $"Next Action\nBank the {resourceType} haul, then rerun {districtName} or pivot into the next upgrade.";
                case ExpeditionObjectiveType.HoldOut:
                    return save.GetHubUpgradeLevel(RouteScannerUpgradeId) == 0
                        ? $"Next Action\nReturn to hub and install Route Scanner before the next hold in {districtName}."
                        : $"Next Action\nReturn to hub, restock, and push a longer hold in {districtName}.";
                default:
                    return $"Next Action\nReturn to hub, secure {zoneName}, and choose the next upgrade or route.";
            }
        }
    }
}
