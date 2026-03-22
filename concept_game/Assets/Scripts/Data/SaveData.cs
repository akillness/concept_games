using System;
using System.Collections.Generic;
using UnityEngine;

namespace MossHarbor.Data
{
    [Serializable]
    public sealed class SaveData
    {
        public string profileId = "slot-0";
        public string currentScene = "Hub";
        public int selectedDistrictIndex;
        public DifficultyLevel selectedDifficulty = DifficultyLevel.Normal;
        public bool tutorialStateInitialized;
        public TutorialStage tutorialStage = TutorialStage.StartFirstExpedition;
        public SerializableDictionary<ResourceType, int> resources = new();
        public SerializableDictionary<string, int> districtStars = new();
        public SerializableDictionary<string, int> hubUpgradeLevels = new();
        public SerializableDictionary<string, bool> hubZoneRestorationStates = new();
        public SerializableDictionary<string, bool> claimedQuests = new();
        public SeedPodTelemetry seedPodTelemetry = new();
        public RunSummary lastRunSummary = new();
    }

    [Serializable]
    public sealed class RunSummary
    {
        public bool completed;
        public string districtId = "dock";
        public int bloomDustCollected;
        public int scrapCollected;
        public int cleanWaterCollected;
        public int memoryPearlCollected;
        public int seedPodCollected;
        public int seedPodDelta;
        public int bioPressUseCount;
        public int bioPressCleanWaterConverted;
        public int pickupsCollected;
        public int coreRoutePickupCount;
        public int sideRoutePickupCount;
        public int elevatedRoutePickupCount;
        public int boostPadUseCount;
        public float objectiveReadyAtSeconds;
        public float objectiveReadyGraceSeconds;
        public float durationSeconds;
        public string resultLabel = "No runs yet";
        public ExpeditionObjectiveType objectiveType = ExpeditionObjectiveType.CollectPickups;
        public ResourceType objectiveResourceType = ResourceType.BloomDust;
        public int objectiveTargetAmount;
        public int objectiveProgressAmount;
        public float objectiveTargetSeconds;
        public float objectiveElapsedSeconds;
        public string objectiveDescription = string.Empty;
        public string objectiveProgressText = string.Empty;

        public string GetObjectiveSummary(DistrictContentBundle bundle = null)
        {
            var resolvedType = ResolveObjectiveType(bundle);
            var resolvedResourceType = ResolveObjectiveResourceType(bundle);
            var resolvedTargetAmount = ResolveObjectiveTargetAmount(bundle, resolvedType);
            var resolvedTargetSeconds = ResolveObjectiveTargetSeconds(bundle);
            var description = string.IsNullOrWhiteSpace(objectiveDescription)
                ? GetFallbackObjectiveDescription(bundle, resolvedType, resolvedResourceType, resolvedTargetAmount, resolvedTargetSeconds)
                : objectiveDescription;
            var progress = string.IsNullOrWhiteSpace(objectiveProgressText)
                ? GetFallbackObjectiveProgress(resolvedType, resolvedResourceType, resolvedTargetAmount, resolvedTargetSeconds)
                : objectiveProgressText;

            return $"{description}\n{progress}";
        }

        public ExpeditionObjectiveType ResolveObjectiveType(DistrictContentBundle bundle = null)
        {
            return ShouldUseBundleObjective(bundle) && bundle?.District != null
                ? bundle.District.objectiveType
                : objectiveType;
        }

        public ResourceType ResolveObjectiveResourceType(DistrictContentBundle bundle = null)
        {
            return ShouldUseBundleObjective(bundle) && bundle?.District != null
                ? bundle.District.objectiveResourceType
                : objectiveResourceType;
        }

        public string GetRewardSummary()
        {
            return
                $"BloomDust: {bloomDustCollected}\n" +
                $"Scrap: {scrapCollected}\n" +
                $"SeedPod: {seedPodCollected}\n" +
                $"CleanWater: {cleanWaterCollected}\n" +
                $"MemoryPearl: {memoryPearlCollected}\n" +
                $"SeedPod Delta: {seedPodDelta}\n" +
                $"Bio Press Uses: {bioPressUseCount}\n" +
                $"Bio Press CleanWater: {bioPressCleanWaterConverted}";
        }

        public string GetTraversalTelemetrySummary()
        {
            var dominantRoute = elevatedRoutePickupCount >= sideRoutePickupCount && elevatedRoutePickupCount >= coreRoutePickupCount
                ? "Elevated"
                : sideRoutePickupCount > coreRoutePickupCount
                    ? "SideLane"
                    : "Core";

            return
                $"Core Route Pickups: {coreRoutePickupCount}\n" +
                $"Side Route Pickups: {sideRoutePickupCount}\n" +
                $"Elevated Route Pickups: {elevatedRoutePickupCount}\n" +
                $"Boost Pad Uses: {boostPadUseCount}\n" +
                $"Objective Ready At: {objectiveReadyAtSeconds:0.0}s\n" +
                $"Grace Window: {objectiveReadyGraceSeconds:0.0}s\n" +
                $"Dominant Route: {dominantRoute}";
        }

        public string GetOperationsSummary()
        {
            return
                $"SeedPod Delta: {seedPodDelta}\n" +
                $"Bio Press Uses: {bioPressUseCount}\n" +
                $"Bio Press CleanWater: {bioPressCleanWaterConverted}\n" +
                $"{GetTraversalTelemetrySummary()}";
        }

        private string GetFallbackObjectiveDescription(
            DistrictContentBundle bundle,
            ExpeditionObjectiveType resolvedType,
            ResourceType resolvedResourceType,
            int resolvedTargetAmount,
            float resolvedTargetSeconds)
        {
            var districtName = bundle?.District != null && !string.IsNullOrWhiteSpace(bundle.District.displayName)
                ? bundle.District.displayName
                : districtId;

            switch (resolvedType)
            {
                case ExpeditionObjectiveType.CollectResource:
                    return $"Recover {resolvedTargetAmount} {resolvedResourceType} from {districtName} before returning.";
                case ExpeditionObjectiveType.HoldOut:
                    return $"Hold the route in {districtName} for {resolvedTargetSeconds:0} seconds, then fall back to the beacon.";
                default:
                    return bundle?.Quest != null && !string.IsNullOrWhiteSpace(bundle.Quest.objectiveText)
                        ? bundle.Quest.objectiveText
                        : "Recover enough salvage to activate the beacon.";
            }
        }

        private string GetFallbackObjectiveProgress(
            ExpeditionObjectiveType resolvedType,
            ResourceType resolvedResourceType,
            int resolvedTargetAmount,
            float resolvedTargetSeconds)
        {
            switch (resolvedType)
            {
                case ExpeditionObjectiveType.CollectResource:
                    return $"{resolvedResourceType}: {ResolveObjectiveProgressAmount(resolvedType, resolvedResourceType, resolvedTargetSeconds)} / {resolvedTargetAmount}";
                case ExpeditionObjectiveType.HoldOut:
                    return $"Holdout: {Mathf.Clamp(ResolveElapsedSeconds(resolvedTargetSeconds), 0f, resolvedTargetSeconds):0} / {resolvedTargetSeconds:0}s";
                default:
                    return $"Pickups: {ResolveObjectiveProgressAmount(resolvedType, resolvedResourceType, resolvedTargetSeconds)} / {resolvedTargetAmount}";
            }
        }

        private bool HasObjectiveSnapshotData()
        {
            return !string.IsNullOrWhiteSpace(objectiveDescription)
                || !string.IsNullOrWhiteSpace(objectiveProgressText)
                || objectiveTargetAmount > 0
                || objectiveProgressAmount > 0
                || objectiveTargetSeconds > 0f
                || objectiveElapsedSeconds > 0f;
        }

        private bool ShouldUseBundleObjective(DistrictContentBundle bundle)
        {
            return !HasObjectiveSnapshotData() && bundle?.District != null;
        }

        private int ResolveObjectiveTargetAmount(DistrictContentBundle bundle, ExpeditionObjectiveType resolvedType)
        {
            if (objectiveTargetAmount > 0)
            {
                return objectiveTargetAmount;
            }

            if (bundle?.District != null)
            {
                if (resolvedType == ExpeditionObjectiveType.CollectPickups)
                {
                    return Math.Max(1, bundle.District.targetPickupCount);
                }

                if (bundle.District.objectiveTargetAmount > 0)
                {
                    return bundle.District.objectiveTargetAmount;
                }
            }

            return 1;
        }

        private float ResolveObjectiveTargetSeconds(DistrictContentBundle bundle)
        {
            if (objectiveTargetSeconds > 0f)
            {
                return objectiveTargetSeconds;
            }

            if (bundle?.District != null)
            {
                if (bundle.District.objectiveHoldSeconds > 0f)
                {
                    return bundle.District.objectiveHoldSeconds;
                }

                return Mathf.Max(30f, bundle.District.runTimerSeconds * 0.5f);
            }

            return 1f;
        }

        private int ResolveObjectiveProgressAmount(
            ExpeditionObjectiveType resolvedType,
            ResourceType resolvedResourceType,
            float resolvedTargetSeconds)
        {
            if (objectiveProgressAmount > 0)
            {
                return objectiveProgressAmount;
            }

            switch (resolvedType)
            {
                case ExpeditionObjectiveType.CollectResource:
                    return ResolveCollectedResourceAmount(resolvedResourceType);
                case ExpeditionObjectiveType.HoldOut:
                    return Mathf.RoundToInt(Mathf.Clamp(ResolveElapsedSeconds(resolvedTargetSeconds), 0f, resolvedTargetSeconds));
                default:
                    return pickupsCollected;
            }
        }

        private float ResolveElapsedSeconds(float resolvedTargetSeconds)
        {
            if (objectiveElapsedSeconds > 0f)
            {
                return objectiveElapsedSeconds;
            }

            return durationSeconds > 0f
                ? Mathf.Min(durationSeconds, resolvedTargetSeconds)
                : 0f;
        }

        private int ResolveCollectedResourceAmount(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.BloomDust:
                    return bloomDustCollected;
                case ResourceType.Scrap:
                    return scrapCollected;
                case ResourceType.SeedPod:
                    return seedPodCollected;
                case ResourceType.CleanWater:
                    return cleanWaterCollected;
                case ResourceType.MemoryPearl:
                    return memoryPearlCollected;
                default:
                    return 0;
            }
        }
    }

    [Serializable]
    public sealed class SeedPodTelemetry
    {
        public int seedPodDelta;
        public int bioPressUseCount;
        public int bioPressCleanWaterConverted;

        public void RecordRefinement(int seedPodDelta, int cleanWaterConverted)
        {
            this.seedPodDelta += seedPodDelta;
            bioPressUseCount++;
            bioPressCleanWaterConverted += cleanWaterConverted;
        }

        public void ApplyTo(RunSummary summary)
        {
            if (summary == null)
            {
                return;
            }

            summary.seedPodDelta += seedPodDelta;
            summary.bioPressUseCount += bioPressUseCount;
            summary.bioPressCleanWaterConverted += bioPressCleanWaterConverted;
        }

        public void Reset()
        {
            seedPodDelta = 0;
            bioPressUseCount = 0;
            bioPressCleanWaterConverted = 0;
        }
    }

    public enum TutorialStage
    {
        StartFirstExpedition = 0,
        ReviewFirstResults = 1,
        InstallFirstUpgrade = 2,
        Completed = 3,
    }

    [Serializable]
    public sealed class SerializableDictionary<TKey, TValue>
    {
        public List<TKey> keys = new();
        public List<TValue> values = new();

        public Dictionary<TKey, TValue> ToDictionary()
        {
            var dict = new Dictionary<TKey, TValue>();
            var count = Math.Min(keys.Count, values.Count);
            for (var i = 0; i < count; i++)
            {
                dict[keys[i]] = values[i];
            }

            return dict;
        }

        public void FromDictionary(Dictionary<TKey, TValue> source)
        {
            keys.Clear();
            values.Clear();

            foreach (var pair in source)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
    }
}
