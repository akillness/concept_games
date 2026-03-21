using MossHarbor.Core;
using MossHarbor.Data;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public static class RewardCalculator
    {
        public struct CompletionReward
        {
            public int bloomDust;
            public int scrap;
            public int cleanWater;
            public int memoryPearl;
            public float duration;
        }

        public static CompletionReward CalculateSuccess(
            DistrictDef district,
            SaveService save,
            int bloomDustCollected,
            int scrapCollected,
            float remainingTime)
        {
            var bonusBloom = district != null ? district.completionBonusBloomDust : 30;
            var bonusScrap = district != null ? district.completionBonusScrap : 8;

            var harborPumpUpgrade = Resources.Load<HubUpgradeDef>(ContentPaths.HarborPumpUpgrade);
            var routeScannerUpgrade = Resources.Load<HubUpgradeDef>(ContentPaths.RouteScannerUpgrade);
            var pearlResonatorUpgrade = Resources.Load<HubUpgradeDef>(ContentPaths.PearlResonatorUpgrade);

            var cleanWaterBonus = save.GetHubUpgradeLevel(UpgradeIds.HarborPump) > 0 && harborPumpUpgrade != null
                ? harborPumpUpgrade.cleanWaterBonus : 0;
            var routeScannerBloomBonus = save.GetHubUpgradeLevel(UpgradeIds.RouteScanner) > 0 && routeScannerUpgrade != null
                ? Mathf.RoundToInt(bloomDustCollected * Mathf.Max(0f, routeScannerUpgrade.bloomMultiplier - 1f))
                : 0;
            var memoryPearlBonus = save.GetHubUpgradeLevel(UpgradeIds.PearlResonator) > 0 && pearlResonatorUpgrade != null
                && district != null && district.recommendedPower >= 3
                ? pearlResonatorUpgrade.memoryPearlBonus : 0;

            var baseRunTime = district != null ? district.runTimerSeconds : 180f;
            var bonusRunTime = save.GetHubUpgradeLevel(UpgradeIds.RouteScanner) > 0 && routeScannerUpgrade != null
                ? routeScannerUpgrade.timerBonusSeconds : 0f;

            return new CompletionReward
            {
                bloomDust = bloomDustCollected + routeScannerBloomBonus + bonusBloom,
                scrap = scrapCollected + bonusScrap,
                cleanWater = cleanWaterBonus,
                memoryPearl = memoryPearlBonus,
                duration = baseRunTime + bonusRunTime - remainingTime
            };
        }

        public static CompletionReward CalculateFailure(
            DistrictDef district,
            SaveService save,
            int bloomDustCollected,
            int scrapCollected,
            float remainingTime)
        {
            var fallbackBloom = district != null ? district.completionBonusBloomDust : 30;
            var retainedBloomDust = bloomDustCollected + Mathf.RoundToInt(fallbackBloom * 0.5f);
            var retainedScrap = Mathf.RoundToInt(scrapCollected * 0.7f);

            var routeScannerUpgrade = Resources.Load<HubUpgradeDef>(ContentPaths.RouteScannerUpgrade);
            var baseRunTime = district != null ? district.runTimerSeconds : 180f;
            var bonusRunTime = save.GetHubUpgradeLevel(UpgradeIds.RouteScanner) > 0 && routeScannerUpgrade != null
                ? routeScannerUpgrade.timerBonusSeconds : 0f;

            return new CompletionReward
            {
                bloomDust = retainedBloomDust,
                scrap = retainedScrap,
                cleanWater = 0,
                memoryPearl = 0,
                duration = baseRunTime + bonusRunTime - remainingTime
            };
        }
    }
}
