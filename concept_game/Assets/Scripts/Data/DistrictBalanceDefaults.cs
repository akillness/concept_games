using UnityEngine;

namespace MossHarbor.Data
{
    public static class DistrictBalanceDefaults
    {
        public static void ApplyIfDefault(DistrictDef district)
        {
            if (district == null) return;

            switch (district.districtId)
            {
                case "dock":
                    ApplyDock(district);
                    break;
                case "reed_fields":
                    ApplyReedFields(district);
                    break;
                case "tidal_vault":
                    ApplyTidalVault(district);
                    break;
                case "glass_narrows":
                    ApplyGlassNarrows(district);
                    break;
                case "sunken_arcade":
                    ApplySunkenArcade(district);
                    break;
                case "lighthouse_crown":
                    ApplyLighthouseCrown(district);
                    break;
            }
        }

        private static void ApplyDock(DistrictDef d)
        {
            // Tutorial district: generous, easy, cyan/misty
            d.recommendedPower = 1;
            d.requiredStars = 0;
            d.expeditionEntryCost = 8;
            d.runTimerSeconds = 210f;
            d.targetPickupCount = 3;
            d.objectiveType = ExpeditionObjectiveType.CollectPickups;
            d.objectiveTargetAmount = 3;
            d.bloomPickupCount = 3;
            d.bloomPickupAmount = 12;
            d.scrapPickupCount = 1;
            d.scrapPickupAmount = 5;
            d.seedPodPickupCount = 0;
            d.completionBonusBloomDust = 20;
            d.completionBonusScrap = 6;
            d.pickupSpawnRadius = 7f;
            d.beaconPosition = new Vector3(0f, 0.75f, 10f);
            d.districtColor = new Color(0.4f, 0.95f, 0.85f, 1f); // Cyan
            // Environment
            d.fogColor = new Color(0.55f, 0.72f, 0.78f, 1f);
            d.fogDensity = 0.022f;
            d.ambientColor = new Color(0.42f, 0.56f, 0.62f, 1f);
            d.sunColor = new Color(0.82f, 0.92f, 0.96f, 1f);
            d.sunIntensity = 1.35f;
            // Star rating
            d.twoStarPickupRatio = 0.8f;
            d.threeStarTimeRatio = 0.65f;
        }

        private static void ApplyReedFields(DistrictDef d)
        {
            // Wetland: route judgment, first quest branch
            d.recommendedPower = 1;
            d.requiredStars = 1;
            d.expeditionEntryCost = 10;
            d.runTimerSeconds = 185f;
            d.targetPickupCount = 4;
            d.objectiveType = ExpeditionObjectiveType.CollectResource;
            d.objectiveResourceType = ResourceType.SeedPod;
            d.objectiveTargetAmount = 5;
            d.bloomPickupCount = 2;
            d.bloomPickupAmount = 10;
            d.scrapPickupCount = 2;
            d.scrapPickupAmount = 5;
            d.seedPodPickupCount = 2;
            d.seedPodPickupAmount = 3;
            d.completionBonusBloomDust = 30;
            d.completionBonusScrap = 8;
            d.pickupSpawnRadius = 9f;
            d.beaconPosition = new Vector3(3f, 0.75f, 12f);
            d.districtColor = new Color(0.45f, 0.78f, 0.42f, 1f); // Swamp green
            // Environment
            d.fogColor = new Color(0.38f, 0.52f, 0.36f, 1f);
            d.fogDensity = 0.026f;
            d.ambientColor = new Color(0.32f, 0.46f, 0.3f, 1f);
            d.sunColor = new Color(0.72f, 0.86f, 0.68f, 1f);
            d.sunIntensity = 1.25f;
            d.twoStarPickupRatio = 0.75f;
            d.threeStarTimeRatio = 0.6f;
        }

        private static void ApplyTidalVault(DistrictDef d)
        {
            // Sealed storage: CleanWater economy intro
            d.recommendedPower = 2;
            d.requiredStars = 2;
            d.expeditionEntryCost = 15;
            d.runTimerSeconds = 180f;
            d.targetPickupCount = 4;
            d.objectiveType = ExpeditionObjectiveType.CollectResource;
            d.objectiveResourceType = ResourceType.CleanWater;
            d.objectiveTargetAmount = 3;
            d.bloomPickupCount = 2;
            d.bloomPickupAmount = 10;
            d.scrapPickupCount = 2;
            d.scrapPickupAmount = 6;
            d.seedPodPickupCount = 1;
            d.seedPodPickupAmount = 2;
            d.completionBonusBloomDust = 35;
            d.completionBonusScrap = 10;
            d.pickupSpawnRadius = 10f;
            d.beaconPosition = new Vector3(-2f, 0.75f, 14f);
            d.districtColor = new Color(0.28f, 0.58f, 0.82f, 1f); // Deep blue
            // Environment
            d.fogColor = new Color(0.22f, 0.36f, 0.52f, 1f);
            d.fogDensity = 0.018f;
            d.ambientColor = new Color(0.2f, 0.32f, 0.48f, 1f);
            d.sunColor = new Color(0.62f, 0.78f, 0.94f, 1f);
            d.sunIntensity = 1.4f;
            d.twoStarPickupRatio = 0.75f;
            d.threeStarTimeRatio = 0.55f;
        }

        private static void ApplyGlassNarrows(DistrictDef d)
        {
            // Glass channels: MemoryPearl intro, long movement routes
            d.recommendedPower = 3;
            d.requiredStars = 4;
            d.expeditionEntryCost = 22;
            d.runTimerSeconds = 180f;
            d.targetPickupCount = 5;
            d.objectiveType = ExpeditionObjectiveType.HoldOut;
            d.objectiveHoldSeconds = 60f;
            d.objectiveTargetAmount = 1;
            d.bloomPickupCount = 2;
            d.bloomPickupAmount = 12;
            d.scrapPickupCount = 2;
            d.scrapPickupAmount = 7;
            d.seedPodPickupCount = 1;
            d.seedPodPickupAmount = 3;
            d.completionBonusBloomDust = 40;
            d.completionBonusScrap = 12;
            d.pickupSpawnRadius = 11f;
            d.beaconPosition = new Vector3(4f, 0.75f, 15f);
            d.districtColor = new Color(0.72f, 0.85f, 0.92f, 1f); // Crystal ice
            // Environment
            d.fogColor = new Color(0.52f, 0.62f, 0.7f, 1f);
            d.fogDensity = 0.015f;
            d.ambientColor = new Color(0.4f, 0.52f, 0.64f, 1f);
            d.sunColor = new Color(0.88f, 0.92f, 1f, 1f);
            d.sunIntensity = 1.6f;
            d.twoStarPickupRatio = 0.7f;
            d.threeStarTimeRatio = 0.5f;
        }

        private static void ApplySunkenArcade(DistrictDef d)
        {
            // Locked shops: tight spaces, surrounded corruption
            d.recommendedPower = 3;
            d.requiredStars = 6;
            d.expeditionEntryCost = 28;
            d.runTimerSeconds = 165f;
            d.targetPickupCount = 5;
            d.objectiveType = ExpeditionObjectiveType.CollectPickups;
            d.objectiveTargetAmount = 5;
            d.bloomPickupCount = 3;
            d.bloomPickupAmount = 14;
            d.scrapPickupCount = 3;
            d.scrapPickupAmount = 8;
            d.seedPodPickupCount = 1;
            d.seedPodPickupAmount = 3;
            d.completionBonusBloomDust = 50;
            d.completionBonusScrap = 15;
            d.pickupSpawnRadius = 8f;
            d.beaconPosition = new Vector3(-3f, 0.75f, 12f);
            d.districtColor = new Color(0.92f, 0.62f, 0.28f, 1f); // Neon amber
            // Environment
            d.fogColor = new Color(0.32f, 0.24f, 0.18f, 1f);
            d.fogDensity = 0.024f;
            d.ambientColor = new Color(0.28f, 0.22f, 0.18f, 1f);
            d.sunColor = new Color(1f, 0.82f, 0.56f, 1f);
            d.sunIntensity = 1.3f;
            d.twoStarPickupRatio = 0.7f;
            d.threeStarTimeRatio = 0.5f;
        }

        private static void ApplyLighthouseCrown(DistrictDef d)
        {
            // Final: multi-objective, long, complex
            d.recommendedPower = 4;
            d.requiredStars = 8;
            d.expeditionEntryCost = 35;
            d.runTimerSeconds = 150f;
            d.targetPickupCount = 6;
            d.objectiveType = ExpeditionObjectiveType.HoldOut;
            d.objectiveHoldSeconds = 75f;
            d.objectiveTargetAmount = 1;
            d.bloomPickupCount = 3;
            d.bloomPickupAmount = 15;
            d.scrapPickupCount = 3;
            d.scrapPickupAmount = 10;
            d.seedPodPickupCount = 2;
            d.seedPodPickupAmount = 4;
            d.completionBonusBloomDust = 60;
            d.completionBonusScrap = 20;
            d.pickupSpawnRadius = 13f;
            d.beaconPosition = new Vector3(0f, 0.75f, 18f);
            d.districtColor = new Color(0.18f, 0.22f, 0.42f, 1f); // Night indigo
            // Environment
            d.fogColor = new Color(0.14f, 0.16f, 0.28f, 1f);
            d.fogDensity = 0.028f;
            d.ambientColor = new Color(0.12f, 0.16f, 0.26f, 1f);
            d.sunColor = new Color(0.56f, 0.62f, 0.82f, 1f);
            d.sunIntensity = 1.15f;
            d.twoStarPickupRatio = 0.65f;
            d.threeStarTimeRatio = 0.65f;
        }
    }
}
