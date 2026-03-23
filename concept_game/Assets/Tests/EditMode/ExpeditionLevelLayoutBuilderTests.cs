using MossHarbor.Data;
using MossHarbor.Expedition;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

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

        [Test]
        public void Build_CreatesBoostPadSurfaceAndTriggerVolume()
        {
            var root = new GameObject("TestRoot");
            try
            {
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(CreateDistrict(10f));

                ExpeditionLevelLayoutBuilder.Build(root.transform, plan, Color.cyan);

                var pad = root.transform.Find("RuntimeLevelLayout/WestBoostPad");
                var trigger = root.transform.Find("RuntimeLevelLayout/WestBoostPad/WestBoostPadTrigger");
                var pulse = root.transform.Find("RuntimeLevelLayout/WestBoostPad/WestBoostPadPulseVisual");
                var guideRamp = root.transform.Find("RuntimeLevelLayout/WestBoostPad/WestBoostPadGuideRamp");
                var liftPath = root.transform.Find("RuntimeLevelLayout/WestBoostPadLiftPath");
                var exitLanding = root.transform.Find("RuntimeLevelLayout/WestBoostPadExitLanding");
                var deckDecor = root.transform.Find("RuntimeLevelLayout/WestBoostPadDeckDecor");
                var liftDecor = root.transform.Find("RuntimeLevelLayout/WestBoostPadLiftDecor");
                var exitDecor = root.transform.Find("RuntimeLevelLayout/WestBoostPadExitDecor");
                var landingMarker = root.transform.Find("RuntimeLevelLayout/WestBoostPadLandingMarker");

                Assert.That(pad, Is.Not.Null);
                Assert.That(trigger, Is.Not.Null);
                Assert.That(pulse, Is.Not.Null);
                Assert.That(guideRamp, Is.Not.Null);
                Assert.That(liftPath, Is.Not.Null);
                Assert.That(exitLanding, Is.Not.Null);
                Assert.That(deckDecor, Is.Not.Null);
                Assert.That(liftDecor, Is.Not.Null);
                Assert.That(exitDecor, Is.Not.Null);
                Assert.That(landingMarker, Is.Not.Null);
                Assert.That(pad.GetComponent<BoxCollider>(), Is.Not.Null);
                Assert.That(pad.GetComponent<BoxCollider>().isTrigger, Is.False);
                Assert.That(pad.GetComponent<TraversalBoostPad>(), Is.Not.Null);
                Assert.That(trigger.GetComponent<BoxCollider>(), Is.Not.Null);
                Assert.That(trigger.GetComponent<BoxCollider>().isTrigger, Is.True);
                Assert.That(trigger.GetComponent<Rigidbody>(), Is.Not.Null);
                Assert.That(trigger.GetComponent<Rigidbody>().isKinematic, Is.True);
                Assert.That(trigger.GetComponent<TraversalBoostPadTriggerRelay>(), Is.Not.Null);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Build_BridgeSweeper_DoesNotCoverAllElevatedAnchors()
        {
            var root = new GameObject("TestRoot");
            try
            {
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(CreateDistrict(10f));

                ExpeditionLevelLayoutBuilder.Build(root.transform, plan, Color.magenta);

                var pivot = root.transform.Find("RuntimeLevelLayout/BridgeSweeper_Pivot");
                var beam = root.transform.Find("RuntimeLevelLayout/BridgeSweeper");
                Assert.That(pivot, Is.Not.Null);
                Assert.That(beam, Is.Not.Null);

                var influenceRadius = Vector3.Distance(beam.position, pivot.position) + beam.localScale.z * 0.5f;
                var coveredElevatedAnchors = 0;
                foreach (var anchor in plan.pickupAnchors)
                {
                    if (anchor.y <= 1f)
                    {
                        continue;
                    }

                    if (Vector3.Distance(new Vector3(anchor.x, 0f, anchor.z), new Vector3(pivot.position.x, 0f, pivot.position.z)) <= influenceRadius)
                    {
                        coveredElevatedAnchors++;
                    }
                }

                Assert.LessOrEqual(coveredElevatedAnchors, 1);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Build_BridgeSweeper_LeavesBeaconBackEdgeOutsideFootprint()
        {
            var root = new GameObject("TestRoot");
            try
            {
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(CreateDistrict(13f));

                ExpeditionLevelLayoutBuilder.Build(root.transform, plan, Color.yellow);

                var pivot = root.transform.Find("RuntimeLevelLayout/BridgeSweeper_Pivot");
                var beam = root.transform.Find("RuntimeLevelLayout/BridgeSweeper");
                Assert.That(pivot, Is.Not.Null);
                Assert.That(beam, Is.Not.Null);

                var influenceRadius = Vector3.Distance(beam.position, pivot.position) + beam.localScale.z * 0.5f;
                var beaconBackEdge = plan.beaconPosition + Vector3.back * 2.7f;
                var distance = Vector3.Distance(new Vector3(beaconBackEdge.x, 0f, beaconBackEdge.z), new Vector3(pivot.position.x, 0f, pivot.position.z));

                Assert.Greater(distance, influenceRadius);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Build_SweepHazards_UseRecoverableImpulseTuning()
        {
            var root = new GameObject("TestRoot");
            try
            {
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(CreateDistrict(10f));

                ExpeditionLevelLayoutBuilder.Build(root.transform, plan, Color.red);

                var centralSweeper = root.transform.Find("RuntimeLevelLayout/CentralSweeper")?.GetComponent<SweepHazard>();
                var bridgeSweeper = root.transform.Find("RuntimeLevelLayout/BridgeSweeper")?.GetComponent<SweepHazard>();

                Assert.That(centralSweeper, Is.Not.Null);
                Assert.That(bridgeSweeper, Is.Not.Null);
                Assert.LessOrEqual(centralSweeper.PushStrength, 4.5f);
                Assert.LessOrEqual(centralSweeper.VerticalLift, 0.7f);
                Assert.GreaterOrEqual(centralSweeper.RepulseCooldown, 0.7f);
                Assert.LessOrEqual(bridgeSweeper.PushStrength, centralSweeper.PushStrength);
                Assert.LessOrEqual(bridgeSweeper.VerticalLift, centralSweeper.VerticalLift);
                Assert.GreaterOrEqual(bridgeSweeper.RepulseCooldown, centralSweeper.RepulseCooldown);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Build_CreatesTraversalLandingsAndPerimeterRails()
        {
            var root = new GameObject("TestRoot");
            try
            {
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(CreateDistrict(10f));

                ExpeditionLevelLayoutBuilder.Build(root.transform, plan, Color.green);

                Assert.That(root.transform.Find("RuntimeLevelLayout/WestRampBaseLanding"), Is.Not.Null);
                Assert.That(root.transform.Find("RuntimeLevelLayout/WestRampTopLanding"), Is.Not.Null);
                Assert.That(root.transform.Find("RuntimeLevelLayout/EastRampBaseLanding"), Is.Not.Null);
                Assert.That(root.transform.Find("RuntimeLevelLayout/EastRampTopLanding"), Is.Not.Null);
                Assert.That(root.transform.Find("RuntimeLevelLayout/MainGroundWestRail"), Is.Not.Null);
                Assert.That(root.transform.Find("RuntimeLevelLayout/MainGroundEastRail"), Is.Not.Null);
                Assert.That(root.transform.Find("RuntimeLevelLayout/ElevatedDeckNorthRail"), Is.Not.Null);
                Assert.That(root.transform.Find("RuntimeLevelLayout/BeaconPlatformNorthRail"), Is.Not.Null);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        private static DistrictDef CreateDistrict(float pickupSpawnRadius)
        {
            var district = ScriptableObject.CreateInstance<DistrictDef>();
            district.pickupSpawnRadius = pickupSpawnRadius;
            return district;
        }
    }
}
