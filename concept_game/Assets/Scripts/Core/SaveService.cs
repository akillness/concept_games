using System.Collections.Generic;
using System.IO;
using MossHarbor.Data;
using UnityEngine;

namespace MossHarbor.Core
{
    public sealed class SaveService
    {
        private const string SaveFileName = "moss_harbor_save.json";

        public SaveData Current { get; private set; }

        public void Initialize()
        {
            Current = LoadOrCreate();
            EnsureDataDefaults(Current);
        }

        public int GetResource(ResourceType type)
        {
            var resources = Current.resources.ToDictionary();
            return resources.TryGetValue(type, out var amount) ? amount : 0;
        }

        public void AddResource(ResourceType type, int amount)
        {
            var resources = Current.resources.ToDictionary();
            resources[type] = GetResource(type) + amount;
            Current.resources.FromDictionary(resources);
            Save();
        }

        public void RecordSeedPodTelemetry(int seedPodDelta, int cleanWaterConverted)
        {
            Current.seedPodTelemetry.RecordRefinement(seedPodDelta, cleanWaterConverted);
            Current.seedPodTelemetry.ApplyTo(Current.lastRunSummary);
            Current.seedPodTelemetry.Reset();
            Save();
        }

        public void SetDistrictStars(string districtId, int stars)
        {
            var starMap = Current.districtStars.ToDictionary();
            starMap[districtId] = Mathf.Max(stars, starMap.TryGetValue(districtId, out var current) ? current : 0);
            Current.districtStars.FromDictionary(starMap);
            Save();
        }

        public int GetDistrictStars(string districtId)
        {
            var starMap = Current.districtStars.ToDictionary();
            return starMap.TryGetValue(districtId, out var stars) ? stars : 0;
        }

        public int GetTotalDistrictStars()
        {
            var total = 0;
            foreach (var pair in Current.districtStars.ToDictionary())
            {
                total += pair.Value;
            }

            return total;
        }

        public int GetHubUpgradeLevel(string upgradeId)
        {
            var levels = Current.hubUpgradeLevels.ToDictionary();
            return levels.TryGetValue(upgradeId, out var level) ? level : 0;
        }

        public void SetHubUpgradeLevel(string upgradeId, int level)
        {
            var levels = Current.hubUpgradeLevels.ToDictionary();
            levels[upgradeId] = Mathf.Max(level, levels.TryGetValue(upgradeId, out var current) ? current : 0);
            Current.hubUpgradeLevels.FromDictionary(levels);
            Save();
        }

        public bool IsQuestClaimed(string questId)
        {
            var claimed = Current.claimedQuests.ToDictionary();
            return claimed.TryGetValue(questId, out var isClaimed) && isClaimed;
        }

        public bool TryGetHubZoneRestorationState(string zoneId, out bool restored)
        {
            var states = Current.hubZoneRestorationStates.ToDictionary();
            return states.TryGetValue(zoneId, out restored);
        }

        public bool GetHubZoneRestorationState(string zoneId)
        {
            return TryGetHubZoneRestorationState(zoneId, out var restored) && restored;
        }

        public bool IsHubZoneRestored(string zoneId)
        {
            return GetHubZoneRestorationState(zoneId);
        }

        public void SetHubZoneRestorationState(string zoneId, bool restored)
        {
            var states = Current.hubZoneRestorationStates.ToDictionary();
            states[zoneId] = restored;
            Current.hubZoneRestorationStates.FromDictionary(states);
            Save();
        }

        public void SetHubZoneRestored(string zoneId, bool isRestored = true)
        {
            SetHubZoneRestorationState(zoneId, isRestored);
        }

        public void MarkQuestClaimed(string questId)
        {
            var claimed = Current.claimedQuests.ToDictionary();
            claimed[questId] = true;
            Current.claimedQuests.FromDictionary(claimed);
            Save();
        }

        public void SetScene(string sceneName)
        {
            Current.currentScene = sceneName;
            Save();
        }

        public void SetSelectedDistrictIndex(int index)
        {
            Current.selectedDistrictIndex = Mathf.Max(0, index);
            Save();
        }

        public TutorialStage GetTutorialStage()
        {
            return Current.tutorialStage;
        }

        public bool IsTutorialActive()
        {
            return GetTutorialStage() != TutorialStage.Completed;
        }

        public void SetTutorialStage(TutorialStage stage)
        {
            if (Current.tutorialStateInitialized && Current.tutorialStage == stage)
            {
                return;
            }

            Current.tutorialStateInitialized = true;
            Current.tutorialStage = stage;
            Save();
        }

        public void AdvanceTutorialStage(TutorialStage stage)
        {
            if ((int)stage <= (int)GetTutorialStage())
            {
                return;
            }

            SetTutorialStage(stage);
        }

        public void CompleteTutorial()
        {
            SetTutorialStage(TutorialStage.Completed);
        }

        public bool HasRunOutcome()
        {
            return TutorialStateRules.HasMeaningfulProgress(new SaveData
            {
                lastRunSummary = Current.lastRunSummary ?? new RunSummary(),
            });
        }

        public void SetLastRunSummary(RunSummary summary)
        {
            Current.seedPodTelemetry.Reset();
            Current.RecordRunSummary(summary);
            Save();
        }

        public void Save()
        {
            var json = JsonUtility.ToJson(Current, true);
            File.WriteAllText(GetSavePath(), json);
        }

        private SaveData LoadOrCreate()
        {
            var path = GetSavePath();
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var loaded = JsonUtility.FromJson<SaveData>(json);
                if (loaded != null)
                {
                    EnsureDataDefaults(loaded);
                    return loaded;
                }
            }

            var created = CreateDefaultSave();
            var jsonCreated = JsonUtility.ToJson(created, true);
            File.WriteAllText(path, jsonCreated);
            return created;
        }

        private static SaveData CreateDefaultSave()
        {
            var data = new SaveData();
            data.resources.FromDictionary(new Dictionary<ResourceType, int>
            {
                { ResourceType.BloomDust, 0 },
                { ResourceType.Scrap, 0 },
                { ResourceType.SeedPod, 0 },
                { ResourceType.CleanWater, 0 },
                { ResourceType.MemoryPearl, 0 },
            });
            return data;
        }

        private static string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, SaveFileName);
        }

        private static void EnsureDataDefaults(SaveData data)
        {
            data.resources ??= new SerializableDictionary<ResourceType, int>();
            data.districtStars ??= new SerializableDictionary<string, int>();
            data.hubUpgradeLevels ??= new SerializableDictionary<string, int>();
            data.hubZoneRestorationStates ??= new SerializableDictionary<string, bool>();
            data.claimedQuests ??= new SerializableDictionary<string, bool>();
            data.seedPodTelemetry ??= new SeedPodTelemetry();
            data.lastRunSummary ??= new RunSummary();
            data.runHistory ??= new List<RunSummary>();
            EnsureTutorialState(data);
        }

        private static void EnsureTutorialState(SaveData data)
        {
            if (data.tutorialStateInitialized)
            {
                return;
            }

            data.tutorialStage = TutorialStateRules.DetermineInitialStage(data);
            data.tutorialStateInitialized = true;
        }
    }
}
