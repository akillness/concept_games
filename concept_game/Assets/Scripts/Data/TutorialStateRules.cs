using System.Collections.Generic;

namespace MossHarbor.Data
{
    public static class TutorialStateRules
    {
        public static TutorialStage DetermineInitialStage(SaveData data)
        {
            return HasMeaningfulProgress(data) ? TutorialStage.Completed : TutorialStage.StartFirstExpedition;
        }

        public static bool HasMeaningfulProgress(SaveData data)
        {
            if (data == null)
            {
                return false;
            }

            if (data.selectedDistrictIndex > 0)
            {
                return true;
            }

            return HasPositiveValue(data.resources)
                || HasPositiveValue(data.districtStars)
                || HasPositiveValue(data.hubUpgradeLevels)
                || HasTrueValue(data.hubZoneRestorationStates)
                || HasTrueValue(data.claimedQuests)
                || HasMeaningfulRunSummary(data.lastRunSummary);
        }

        private static bool HasPositiveValue(SerializableDictionary<ResourceType, int> values)
        {
            return values != null && HasPositiveValue(values.ToDictionary());
        }

        private static bool HasPositiveValue(SerializableDictionary<string, int> values)
        {
            return values != null && HasPositiveValue(values.ToDictionary());
        }

        private static bool HasTrueValue(SerializableDictionary<string, bool> values)
        {
            if (values == null)
            {
                return false;
            }

            foreach (var pair in values.ToDictionary())
            {
                if (pair.Value)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasPositiveValue<T>(Dictionary<T, int> values)
        {
            if (values == null)
            {
                return false;
            }

            foreach (var pair in values)
            {
                if (pair.Value > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasMeaningfulRunSummary(RunSummary summary)
        {
            if (summary == null)
            {
                return false;
            }

            return summary.completed
                || summary.bloomDustCollected > 0
                || summary.scrapCollected > 0
                || summary.cleanWaterCollected > 0
                || summary.memoryPearlCollected > 0
                || summary.pickupsCollected > 0
                || summary.durationSeconds > 0f
                || summary.resultLabel != "No runs yet";
        }
    }
}
