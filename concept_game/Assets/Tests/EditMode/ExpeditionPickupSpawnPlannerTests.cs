using System.Collections.Generic;
using System.Linq;
using MossHarbor.Data;
using MossHarbor.Expedition;
using NUnit.Framework;
using UnityEngine;

namespace MossHarbor.Tests.EditMode
{
    public sealed class ExpeditionPickupSpawnPlannerTests
    {
        [Test]
        public void BuildOrderedAnchors_UsesSideLaneByFifthPickup()
        {
            var plan = ExpeditionLevelLayoutBuilder.CreatePlan(CreateDistrict(10f));

            var orderedAnchors = ExpeditionPickupSpawnPlanner.BuildOrderedAnchors(plan);

            Assert.That(orderedAnchors.Length, Is.GreaterThanOrEqualTo(5));
            var fifthTier = ExpeditionPickupRouteRules.Resolve(orderedAnchors[4], plan, ResourceType.BloomDust, 1).Tier;
            Assert.That(fifthTier, Is.EqualTo(ExpeditionRouteTier.SideLane));
        }

        [Test]
        public void DefaultDistricts_WithFiveOrMorePickups_UseAtLeastOneSideLaneAnchor()
        {
            for (var districtIndex = 0; districtIndex < ContentPaths.DistrictCount; districtIndex++)
            {
                var bundle = DistrictContentCatalog.LoadByIndex(districtIndex);
                var district = bundle.District;
                var totalPickups = district.bloomPickupCount + district.scrapPickupCount + district.seedPodPickupCount;
                if (totalPickups < 5)
                {
                    continue;
                }

                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);
                var orderedAnchors = ExpeditionPickupSpawnPlanner.BuildOrderedAnchors(plan).Take(totalPickups).ToArray();

                Assert.That(
                    orderedAnchors.Any(anchor => ExpeditionPickupRouteRules.Resolve(anchor, plan, ResourceType.BloomDust, 1).Tier == ExpeditionRouteTier.SideLane),
                    Is.True,
                    $"{district.districtId} should include at least one side-lane pickup.");
            }
        }

        [Test]
        public void DefaultDistricts_DoNotPinAllSeedPodsToElevatedRoute()
        {
            for (var districtIndex = 0; districtIndex < ContentPaths.DistrictCount; districtIndex++)
            {
                var bundle = DistrictContentCatalog.LoadByIndex(districtIndex);
                var district = bundle.District;
                if (district.seedPodPickupCount <= 1)
                {
                    continue;
                }

                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);
                var assignedSeedPodTiers = ResolveAssignedTiers(district, plan)
                    .Where(entry => entry.resourceType == ResourceType.SeedPod)
                    .Select(entry => entry.tier)
                    .ToArray();

                Assert.That(assignedSeedPodTiers.Any(tier => tier != ExpeditionRouteTier.Elevated), Is.True, $"{district.districtId} should keep at least one seed pod off the elevated route.");
            }
        }

        private static DistrictDef CreateDistrict(float pickupSpawnRadius)
        {
            var district = ScriptableObject.CreateInstance<DistrictDef>();
            district.pickupSpawnRadius = pickupSpawnRadius;
            district.bloomPickupCount = 3;
            district.scrapPickupCount = 2;
            district.seedPodPickupCount = 1;
            return district;
        }

        private static IEnumerable<(ResourceType resourceType, ExpeditionRouteTier tier)> ResolveAssignedTiers(DistrictDef district, ExpeditionLevelLayoutPlan plan)
        {
            var orderedAnchors = ExpeditionPickupSpawnPlanner.BuildOrderedAnchors(plan);
            var spawnOrder = new List<ResourceType>();

            for (var index = 0; index < district.bloomPickupCount; index++)
            {
                spawnOrder.Add(ResourceType.BloomDust);
            }

            for (var index = 0; index < district.scrapPickupCount; index++)
            {
                spawnOrder.Add(ResourceType.Scrap);
            }

            for (var index = 0; index < district.seedPodPickupCount; index++)
            {
                spawnOrder.Add(ResourceType.SeedPod);
            }

            for (var index = 0; index < spawnOrder.Count; index++)
            {
                var resourceType = spawnOrder[index];
                var anchor = orderedAnchors[index % orderedAnchors.Length];
                yield return (resourceType, ExpeditionPickupRouteRules.Resolve(anchor, plan, resourceType, 1).Tier);
            }
        }
    }
}
