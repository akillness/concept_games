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
            var district = CreateDistrict(10f);
            var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);

            var orderedAnchors = ExpeditionPickupSpawnPlanner.BuildOrderedAnchors(plan, district);

            Assert.That(orderedAnchors.Length, Is.GreaterThanOrEqualTo(5));
            var fifthTier = ExpeditionPickupRouteRules.Resolve(orderedAnchors[4], plan, ResourceType.BloomDust, 1).Tier;
            Assert.That(fifthTier, Is.Not.EqualTo(ExpeditionRouteTier.Elevated));
        }

        [Test]
        public void CollectPickupDistricts_KeepObjectiveQuotaOffElevatedRoute()
        {
            for (var districtIndex = 0; districtIndex < ContentPaths.DistrictCount; districtIndex++)
            {
                var bundle = DistrictContentCatalog.LoadByIndex(districtIndex);
                var district = bundle.District;
                if (district.objectiveType != ExpeditionObjectiveType.CollectPickups)
                {
                    continue;
                }

                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);
                var orderedAnchors = ExpeditionPickupSpawnPlanner.BuildOrderedAnchors(plan, district);
                var groundQuota = district.targetPickupCount;

                Assert.That(CountGroundAnchors(plan), Is.GreaterThanOrEqualTo(groundQuota), $"{district.districtId} should have enough first-floor anchors to satisfy its collect-pickups objective.");

                for (var index = 0; index < groundQuota; index++)
                {
                    var tier = ExpeditionPickupRouteRules.Resolve(orderedAnchors[index], plan, ResourceType.BloomDust, 1).Tier;
                    Assert.That(tier, Is.Not.EqualTo(ExpeditionRouteTier.Elevated), $"{district.districtId} should not require elevated pickups before its core objective quota.");
                }
            }
        }

        [Test]
        public void CollectResourceDistricts_KeepObjectiveCriticalPickupSlotsOffElevatedRoute()
        {
            for (var districtIndex = 0; districtIndex < ContentPaths.DistrictCount; districtIndex++)
            {
                var bundle = DistrictContentCatalog.LoadByIndex(districtIndex);
                var district = bundle.District;
                if (district.objectiveType != ExpeditionObjectiveType.CollectResource)
                {
                    continue;
                }

                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);
                var orderedAnchors = ExpeditionPickupSpawnPlanner.BuildOrderedAnchors(plan, district);
                var objectivePickupSlots = ResolveObjectivePickupSlots(district);
                var groundQuota = Mathf.Min(objectivePickupSlots, CountGroundAnchors(plan));

                for (var index = 0; index < groundQuota; index++)
                {
                    var tier = ExpeditionPickupRouteRules.Resolve(orderedAnchors[index], plan, ResourceType.BloomDust, 1).Tier;
                    Assert.That(tier, Is.Not.EqualTo(ExpeditionRouteTier.Elevated), $"{district.districtId} should keep objective-critical pickup slots off the elevated route.");
                }
            }
        }

        [Test]
        public void CreatePlan_ProvidesEnoughGroundAnchorsForLargestCollectPickupObjective()
        {
            var largestCollectPickupTarget = 0;
            for (var districtIndex = 0; districtIndex < ContentPaths.DistrictCount; districtIndex++)
            {
                var district = DistrictContentCatalog.LoadByIndex(districtIndex).District;
                if (district.objectiveType == ExpeditionObjectiveType.CollectPickups)
                {
                    largestCollectPickupTarget = Mathf.Max(largestCollectPickupTarget, district.targetPickupCount);
                }
            }

            var plan = ExpeditionLevelLayoutBuilder.CreatePlan(CreateDistrict(10f));
            Assert.That(CountGroundAnchors(plan), Is.GreaterThanOrEqualTo(largestCollectPickupTarget));
        }

        [Test]
        public void DefaultDistricts_WithSpareGroundCapacity_UseSideLaneBeforeElevatedRoute()
        {
            for (var districtIndex = 0; districtIndex < ContentPaths.DistrictCount; districtIndex++)
            {
                var bundle = DistrictContentCatalog.LoadByIndex(districtIndex);
                var district = bundle.District;
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);
                var orderedAnchors = ExpeditionPickupSpawnPlanner.BuildOrderedAnchors(plan, district);
                var tiers = orderedAnchors
                    .Take(Mathf.Min(CountGroundAnchors(plan), district.bloomPickupCount + district.scrapPickupCount + district.seedPodPickupCount))
                    .Select(anchor => ExpeditionPickupRouteRules.Resolve(anchor, plan, ResourceType.BloomDust, 1).Tier)
                    .ToArray();

                if (!tiers.Contains(ExpeditionRouteTier.SideLane) || !tiers.Contains(ExpeditionRouteTier.Elevated))
                {
                    continue;
                }

                Assert.That(System.Array.IndexOf(tiers, ExpeditionRouteTier.SideLane), Is.LessThan(System.Array.IndexOf(tiers, ExpeditionRouteTier.Elevated)), $"{district.districtId} should expose a first-floor side route before forcing the elevated route.");
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

        private static int CountGroundAnchors(ExpeditionLevelLayoutPlan plan)
        {
            return plan.pickupAnchors.Count(anchor => ExpeditionPickupRouteRules.Resolve(anchor, plan, ResourceType.BloomDust, 1).Tier != ExpeditionRouteTier.Elevated);
        }

        private static int ResolveObjectivePickupSlots(DistrictDef district)
        {
            return district.objectiveResourceType switch
            {
                ResourceType.BloomDust => district.bloomPickupCount,
                ResourceType.Scrap => district.bloomPickupCount + district.scrapPickupCount,
                ResourceType.SeedPod => district.bloomPickupCount + district.scrapPickupCount + district.seedPodPickupCount,
                _ => district.bloomPickupCount + district.scrapPickupCount + district.seedPodPickupCount,
            };
        }
    }
}
