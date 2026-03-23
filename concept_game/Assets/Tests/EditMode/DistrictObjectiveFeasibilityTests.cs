using MossHarbor.Data;
using MossHarbor.Expedition;
using NUnit.Framework;
using UnityEngine;

namespace MossHarbor.Tests.EditMode
{
    public sealed class DistrictObjectiveFeasibilityTests
    {
        [Test]
        public void AllDistricts_ObjectiveTargetsFitWithinAvailablePickupBudget()
        {
            for (var districtIndex = 0; districtIndex < ContentPaths.DistrictCount; districtIndex++)
            {
                var district = DistrictContentCatalog.LoadByIndex(districtIndex).District;
                Assert.NotNull(district, $"Missing district for index {districtIndex}");

                var objective = new ObjectiveService(district);
                var totalPickupCount = GetTotalPickupCount(district);

                switch (objective.ObjectiveType)
                {
                    case ExpeditionObjectiveType.CollectResource:
                    {
                        var availableResource = GetAvailableResourceAmount(district, objective.ObjectiveResourceType);
                        Assert.GreaterOrEqual(
                            availableResource,
                            objective.TargetAmount,
                            $"{district.displayName} requires {objective.TargetAmount} {objective.ObjectiveResourceType} but only spawns {availableResource}.");
                        break;
                    }
                    case ExpeditionObjectiveType.CollectPickups:
                    {
                        Assert.GreaterOrEqual(
                            totalPickupCount,
                            objective.TargetAmount,
                            $"{district.displayName} requires {objective.TargetAmount} pickups but only spawns {totalPickupCount}.");
                        break;
                    }
                }
            }
        }

        [Test]
        public void AllDistricts_TargetPickupCountsStayWithinSpawnBudget()
        {
            for (var districtIndex = 0; districtIndex < ContentPaths.DistrictCount; districtIndex++)
            {
                var district = DistrictContentCatalog.LoadByIndex(districtIndex).District;
                Assert.NotNull(district, $"Missing district for index {districtIndex}");

                var totalPickupCount = GetTotalPickupCount(district);
                Assert.GreaterOrEqual(
                    totalPickupCount,
                    district.targetPickupCount,
                    $"{district.displayName} target pickup count exceeds authored pickup supply.");
            }
        }

        [Test]
        public void HoldOutDistricts_ReserveTravelMarginBeforeRunTimerExpires()
        {
            const float MinimumTravelMarginSeconds = 45f;

            for (var districtIndex = 0; districtIndex < ContentPaths.DistrictCount; districtIndex++)
            {
                var district = DistrictContentCatalog.LoadByIndex(districtIndex).District;
                Assert.NotNull(district, $"Missing district for index {districtIndex}");

                var objective = new ObjectiveService(district);
                if (objective.ObjectiveType != ExpeditionObjectiveType.HoldOut)
                {
                    continue;
                }

                var travelMargin = district.runTimerSeconds - objective.TargetHoldSeconds;
                Assert.GreaterOrEqual(
                    travelMargin,
                    MinimumTravelMarginSeconds,
                    $"{district.displayName} leaves only {travelMargin:0.#}s to disengage and return after holdout.");
            }
        }

        private static int GetTotalPickupCount(DistrictDef district)
        {
            return Mathf.Max(0, district.bloomPickupCount)
                   + Mathf.Max(0, district.scrapPickupCount)
                   + Mathf.Max(0, district.seedPodPickupCount);
        }

        private static int GetAvailableResourceAmount(DistrictDef district, ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.BloomDust => Mathf.Max(0, district.bloomPickupCount) * Mathf.Max(0, district.bloomPickupAmount),
                ResourceType.Scrap => Mathf.Max(0, district.scrapPickupCount) * Mathf.Max(0, district.scrapPickupAmount),
                ResourceType.SeedPod => Mathf.Max(0, district.seedPodPickupCount) * Mathf.Max(0, district.seedPodPickupAmount),
                _ => 0,
            };
        }
    }
}
