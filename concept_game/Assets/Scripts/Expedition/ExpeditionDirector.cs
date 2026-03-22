using MossHarbor.Art;
using MossHarbor.Core;
using MossHarbor.Data;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class ExpeditionDirector : MonoBehaviour
    {
        [SerializeField] private DistrictDef districtDefinition;
        [SerializeField] private bool autoCompleteWhenTimerEnds;
        [SerializeField] private bool generateRunContent = true;
        [SerializeField] private float pickupRotateSpeed = 90f;
        [Header("Verification Overrides")]
        [SerializeField] private string districtIdOverride;
        [SerializeField] private float objectiveHoldSecondsOverride;

        private GameBootstrap _bootstrap;
        private DistrictContentBundle _contentBundle;
        private float _remainingTime;
        private bool _runActive;
        private int _bloomDustCollected;
        private int _scrapCollected;
        private int _seedPodCollected;
        private int _pickupsCollected;
        private Transform _runtimeContentRoot;
        private ObjectiveService _objectiveService;

        public float RemainingTime => _remainingTime;
        public DistrictContentBundle RuntimeContentBundle => _contentBundle;
        public DistrictDef RuntimeDistrict => districtDefinition;
        public QuestDef RuntimeQuest => _contentBundle != null ? _contentBundle.Quest : null;
        public int BloomDustCollected => _bloomDustCollected;
        public int ScrapCollected => _scrapCollected;
        public int SeedPodCollected => _seedPodCollected;
        public int PickupsCollected => _pickupsCollected;
        public int TargetPickupCount => _objectiveService != null ? _objectiveService.TargetAmount : (districtDefinition != null ? districtDefinition.targetPickupCount : 3);
        public bool ObjectiveReady => _objectiveService != null ? _objectiveService.IsComplete : _pickupsCollected >= TargetPickupCount;
        public string DistrictDisplayName => RuntimeDistrict != null && !string.IsNullOrWhiteSpace(RuntimeDistrict.displayName) ? RuntimeDistrict.displayName : "Dock";
        public string QuestDisplayName => RuntimeQuest != null && !string.IsNullOrWhiteSpace(RuntimeQuest.displayName) ? RuntimeQuest.displayName : "Restore the harbor";
        public string ObjectiveDescription => _objectiveService != null
            ? _objectiveService.GetInstructionText(DistrictDisplayName, RuntimeQuest != null && !string.IsNullOrWhiteSpace(RuntimeQuest.objectiveText) ? RuntimeQuest.objectiveText : "Collect enough salvage and activate the beacon.")
            : (RuntimeQuest != null && !string.IsNullOrWhiteSpace(RuntimeQuest.objectiveText) ? RuntimeQuest.objectiveText : "Collect enough salvage and activate the beacon.");
        public string ObjectiveProgressText => _objectiveService != null ? _objectiveService.GetProgressText() : $"Pickups: {_pickupsCollected} / {TargetPickupCount}";
        public Color DistrictThemeColor => RuntimeDistrict != null ? RuntimeDistrict.districtColor : new Color(0.4f, 0.95f, 0.85f, 1f);
        public DifficultyLevel ActiveDifficulty => _bootstrap != null ? _bootstrap.SaveService.Current.selectedDifficulty : DifficultyLevel.Normal;

        private void Start()
        {
            _bootstrap = GameBootstrap.Instance;
            ResolveConfiguredDistrict();
            _objectiveService = new ObjectiveService(districtDefinition, objectiveHoldSecondsOverride);
            var routeScannerUpgrade = Resources.Load<HubUpgradeDef>(ContentPaths.RouteScannerUpgrade);
            var routeScannerBonus = _bootstrap != null && _bootstrap.SaveService.GetHubUpgradeLevel(UpgradeIds.RouteScanner) > 0 && routeScannerUpgrade != null
                ? routeScannerUpgrade.timerBonusSeconds
                : 0f;
            var difficulty = _bootstrap != null ? _bootstrap.SaveService.Current.selectedDifficulty : DifficultyLevel.Normal;
            var baseTimer = districtDefinition != null ? districtDefinition.runTimerSeconds : 180f;
            var runTimerSeconds = baseTimer * DifficultyConfig.TimerMultiplier(difficulty) + routeScannerBonus;
            _remainingTime = runTimerSeconds;
            _runActive = true;
            if (generateRunContent)
            {
                BuildDistrictContent();
            }
            else
            {
                RuntimeArtDirector.DecorateExpedition(transform);
            }

            if (_bootstrap != null)
            {
                _bootstrap.GameStateService.SetState(GameFlowState.Expedition);
            }
        }

        private void Update()
        {
            if (!_runActive)
            {
                return;
            }

            _objectiveService?.Tick(Time.deltaTime);
            _remainingTime -= Time.deltaTime;
            if (_remainingTime > 0f)
            {
                return;
            }

            _runActive = false;

            if (autoCompleteWhenTimerEnds)
            {
                CompleteCurrentRun();
                return;
            }

            FailCurrentRun();
        }

        [ContextMenu("Complete Run")]
        public void CompleteCurrentRun()
        {
            _runActive = false;
            if (_bootstrap != null)
            {
                var reward = RewardCalculator.CalculateSuccess(
                    districtDefinition, _bootstrap.SaveService,
                    _bloomDustCollected, _scrapCollected, _remainingTime);

                _bootstrap.SaveService.AddResource(ResourceType.BloomDust, reward.bloomDust);
                _bootstrap.SaveService.AddResource(ResourceType.Scrap, reward.scrap);
                if (reward.cleanWater > 0)
                {
                    _bootstrap.SaveService.AddResource(ResourceType.CleanWater, reward.cleanWater);
                }
                if (reward.memoryPearl > 0)
                {
                    _bootstrap.SaveService.AddResource(ResourceType.MemoryPearl, reward.memoryPearl);
                }
                if (_seedPodCollected > 0)
                {
                    _bootstrap.SaveService.AddResource(ResourceType.SeedPod, _seedPodCollected);
                }
                _bootstrap.SaveService.SetLastRunSummary(BuildRunSummary(
                    completed: true,
                    bloomDustCollected: reward.bloomDust,
                    scrapCollected: reward.scrap,
                    cleanWaterCollected: reward.cleanWater,
                    memoryPearlCollected: reward.memoryPearl,
                    seedPodCollected: _seedPodCollected,
                    durationSeconds: reward.duration,
                    resultLabel: $"{(districtDefinition != null ? districtDefinition.displayName : "Dock")} Cleared"));
                _bootstrap.SceneFlowService.LoadResults();
            }
        }

        [ContextMenu("Fail Run")]
        public void FailCurrentRun()
        {
            _runActive = false;
            if (_bootstrap != null)
            {
                var reward = RewardCalculator.CalculateFailure(
                    districtDefinition, _bootstrap.SaveService,
                    _bloomDustCollected, _scrapCollected, _remainingTime,
                    _bootstrap.SaveService.Current.selectedDifficulty);

                _bootstrap.SaveService.AddResource(ResourceType.BloomDust, reward.bloomDust);
                if (reward.scrap > 0)
                {
                    _bootstrap.SaveService.AddResource(ResourceType.Scrap, reward.scrap);
                }
                var retention = DifficultyConfig.FailResourceRetention(_bootstrap.SaveService.Current.selectedDifficulty);
                var retainedSeedPods = Mathf.RoundToInt(_seedPodCollected * retention);
                if (retainedSeedPods > 0)
                {
                    _bootstrap.SaveService.AddResource(ResourceType.SeedPod, retainedSeedPods);
                }
                _bootstrap.SaveService.SetLastRunSummary(BuildRunSummary(
                    completed: false,
                    bloomDustCollected: reward.bloomDust,
                    scrapCollected: reward.scrap,
                    cleanWaterCollected: 0,
                    memoryPearlCollected: 0,
                    seedPodCollected: retainedSeedPods,
                    durationSeconds: reward.duration,
                    resultLabel: "Run Failed"));
                _bootstrap.SceneFlowService.LoadResults();
            }
        }

        public void Collect(ResourceType resourceType, int amount)
        {
            if (!_runActive)
            {
                return;
            }

            switch (resourceType)
            {
                case ResourceType.BloomDust:
                    _bloomDustCollected += amount;
                    break;
                case ResourceType.Scrap:
                    _scrapCollected += amount;
                    break;
                case ResourceType.SeedPod:
                    _seedPodCollected += amount;
                    break;
            }

            _pickupsCollected++;
            _objectiveService?.RegisterCollection(resourceType, amount);
        }

        public void ActivateObjectiveBeacon()
        {
            if (!_runActive || !ObjectiveReady)
            {
                return;
            }

            CompleteCurrentRun();
        }

        private RunSummary BuildRunSummary(
            bool completed,
            int bloomDustCollected,
            int scrapCollected,
            int cleanWaterCollected,
            int memoryPearlCollected,
            int seedPodCollected,
            float durationSeconds,
            string resultLabel)
        {
            return new RunSummary
            {
                completed = completed,
                bloomDustCollected = bloomDustCollected,
                scrapCollected = scrapCollected,
                cleanWaterCollected = cleanWaterCollected,
                memoryPearlCollected = memoryPearlCollected,
                seedPodCollected = seedPodCollected,
                pickupsCollected = _pickupsCollected,
                durationSeconds = durationSeconds,
                districtId = districtDefinition != null ? districtDefinition.districtId : "dock",
                resultLabel = resultLabel,
                objectiveType = _objectiveService != null ? _objectiveService.ObjectiveType : ExpeditionObjectiveType.CollectPickups,
                objectiveResourceType = _objectiveService != null ? _objectiveService.ObjectiveResourceType : ResourceType.BloomDust,
                objectiveTargetAmount = _objectiveService != null ? _objectiveService.TargetAmount : TargetPickupCount,
                objectiveProgressAmount = ResolveObjectiveProgressAmount(),
                objectiveTargetSeconds = _objectiveService != null ? _objectiveService.TargetHoldSeconds : 0f,
                objectiveElapsedSeconds = _objectiveService != null ? _objectiveService.ElapsedSeconds : 0f,
                objectiveDescription = ObjectiveDescription,
                objectiveProgressText = ObjectiveProgressText,
            };
        }

        private int ResolveObjectiveProgressAmount()
        {
            if (_objectiveService == null)
            {
                return _pickupsCollected;
            }

            switch (_objectiveService.ObjectiveType)
            {
                case ExpeditionObjectiveType.CollectResource:
                    return _objectiveService.GetCollectedAmount(_objectiveService.ObjectiveResourceType);
                case ExpeditionObjectiveType.HoldOut:
                    return Mathf.RoundToInt(_objectiveService.ElapsedSeconds);
                default:
                    return _pickupsCollected;
            }
        }

        private void ResolveConfiguredDistrict()
        {
            if (!string.IsNullOrWhiteSpace(districtIdOverride))
            {
                _contentBundle = DistrictContentCatalog.LoadByDistrictId(districtIdOverride);
                districtDefinition = _contentBundle.District;
                return;
            }

            var selectedIndex = _bootstrap != null ? _bootstrap.SaveService.Current.selectedDistrictIndex : 0;
            if (districtDefinition == null)
            {
                _contentBundle = DistrictContentCatalog.LoadByIndex(selectedIndex);
                districtDefinition = _contentBundle.District;
            }
            else
            {
                _contentBundle = DistrictContentCatalog.LoadByDistrictId(districtDefinition.districtId);
            }

            if (districtDefinition == null)
            {
                _contentBundle = DistrictContentCatalog.LoadDefault();
                districtDefinition = _contentBundle.District;
            }
        }

        private void BuildDistrictContent()
        {
            ClearSceneRunContent();
            _runtimeContentRoot = new GameObject("GeneratedRunContent").transform;
            RuntimeArtDirector.DecorateExpedition(_runtimeContentRoot, districtDefinition);
            var bloomTint = Color.Lerp(DistrictThemeColor, Color.white, 0.3f);
            var scrapTint = Color.Lerp(DistrictThemeColor, new Color(0.9f, 0.78f, 0.42f, 1f), 0.45f);
            var seedPodTint = Color.Lerp(DistrictThemeColor, new Color(0.42f, 0.82f, 0.38f, 1f), 0.5f);

            var bloomCount = districtDefinition != null ? districtDefinition.bloomPickupCount : 2;
            var scrapCount = districtDefinition != null ? districtDefinition.scrapPickupCount : 1;
            var seedPodCount = districtDefinition != null ? districtDefinition.seedPodPickupCount : 0;
            var totalPickups = Mathf.Max(1, bloomCount + scrapCount + seedPodCount);

            for (var i = 0; i < bloomCount; i++)
            {
                CreatePickup(
                    $"BloomPickup_{i + 1}",
                    ResolvePickupPosition(i, totalPickups),
                    ResourceType.BloomDust,
                    districtDefinition != null ? districtDefinition.bloomPickupAmount : 10,
                    PrimitiveType.Cube,
                    bloomTint);
            }

            for (var i = 0; i < scrapCount; i++)
            {
                CreatePickup(
                    $"ScrapPickup_{i + 1}",
                    ResolvePickupPosition(bloomCount + i, totalPickups),
                    ResourceType.Scrap,
                    districtDefinition != null ? districtDefinition.scrapPickupAmount : 4,
                    PrimitiveType.Sphere,
                    scrapTint);
            }

            for (var i = 0; i < seedPodCount; i++)
            {
                CreatePickup(
                    $"SeedPodPickup_{i + 1}",
                    ResolvePickupPosition(bloomCount + scrapCount + i, totalPickups),
                    ResourceType.SeedPod,
                    districtDefinition != null ? districtDefinition.seedPodPickupAmount : 3,
                    PrimitiveType.Capsule,
                    seedPodTint);
            }

            CreateObjectiveBeacon(districtDefinition != null ? districtDefinition.beaconPosition : new Vector3(0f, 0.75f, 8f));
        }

        private void ClearSceneRunContent()
        {
            foreach (var pickup in FindObjectsByType<SimplePickup>(FindObjectsSortMode.None))
            {
                Destroy(pickup.gameObject);
            }

            foreach (var beacon in FindObjectsByType<ObjectiveBeacon>(FindObjectsSortMode.None))
            {
                Destroy(beacon.gameObject);
            }

            var existingRoot = GameObject.Find("GeneratedRunContent");
            if (existingRoot != null)
            {
                Destroy(existingRoot);
            }
        }

        private Vector3 ResolvePickupPosition(int index, int totalPickups)
        {
            var radius = districtDefinition != null ? districtDefinition.pickupSpawnRadius : 8f;
            var angle = totalPickups <= 1 ? 0f : (Mathf.PI * 2f * index) / totalPickups;
            return new Vector3(Mathf.Cos(angle) * radius, 0.75f, Mathf.Sin(angle) * radius);
        }

        private void CreatePickup(string objectName, Vector3 position, ResourceType resourceType, int amount, PrimitiveType primitiveType, Color tint)
        {
            var pickup = RuntimeArtDirector.CreatePickupVisual(_runtimeContentRoot, objectName, position, resourceType, tint) ?? GameObject.CreatePrimitive(primitiveType);
            pickup.name = objectName;
            pickup.transform.SetParent(_runtimeContentRoot, false);
            pickup.transform.position = position;

            if (pickup.GetComponent<SimplePickup>() == null)
            {
                pickup.transform.localScale = primitiveType == PrimitiveType.Cube ? new Vector3(1f, 1f, 1f) : new Vector3(1.15f, 1.15f, 1.15f);
                if (pickup.TryGetComponent<MeshRenderer>(out var renderer))
                {
                    renderer.material.color = tint;
                }
            }

            EnsureTriggerCollider(pickup, resourceType == ResourceType.Scrap ? 0.7f : 0.9f);

            var pickupComponent = pickup.GetComponent<SimplePickup>();
            if (pickupComponent == null)
            {
                pickupComponent = pickup.AddComponent<SimplePickup>();
            }
            pickupComponent.Configure(resourceType, amount, pickupRotateSpeed);
        }

        private void CreateObjectiveBeacon(Vector3 position)
        {
            var beacon = RuntimeArtDirector.CreateObjectiveBeaconVisual(_runtimeContentRoot, position) ?? GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beacon.name = "ObjectiveBeacon";
            beacon.transform.SetParent(_runtimeContentRoot, false);
            beacon.transform.position = position;

            if (beacon.GetComponent<ObjectiveBeacon>() == null)
            {
                beacon.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            EnsureTriggerCollider(beacon, 1.2f);

            var beaconComponent = beacon.GetComponent<ObjectiveBeacon>();
            if (beaconComponent == null)
            {
                beaconComponent = beacon.AddComponent<ObjectiveBeacon>();
            }
            beaconComponent.SetTheme(DistrictThemeColor);
        }

        private static void EnsureTriggerCollider(GameObject root, float radius)
        {
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
            {
                collider.enabled = false;
            }

            var triggerCollider = root.GetComponent<SphereCollider>();
            if (triggerCollider == null)
            {
                triggerCollider = root.AddComponent<SphereCollider>();
            }

            triggerCollider.center = Vector3.zero;
            triggerCollider.radius = radius;
            triggerCollider.isTrigger = true;
            triggerCollider.enabled = true;
        }
    }
}
