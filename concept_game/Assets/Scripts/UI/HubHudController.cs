using MossHarbor.Core;
using MossHarbor.Data;
using MossHarbor.Hub;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MossHarbor.UI
{
    public sealed class HubHudController : MonoBehaviour
    {
        private TMP_Text _objectiveTitleText;
        private TMP_Text _objectiveBodyText;
        private TMP_Text _bundleSummaryText;
        private TMP_Text _resourceText;
        private TMP_Text _runSummaryText;
        private HubManager _hubManager;
        private Button _startExpeditionButton;
        private Button _harborPumpButton;
        private Button _routeScannerButton;
        private Button _pearlResonatorButton;
        private Button _previousDistrictButton;
        private Button _nextDistrictButton;
        private Button _debugResourcesButton;

        private void Start()
        {
            _hubManager = GetComponent<HubManager>();
            BuildUi();
        }

        private void Update()
        {
            if (_resourceText == null || GameBootstrap.Instance == null)
            {
                return;
            }

            var save = GameBootstrap.Instance.SaveService;
            _resourceText.text =
                $"BloomDust {save.GetResource(ResourceType.BloomDust)}\n" +
                $"Scrap {save.GetResource(ResourceType.Scrap)}\n" +
                $"SeedPod {save.GetResource(ResourceType.SeedPod)}\n" +
                $"CleanWater {save.GetResource(ResourceType.CleanWater)}\n" +
                $"MemoryPearl {save.GetResource(ResourceType.MemoryPearl)}";

            if (_runSummaryText != null)
            {
                var summary = save.Current.lastRunSummary;
                var lastRunBundle = DistrictContentCatalog.LoadByDistrictId(summary.districtId);
                _runSummaryText.text =
                    $"Last Run: {summary.resultLabel}\n" +
                    $"Objective\n{summary.GetObjectiveSummary(lastRunBundle)}\n" +
                    $"Rewards\n{summary.GetRewardSummary()}\n" +
                    $"Pickups: {summary.pickupsCollected}\n" +
                    $"Duration: {summary.durationSeconds:0.0}s";
            }

            if (_hubManager == null)
            {
                return;
            }

            if (_objectiveTitleText != null)
            {
                _objectiveTitleText.text = _hubManager.IsTutorialActive ? "First Session Goal" : "Recommended Action";
            }

            if (_objectiveBodyText != null)
            {
                _objectiveBodyText.text = BuildPrimaryObjectiveText(save);
            }

            if (_bundleSummaryText != null)
            {
                _bundleSummaryText.text = BuildBundleSummaryText();
            }

            RefreshButtonStates(save);
        }

        private void BuildUi()
        {
            var canvas = RuntimeUiFactory.CreateCanvas("HubCanvas");
            var title = RuntimeUiFactory.CreateLabel(canvas.transform, "HubTitle", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(32f, -88f), new Vector2(420f, -24f), 34, TextAlignmentOptions.TopLeft);
            title.text = "Moss Harbor Hub";

            RuntimeUiFactory.CreatePanel(canvas.transform, "ObjectivePanel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-340f, -214f), new Vector2(340f, -34f), new Color(0.05f, 0.11f, 0.14f, 0.72f));
            _objectiveTitleText = RuntimeUiFactory.CreateLabel(canvas.transform, "ObjectiveTitle", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-240f, -82f), new Vector2(240f, -42f), 22, TextAlignmentOptions.Center);
            _objectiveBodyText = RuntimeUiFactory.CreateLabel(canvas.transform, "ObjectiveBody", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-300f, -184f), new Vector2(300f, -80f), 17, TextAlignmentOptions.Center);

            RuntimeUiFactory.CreatePanel(canvas.transform, "BundlePanel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-260f, -318f), new Vector2(260f, -226f), new Color(0.08f, 0.13f, 0.18f, 0.68f));
            _bundleSummaryText = RuntimeUiFactory.CreateLabel(canvas.transform, "BundleSummary", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-228f, -308f), new Vector2(228f, -232f), 16, TextAlignmentOptions.Center);

            RuntimeUiFactory.CreatePanel(canvas.transform, "ResourcePanel", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -230f), new Vector2(250f, -76f), new Color(0.09f, 0.13f, 0.16f, 0.7f));
            _resourceText = RuntimeUiFactory.CreateLabel(canvas.transform, "HubResources", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(34f, -214f), new Vector2(236f, -90f), 18, TextAlignmentOptions.TopLeft);

            RuntimeUiFactory.CreatePanel(canvas.transform, "RunSummaryPanel", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-290f, -230f), new Vector2(-20f, -76f), new Color(0.08f, 0.11f, 0.15f, 0.7f));
            _runSummaryText = RuntimeUiFactory.CreateLabel(canvas.transform, "LastRunSummary", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-276f, -216f), new Vector2(-34f, -92f), 17, TextAlignmentOptions.TopRight);

            _startExpeditionButton = RuntimeUiFactory.CreateButton(canvas.transform, "Start Expedition", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(24f, 32f), new Vector2(254f, 88f));
            _startExpeditionButton.onClick.AddListener(() => _hubManager?.StartExpedition());

            _harborPumpButton = RuntimeUiFactory.CreateButton(canvas.transform, "Install Harbor Pump", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(24f, 96f), new Vector2(254f, 152f));
            _harborPumpButton.onClick.AddListener(() => _hubManager?.TryInstallHarborPump());

            _routeScannerButton = RuntimeUiFactory.CreateButton(canvas.transform, "Install Route Scanner", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(24f, 160f), new Vector2(254f, 216f));
            _routeScannerButton.onClick.AddListener(() => _hubManager?.TryInstallRouteScanner());

            _pearlResonatorButton = RuntimeUiFactory.CreateButton(canvas.transform, "Install Pearl Resonator", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(24f, 224f), new Vector2(254f, 280f));
            _pearlResonatorButton.onClick.AddListener(() => _hubManager?.TryInstallPearlResonator());

            _previousDistrictButton = RuntimeUiFactory.CreateButton(canvas.transform, "Previous District", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-254f, 32f), new Vector2(-24f, 88f));
            _previousDistrictButton.onClick.AddListener(() => _hubManager?.SelectPreviousDistrict());

            _nextDistrictButton = RuntimeUiFactory.CreateButton(canvas.transform, "Next District", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-254f, 96f), new Vector2(-24f, 152f));
            _nextDistrictButton.onClick.AddListener(() => _hubManager?.SelectNextDistrict());

            _debugResourcesButton = RuntimeUiFactory.CreateButton(canvas.transform, "[Debug] Grant Resources", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-254f, 224f), new Vector2(-24f, 280f));
            _debugResourcesButton.onClick.AddListener(() => _hubManager?.GrantDebugStarterResources());
        }

        private string BuildPrimaryObjectiveText(SaveService save)
        {
            var districtName = _hubManager.RuntimeDistrict != null ? _hubManager.RuntimeDistrict.displayName : "Dock";
            var zoneName = _hubManager.RuntimeHubZone != null ? _hubManager.RuntimeHubZone.displayName : districtName;
            var questName = _hubManager.RuntimeQuest != null ? _hubManager.RuntimeQuest.displayName : "Restore the harbor";
            var routeDirective = BuildRouteDirectiveText();

            switch (save.GetTutorialStage())
            {
                case TutorialStage.StartFirstExpedition:
                    return
                        $"Launch the first trip into {districtName}.\n" +
                        $"{routeDirective}\n" +
                        $"Spend {_hubManager.EffectiveEntryCost} BloomDust to start the expedition.";
                case TutorialStage.ReviewFirstResults:
                    return
                        "Your first run is recorded.\n" +
                        "Open the results, then return to the hub for the next step.";
                case TutorialStage.InstallFirstUpgrade:
                    return
                        $"Turn the first return into a lasting upgrade.\n" +
                        $"Install Harbor Pump for {zoneName} with {_hubManager.HarborPumpScrapCost} Scrap.";
                default:
                    if (!_hubManager.SelectedDistrictUnlocked)
                    {
                        return $"Earn more stars to unlock {districtName}.";
                    }

                    if (!_hubManager.SelectedHubZoneRestored)
                    {
                        return
                            $"Run {districtName} again.\n" +
                            $"{routeDirective}\n" +
                            $"Restore {zoneName} and push district progress.";
                    }

                    if (_hubManager.HarborPumpLevel == 0)
                    {
                        return $"Harbor Pump is still offline.\nInstall it when you have {_hubManager.HarborPumpScrapCost} Scrap.";
                    }

                    if (_hubManager.RuntimeDistrict != null && _hubManager.RuntimeDistrict.objectiveType == ExpeditionObjectiveType.HoldOut && _hubManager.RouteScannerLevel == 0)
                    {
                        return
                            $"{routeDirective}\n" +
                            $"Install Route Scanner, then push a steadier hold in {districtName}.";
                    }

                    return
                        $"{routeDirective}\n" +
                        $"Secure more stars in {districtName} and keep the harbor upgrades coming.";
            }
        }

        private string BuildBundleSummaryText()
        {
            var district = _hubManager.RuntimeDistrict;
            var zone = _hubManager.RuntimeHubZone;
            var quest = _hubManager.RuntimeQuest;
            var districtName = district != null ? district.displayName : "Unknown District";
            var zoneName = zone != null ? zone.displayName : "Unknown Zone";
            var questName = quest != null ? quest.displayName : "No Quest";
            var requiredStars = district != null ? district.requiredStars : 0;
            var questState = _hubManager.ActiveQuestClaimed ? "Reward claimed" : "Reward pending";
            var zoneState = _hubManager.SelectedHubZoneRestored ? "Restored" : "Needs restoration";
            var districtState = _hubManager.SelectedDistrictUnlocked ? "Unlocked" : $"Locked at {requiredStars} total stars";

            return
                $"District: {districtName} ({districtState})\n" +
                $"Zone: {zoneName} ({zoneState})\n" +
                $"Quest: {questName} ({questState})\n" +
                $"Entry Cost: {_hubManager.EffectiveEntryCost} BloomDust | District Stars: {_hubManager.SelectedDistrictStars} | Total Stars: {_hubManager.TotalDistrictStars}";
        }

        private string BuildRouteDirectiveText()
        {
            var district = _hubManager.RuntimeDistrict;
            var quest = _hubManager.RuntimeQuest;
            var districtName = district != null && !string.IsNullOrWhiteSpace(district.displayName) ? district.displayName : "Dock";

            if (district == null)
            {
                return quest != null && !string.IsNullOrWhiteSpace(quest.objectiveText)
                    ? quest.objectiveText
                    : "Recover enough salvage to activate the beacon.";
            }

            switch (district.objectiveType)
            {
                case ExpeditionObjectiveType.CollectResource:
                    return $"Recover {Mathf.Max(1, district.objectiveTargetAmount)} {district.objectiveResourceType} from {districtName} before extraction.";
                case ExpeditionObjectiveType.HoldOut:
                    return $"Hold the route in {districtName} for {Mathf.Max(1f, district.objectiveHoldSeconds):0} seconds, then fall back to the beacon.";
                default:
                    return quest != null && !string.IsNullOrWhiteSpace(quest.objectiveText)
                        ? quest.objectiveText
                        : "Recover enough salvage to activate the beacon.";
            }
        }

        private void RefreshButtonStates(SaveService save)
        {
            var tutorialActive = _hubManager.IsTutorialActive;
            var stage = save.GetTutorialStage();

            if (_startExpeditionButton != null)
            {
                _startExpeditionButton.interactable = _hubManager.SelectedDistrictUnlocked && _hubManager.CanAffordSelectedExpedition;
            }

            if (_harborPumpButton != null)
            {
                var showHarborPump = !tutorialActive || stage == TutorialStage.InstallFirstUpgrade || _hubManager.HarborPumpLevel > 0;
                _harborPumpButton.gameObject.SetActive(showHarborPump);
                _harborPumpButton.interactable = _hubManager.HarborPumpLevel == 0 && _hubManager.CanAffordHarborPump;
            }

            if (_routeScannerButton != null)
            {
                _routeScannerButton.gameObject.SetActive(!tutorialActive);
                _routeScannerButton.interactable = _hubManager.RouteScannerLevel == 0 && save.GetResource(ResourceType.BloomDust) >= _hubManager.RouteScannerBloomCost;
            }

            if (_pearlResonatorButton != null)
            {
                _pearlResonatorButton.gameObject.SetActive(!tutorialActive);
                _pearlResonatorButton.interactable = _hubManager.PearlResonatorLevel == 0 && save.GetResource(ResourceType.CleanWater) >= _hubManager.PearlResonatorWaterCost;
            }

            if (_previousDistrictButton != null)
            {
                _previousDistrictButton.gameObject.SetActive(!tutorialActive);
            }

            if (_nextDistrictButton != null)
            {
                _nextDistrictButton.gameObject.SetActive(!tutorialActive);
            }

            if (_debugResourcesButton != null)
            {
                _debugResourcesButton.gameObject.SetActive(Application.isEditor);
            }
        }
    }
}
