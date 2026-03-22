using MossHarbor.Data;
using MossHarbor.Expedition;
using NUnit.Framework;
using UnityEngine;

namespace MossHarbor.Tests.EditMode
{
    public sealed class ExpeditionLevelLayoutBuilderTests
    {
        [Test]
        public void CreatePlan_ExpandsFootprintBeyondOldGround()
        {
            var district = ScriptableObject.CreateInstance<DistrictDef>();
            district.pickupSpawnRadius = 11f;

            var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);

            Assert.GreaterOrEqual(plan.halfWidth, 20f);
            Assert.GreaterOrEqual(plan.halfLength, 26f);
        }

        [Test]
        public void CreatePlan_ProvidesElevatedPickupAnchors()
        {
            var district = ScriptableObject.CreateInstance<DistrictDef>();
            district.pickupSpawnRadius = 8f;

            var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);

            Assert.GreaterOrEqual(plan.pickupAnchors.Length, 10);
            Assert.Greater(plan.ElevatedAnchorCount, 0);
        }

        [Test]
        public void CreatePlan_PlacesBeaconOnElevatedNorthRoute()
        {
            var district = ScriptableObject.CreateInstance<DistrictDef>();
            district.pickupSpawnRadius = 13f;

            var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);

            Assert.Greater(plan.beaconPosition.y, 1f);
            Assert.Greater(plan.beaconPosition.z, plan.halfLength * 0.7f);
        }
    }
}
