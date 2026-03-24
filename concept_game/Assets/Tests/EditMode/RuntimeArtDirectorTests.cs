using MossHarbor.Art;
using MossHarbor.Data;
using MossHarbor.Expedition;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MossHarbor.Tests.EditMode
{
    public sealed class RuntimeArtDirectorTests
    {
        [Test]
        public void DecorateExpedition_PlacesBackdropOutsideTraversalEnvelope()
        {
            var root = new GameObject("ArtRoot");
            try
            {
                var district = CreateDistrict("dock", 12f);
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);

                RuntimeArtDirector.DecorateExpedition(root.transform, district, plan);

                var artRoot = root.transform.Find("RuntimeExpeditionArt");
                Assert.That(artRoot, Is.Not.Null);

                foreach (Transform child in artRoot)
                {
                    Assert.IsFalse(
                        IsInsideTraversalEnvelope(child.position, plan),
                        $"{child.name} intrudes into the expedition traversal envelope.");
                }
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void DecorateExpedition_CreatesNorthBackdropAndSideSignals()
        {
            var root = new GameObject("ArtRoot");
            try
            {
                var district = CreateDistrict("reed_fields", 10f);
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);

                RuntimeArtDirector.DecorateExpedition(root.transform, district, plan);

                var artRoot = root.transform.Find("RuntimeExpeditionArt");
                var backdropWest = artRoot != null ? artRoot.Find("ExpeditionBackdropWest") : null;
                var backdropEast = artRoot != null ? artRoot.Find("ExpeditionBackdropEast") : null;
                var signalWest = artRoot != null ? artRoot.Find("ExpeditionSignalWest") : null;
                var signalEast = artRoot != null ? artRoot.Find("ExpeditionSignalEast") : null;

                Assert.That(backdropWest, Is.Not.Null);
                Assert.That(backdropEast, Is.Not.Null);
                Assert.That(signalWest, Is.Not.Null);
                Assert.That(signalEast, Is.Not.Null);
                Assert.Greater(backdropWest.position.z, plan.beaconPosition.z + 2f);
                Assert.Greater(backdropEast.position.z, plan.beaconPosition.z + 1f);
                Assert.Greater(Mathf.Abs(backdropWest.position.x), plan.halfWidth * 0.75f);
                Assert.Greater(Mathf.Abs(backdropEast.position.x), plan.halfWidth * 0.75f);
                Assert.Greater(Mathf.Abs(signalWest.position.x), plan.halfWidth * 0.65f);
                Assert.Greater(Mathf.Abs(signalEast.position.x), plan.halfWidth * 0.65f);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        private static DistrictDef CreateDistrict(string districtId, float pickupSpawnRadius)
        {
            var district = ScriptableObject.CreateInstance<DistrictDef>();
            district.districtId = districtId;
            district.pickupSpawnRadius = pickupSpawnRadius;
            district.districtColor = new Color(0.4f, 0.95f, 0.85f, 1f);
            return district;
        }

        private static bool IsInsideTraversalEnvelope(Vector3 position, ExpeditionLevelLayoutPlan plan)
        {
            if (Mathf.Abs(position.x) <= plan.halfWidth * 0.62f
                && position.z >= -plan.halfLength * 0.18f
                && position.z <= plan.beaconPosition.z + 1.2f)
            {
                return true;
            }

            var rampChannelOffset = Mathf.Abs(Mathf.Abs(position.x) - plan.halfWidth * 0.18f);
            if (rampChannelOffset <= 4f
                && position.z >= plan.halfLength * 0.16f
                && position.z <= plan.halfLength * 0.62f)
            {
                return true;
            }

            var boostChannelOffset = Mathf.Abs(Mathf.Abs(position.x) - plan.halfWidth * 0.44f);
            return boostChannelOffset <= 2.8f
                   && position.z >= -plan.halfLength * 0.02f
                   && position.z <= plan.halfLength * 0.48f;
        }
    }
}
