using MossHarbor.Core;
using MossHarbor.Data;
using UnityEngine;

namespace MossHarbor.Hub
{
    public sealed class HubManager : MonoBehaviour
    {
        private const string HarborPumpUpgradeId = "harbor_pump";
        private const string RouteScannerUpgradeId = "route_scanner";
        private const string PearlResonatorUpgradeId = "pearl_resonator";

        [SerializeField] private string firstDistrictId = "dock";
        [SerializeField] private DistrictDef defaultDistrict;
        [SerializeField] private ToolDef defaultTool;
        [SerializeField] private HubZoneDef defaultHubZone;
        [SerializeField] private QuestDef starterQuest;
        [SerializeField] private HubUpgradeDef harborPumpUpgrade;
        [SerializeField] private HubUpgradeDef routeScannerUpgrade;
        [SerializeField] private HubUpgradeDef pearlResonatorUpgrade;

        private GameBootstrap _bootstrap;
        private DistrictContentBundle _contentBundle;
        private DistrictDef _runtimeDistrict;
        private QuestDef _runtimeQuest;
        private HubZoneDef _runtimeHubZone;
        private int _selectedDistrictIndex;

        private void Start()
        {
            _bootstrap = GameBootstrap.Instance;
            if (_bootstrap != null)
            {
                _selectedDistrictIndex = _bootstrap.SaveService.Current.selectedDistrictIndex;
                ResolveDistrictFromSelection();
                _bootstrap.GameStateService.SetState(GameFlowState.Hub);
                ProcessCompletedRun();
                SyncTutorialProgressAfterHubLoad();
            }
            else
            {
                ApplyContentBundle(DistrictContentCatalog.LoadDefault());
            }
        }

        public DistrictContentBundle RuntimeContentBundle => _contentBundle;
        public DistrictDef RuntimeDistrict => _runtimeDistrict;
        public ToolDef RuntimeTool => defaultTool != null ? defaultTool : Resources.Load<ToolDef>(ContentPaths.DefaultTool);
        public QuestDef RuntimeQuest => _runtimeQuest;
        public HubZoneDef RuntimeHubZone => _runtimeHubZone;
        public HubUpgradeDef HarborPumpUpgrade => harborPumpUpgrade != null ? harborPumpUpgrade : Resources.Load<HubUpgradeDef>(ContentPaths.HarborPumpUpgrade);
        public HubUpgradeDef RouteScannerUpgrade => routeScannerUpgrade != null ? routeScannerUpgrade : Resources.Load<HubUpgradeDef>(ContentPaths.RouteScannerUpgrade);
        public HubUpgradeDef PearlResonatorUpgrade => pearlResonatorUpgrade != null ? pearlResonatorUpgrade : Resources.Load<HubUpgradeDef>(ContentPaths.PearlResonatorUpgrade);
        public int SelectedDistrictIndex => _selectedDistrictIndex;
        public int HarborPumpLevel => _bootstrap != null ? _bootstrap.SaveService.GetHubUpgradeLevel(HarborPumpUpgradeId) : 0;
        public int HarborPumpScrapCost => HarborPumpUpgrade != null ? HarborPumpUpgrade.costAmount : 15;
        public int RouteScannerLevel => _bootstrap != null ? _bootstrap.SaveService.GetHubUpgradeLevel(RouteScannerUpgradeId) : 0;
        public int RouteScannerBloomCost => RouteScannerUpgrade != null ? RouteScannerUpgrade.costAmount : 60;
        public int PearlResonatorLevel => _bootstrap != null ? _bootstrap.SaveService.GetHubUpgradeLevel(PearlResonatorUpgradeId) : 0;
        public int PearlResonatorWaterCost => PearlResonatorUpgrade != null ? PearlResonatorUpgrade.costAmount : 20;
        public bool IsTutorialActive => _bootstrap != null && _bootstrap.SaveService.IsTutorialActive();
        public bool ActiveQuestClaimed => _bootstrap != null && RuntimeQuest != null && _bootstrap.SaveService.IsQuestClaimed(RuntimeQuest.questId);
        public bool SelectedDistrictUnlocked => _runtimeDistrict == null || _bootstrap == null || _bootstrap.SaveService.GetTotalDistrictStars() >= _runtimeDistrict.requiredStars;
        public int SelectedDistrictStars => _runtimeDistrict == null || _bootstrap == null ? 0 : _bootstrap.SaveService.GetDistrictStars(_runtimeDistrict.districtId);
        public bool SelectedHubZoneRestored => ResolveHubZoneRestoredState();
        public int TotalDistrictStars => _bootstrap == null ? 0 : _bootstrap.SaveService.GetTotalDistrictStars();
        public int HarborPumpWaterBonus => HarborPumpLevel > 0 && HarborPumpUpgrade != null ? HarborPumpUpgrade.cleanWaterBonus : 0;
        public int RouteScannerTimerBonus => RouteScannerLevel > 0 && RouteScannerUpgrade != null ? Mathf.RoundToInt(RouteScannerUpgrade.timerBonusSeconds) : 0;
        public float RouteScannerBloomMultiplier => RouteScannerLevel > 0 && RouteScannerUpgrade != null ? RouteScannerUpgrade.bloomMultiplier : 1f;
        public int PearlResonatorMemoryBonus => PearlResonatorLevel > 0 && PearlResonatorUpgrade != null ? PearlResonatorUpgrade.memoryPearlBonus : 0;
        public bool CanAffordSelectedExpedition => _bootstrap != null && _bootstrap.SaveService.GetResource(ResourceType.BloomDust) >= EffectiveEntryCost;
        public bool CanAffordHarborPump => _bootstrap != null && HarborPumpUpgrade != null && _bootstrap.SaveService.GetResource(HarborPumpUpgrade.costType) >= HarborPumpUpgrade.costAmount;
        public int EffectiveEntryCost
        {
            get
            {
                var baseCost = _runtimeDistrict != null ? _runtimeDistrict.expeditionEntryCost : 10;
                var reduction = HarborPumpLevel > 0 && HarborPumpUpgrade != null ? HarborPumpUpgrade.entryCostReduction : 0;
                return Mathf.Max(1, baseCost - reduction);
            }
        }

        [ContextMenu("Grant Debug Starter Resources")]
        public void GrantDebugStarterResources()
        {
            if (_bootstrap == null)
            {
                return;
            }

            _bootstrap.SaveService.AddResource(ResourceType.BloomDust, 100);
            _bootstrap.SaveService.AddResource(ResourceType.Scrap, 25);
            _bootstrap.SaveService.AddResource(ResourceType.SeedPod, 10);
        }

        [ContextMenu("Start Expedition")]
        public void StartExpedition()
        {
            if (_bootstrap == null)
            {
                return;
            }

            ResolveDistrictFromSelection();
            if (!SelectedDistrictUnlocked)
            {
                Debug.Log("Selected district is locked.");
                return;
            }

            if (_bootstrap.SaveService.GetResource(ResourceType.BloomDust) < EffectiveEntryCost)
            {
                Debug.Log("Not enough BloomDust to start expedition.");
                return;
            }

            _bootstrap.SaveService.AddResource(ResourceType.BloomDust, -EffectiveEntryCost);
            if (_bootstrap.SaveService.GetTutorialStage() == TutorialStage.StartFirstExpedition)
            {
                _bootstrap.SaveService.AdvanceTutorialStage(TutorialStage.ReviewFirstResults);
            }

            _bootstrap.SceneFlowService.LoadExpedition();
        }

        [ContextMenu("Mark First District Complete")]
        public void MarkFirstDistrictComplete()
        {
            if (_bootstrap == null)
            {
                return;
            }

            _bootstrap.SaveService.SetDistrictStars(_runtimeDistrict != null ? _runtimeDistrict.districtId : firstDistrictId, 1);
        }

        [ContextMenu("Install Harbor Pump")]
        public void TryInstallHarborPump()
        {
            if (_bootstrap == null || HarborPumpLevel > 0)
            {
                return;
            }

            var upgrade = HarborPumpUpgrade;
            if (upgrade == null || _bootstrap.SaveService.GetResource(upgrade.costType) < upgrade.costAmount)
            {
                Debug.Log("Not enough Scrap to install Harbor Pump.");
                return;
            }

            _bootstrap.SaveService.AddResource(upgrade.costType, -upgrade.costAmount);
            _bootstrap.SaveService.SetHubUpgradeLevel(HarborPumpUpgradeId, 1);
            if (upgrade.cleanWaterBonus > 0)
            {
                _bootstrap.SaveService.AddResource(ResourceType.CleanWater, upgrade.cleanWaterBonus);
            }

            CompleteTutorialIfReady();
        }

        [ContextMenu("Install Route Scanner")]
        public void TryInstallRouteScanner()
        {
            if (_bootstrap == null || RouteScannerLevel > 0)
            {
                return;
            }

            var upgrade = RouteScannerUpgrade;
            if (upgrade == null || _bootstrap.SaveService.GetResource(upgrade.costType) < upgrade.costAmount)
            {
                Debug.Log("Not enough BloomDust to install Route Scanner.");
                return;
            }

            _bootstrap.SaveService.AddResource(upgrade.costType, -upgrade.costAmount);
            _bootstrap.SaveService.SetHubUpgradeLevel(RouteScannerUpgradeId, 1);
            CompleteTutorialIfReady();
        }

        [ContextMenu("Install Pearl Resonator")]
        public void TryInstallPearlResonator()
        {
            if (_bootstrap == null || PearlResonatorLevel > 0)
            {
                return;
            }

            var upgrade = PearlResonatorUpgrade;
            if (upgrade == null || _bootstrap.SaveService.GetResource(upgrade.costType) < upgrade.costAmount)
            {
                Debug.Log("Not enough CleanWater to install Pearl Resonator.");
                return;
            }

            _bootstrap.SaveService.AddResource(upgrade.costType, -upgrade.costAmount);
            _bootstrap.SaveService.SetHubUpgradeLevel(PearlResonatorUpgradeId, 1);
            CompleteTutorialIfReady();
        }

        [ContextMenu("Select Next District")]
        public void SelectNextDistrict()
        {
            ChangeDistrictSelection(1);
        }

        [ContextMenu("Select Previous District")]
        public void SelectPreviousDistrict()
        {
            ChangeDistrictSelection(-1);
        }

        private void ProcessCompletedRun()
        {
            if (_runtimeDistrict == null)
            {
                return;
            }

            var summary = _bootstrap.SaveService.Current.lastRunSummary;
            if (!summary.completed || summary.districtId != _runtimeDistrict.districtId)
            {
                return;
            }

            _bootstrap.SaveService.SetDistrictStars(_runtimeDistrict.districtId, 1);
            if (!string.IsNullOrWhiteSpace(_runtimeHubZone != null ? _runtimeHubZone.zoneId : null))
            {
                _bootstrap.SaveService.SetHubZoneRestorationState(_runtimeHubZone.zoneId, true);
            }

            if (RuntimeQuest == null || _bootstrap.SaveService.IsQuestClaimed(RuntimeQuest.questId))
            {
                return;
            }

            _bootstrap.SaveService.AddResource(RuntimeQuest.rewardType, RuntimeQuest.rewardAmount);
            _bootstrap.SaveService.MarkQuestClaimed(RuntimeQuest.questId);
        }

        private void ChangeDistrictSelection(int delta)
        {
            if (_bootstrap == null || ContentPaths.DistrictCount <= 0)
            {
                return;
            }

            _selectedDistrictIndex = (_selectedDistrictIndex + delta + ContentPaths.DistrictCount) % ContentPaths.DistrictCount;
            _bootstrap.SaveService.SetSelectedDistrictIndex(_selectedDistrictIndex);
            ResolveDistrictFromSelection();
        }

        private bool ResolveHubZoneRestoredState()
        {
            if (_runtimeHubZone == null)
            {
                return false;
            }

            if (_bootstrap == null)
            {
                return _runtimeHubZone.defaultRestored;
            }

            return _bootstrap.SaveService.TryGetHubZoneRestorationState(_runtimeHubZone.zoneId, out var restored)
                ? restored
                : _runtimeHubZone.defaultRestored;
        }

        private void SyncTutorialProgressAfterHubLoad()
        {
            if (_bootstrap == null)
            {
                return;
            }

            if (_bootstrap.SaveService.GetTutorialStage() != TutorialStage.ReviewFirstResults || !_bootstrap.SaveService.HasRunOutcome())
            {
                return;
            }

            if (_bootstrap.SaveService.Current.lastRunSummary.completed)
            {
                _bootstrap.SaveService.AdvanceTutorialStage(TutorialStage.InstallFirstUpgrade);
                return;
            }

            _bootstrap.SaveService.SetTutorialStage(TutorialStage.StartFirstExpedition);
        }

        private void CompleteTutorialIfReady()
        {
            if (_bootstrap == null)
            {
                return;
            }

            if (_bootstrap.SaveService.GetTutorialStage() == TutorialStage.InstallFirstUpgrade)
            {
                _bootstrap.SaveService.CompleteTutorial();
            }
        }

        private void ResolveDistrictFromSelection()
        {
            var bundle = _bootstrap == null
                ? DistrictContentCatalog.LoadDefault()
                : DistrictContentCatalog.LoadByIndex(_selectedDistrictIndex);

            ApplyContentBundle(bundle);
        }

        private void ApplyContentBundle(DistrictContentBundle bundle)
        {
            _contentBundle = bundle;
            _runtimeDistrict = bundle?.District != null ? bundle.District : defaultDistrict;
            _runtimeHubZone = bundle?.HubZone != null ? bundle.HubZone : defaultHubZone;
            _runtimeQuest = bundle?.Quest != null ? bundle.Quest : starterQuest;

            if (_runtimeDistrict == null)
            {
                _runtimeDistrict = Resources.Load<DistrictDef>(ContentPaths.DefaultDistrict);
            }

            if (_runtimeHubZone == null)
            {
                _runtimeHubZone = Resources.Load<HubZoneDef>(ContentPaths.DefaultHubZone);
            }

            if (_runtimeQuest == null)
            {
                _runtimeQuest = Resources.Load<QuestDef>(ContentPaths.DefaultQuest);
            }
        }
    }
}
