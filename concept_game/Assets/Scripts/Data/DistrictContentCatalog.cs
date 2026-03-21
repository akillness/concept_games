using UnityEngine;

namespace MossHarbor.Data
{
    public static class DistrictContentCatalog
    {
        public static DistrictContentBundle LoadDefault()
        {
            return LoadInternal(
                0,
                ContentPaths.DefaultDistrict,
                ContentPaths.DefaultHubZone,
                ContentPaths.DefaultQuest);
        }

        public static DistrictContentBundle LoadByIndex(int index)
        {
            return LoadInternal(
                index,
                ContentPaths.GetDistrictPath(index),
                ContentPaths.GetHubZonePath(index),
                ContentPaths.GetQuestPath(index));
        }

        public static DistrictContentBundle LoadByDistrictId(string districtId)
        {
            if (!string.IsNullOrWhiteSpace(districtId))
            {
                for (var index = 0; index < ContentPaths.DistrictCount; index++)
                {
                    var bundle = LoadByIndex(index);
                    if (bundle.District != null && bundle.District.districtId == districtId)
                    {
                        return bundle;
                    }
                }
            }

            return LoadDefault();
        }

        private static DistrictContentBundle LoadInternal(int selectionIndex, string districtPath, string hubZonePath, string questPath)
        {
            var district = LoadOrDefault<DistrictDef>(districtPath, ContentPaths.DefaultDistrict);
            var hubZone = LoadOrDefault<HubZoneDef>(hubZonePath, ContentPaths.DefaultHubZone);
            var quest = LoadOrDefault<QuestDef>(questPath, ContentPaths.DefaultQuest);
            var bundle = new DistrictContentBundle(selectionIndex, district, hubZone, quest);

            ValidateBundle(bundle, districtPath, hubZonePath, questPath);
            return bundle;
        }

        private static T LoadOrDefault<T>(string path, string defaultPath) where T : Object
        {
            var asset = Resources.Load<T>(path);
            if (asset == null && path != defaultPath)
            {
                asset = Resources.Load<T>(defaultPath);
            }

            return asset;
        }

        private static void ValidateBundle(DistrictContentBundle bundle, string districtPath, string hubZonePath, string questPath)
        {
            if (bundle.District == null)
            {
                Debug.LogWarning($"DistrictContentCatalog could not load district at '{districtPath}'.");
            }

            if (bundle.HubZone == null)
            {
                Debug.LogWarning($"DistrictContentCatalog could not load hub zone at '{hubZonePath}'.");
            }

            if (bundle.Quest == null)
            {
                Debug.LogWarning($"DistrictContentCatalog could not load quest at '{questPath}'.");
            }

            if (bundle.District == null)
            {
                return;
            }

            if (bundle.HubZone != null && !string.IsNullOrWhiteSpace(bundle.HubZone.districtId) && bundle.HubZone.districtId != bundle.District.districtId)
            {
                Debug.LogWarning(
                    $"DistrictContentCatalog found hub zone mismatch for '{bundle.District.displayName}': " +
                    $"zone districtId '{bundle.HubZone.districtId}' != districtId '{bundle.District.districtId}'.");
            }

            if (bundle.Quest != null && !string.IsNullOrWhiteSpace(bundle.Quest.districtId) && bundle.Quest.districtId != bundle.District.districtId)
            {
                Debug.LogWarning(
                    $"DistrictContentCatalog found quest mismatch for '{bundle.District.displayName}': " +
                    $"quest districtId '{bundle.Quest.districtId}' != districtId '{bundle.District.districtId}'.");
            }
        }
    }
}
