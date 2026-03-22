using System.Collections.Generic;
using MossHarbor.Data;
using MossHarbor.Hub;
using NUnit.Framework;
using UnityEngine;

namespace MossHarbor.Tests.EditMode
{
    public sealed class SerializableDictionaryTests
    {
        [Test]
        public void RoundTrip_PreservesValues()
        {
            var data = new SerializableDictionary<ResourceType, int>();
            data.FromDictionary(new Dictionary<ResourceType, int>
            {
                { ResourceType.BloomDust, 10 },
                { ResourceType.Scrap, 3 },
            });

            var roundTrip = data.ToDictionary();

            Assert.AreEqual(10, roundTrip[ResourceType.BloomDust]);
            Assert.AreEqual(3, roundTrip[ResourceType.Scrap]);
        }

        [Test]
        public void RunSummary_DefaultLabel_IsNotEmpty()
        {
            var summary = new RunSummary();

            Assert.IsFalse(string.IsNullOrWhiteSpace(summary.resultLabel));
        }

        [Test]
        public void RunSummary_DefaultDistrictId_IsNotEmpty()
        {
            var summary = new RunSummary();

            Assert.IsFalse(string.IsNullOrWhiteSpace(summary.districtId));
        }

        [Test]
        public void RunSummary_DefaultSeedPodTelemetry_IsZero()
        {
            var summary = new RunSummary();

            Assert.AreEqual(0, summary.seedPodDelta);
            Assert.AreEqual(0, summary.bioPressUseCount);
            Assert.AreEqual(0, summary.bioPressCleanWaterConverted);
        }

        [Test]
        public void SeedPodTelemetry_RecordRefinement_AccumulatesAndResets()
        {
            var telemetry = new SeedPodTelemetry();
            telemetry.RecordRefinement(-6, 2);
            telemetry.RecordRefinement(-5, 3);

            var summary = new RunSummary();
            telemetry.ApplyTo(summary);

            Assert.AreEqual(-11, summary.seedPodDelta);
            Assert.AreEqual(2, summary.bioPressUseCount);
            Assert.AreEqual(5, summary.bioPressCleanWaterConverted);

            telemetry.Reset();
            Assert.AreEqual(0, telemetry.seedPodDelta);
            Assert.AreEqual(0, telemetry.bioPressUseCount);
            Assert.AreEqual(0, telemetry.bioPressCleanWaterConverted);
        }

        [Test]
        public void SeedPodRefineryRules_ExposesExperimentRatioProfiles()
        {
            Assert.AreEqual(3, SeedPodRefineryRules.CandidateProfiles.Length);
            Assert.AreEqual("baseline", SeedPodRefineryRules.CandidateProfiles[0].ProfileId);
            Assert.AreEqual("6:2", SeedPodRefineryRules.CandidateProfiles[0].RatioLabel);
            Assert.AreEqual("fast-sink", SeedPodRefineryRules.CandidateProfiles[1].ProfileId);
            Assert.AreEqual("5:2", SeedPodRefineryRules.CandidateProfiles[1].RatioLabel);
            Assert.AreEqual("high-yield", SeedPodRefineryRules.CandidateProfiles[2].ProfileId);
            Assert.AreEqual("6:3", SeedPodRefineryRules.CandidateProfiles[2].RatioLabel);
        }

        [Test]
        public void RunSummary_GetObjectiveSummary_UsesStoredObjectiveStrings()
        {
            var summary = new RunSummary
            {
                objectiveDescription = "Recover 12 Scrap from Reed Fields before returning.",
                objectiveProgressText = "Scrap: 6 / 12",
            };

            Assert.AreEqual(
                "Recover 12 Scrap from Reed Fields before returning.\nScrap: 6 / 12",
                summary.GetObjectiveSummary());
        }

        [Test]
        public void RunSummary_GetTraversalTelemetrySummary_ReportsDominantRouteAndBoostUsage()
        {
            var summary = new RunSummary
            {
                coreRoutePickupCount = 1,
                sideRoutePickupCount = 3,
                elevatedRoutePickupCount = 2,
                boostPadUseCount = 4,
                objectiveReadyAtSeconds = 28.5f,
                objectiveReadyGraceSeconds = 2.25f,
            };

            var text = summary.GetTraversalTelemetrySummary();

            StringAssert.Contains("Core Route Pickups: 1", text);
            StringAssert.Contains("Side Route Pickups: 3", text);
            StringAssert.Contains("Elevated Route Pickups: 2", text);
            StringAssert.Contains("Boost Pad Uses: 4", text);
            StringAssert.Contains("Objective Ready At: 28.5s", text);
            StringAssert.Contains("Grace Window: 2.3s", text);
            StringAssert.Contains("Dominant Route: SideLane", text);
        }

        [Test]
        public void RunSummary_GetOperationsSummary_CombinesSeedPodAndTraversalTelemetry()
        {
            var summary = new RunSummary
            {
                seedPodDelta = -6,
                bioPressUseCount = 1,
                bioPressCleanWaterConverted = 2,
                coreRoutePickupCount = 0,
                sideRoutePickupCount = 2,
                elevatedRoutePickupCount = 1,
                boostPadUseCount = 3,
                objectiveReadyAtSeconds = 31.2f,
                objectiveReadyGraceSeconds = 2.25f,
            };

            var text = summary.GetOperationsSummary();

            StringAssert.Contains("SeedPod Delta: -6", text);
            StringAssert.Contains("Bio Press Uses: 1", text);
            StringAssert.Contains("Bio Press CleanWater: 2", text);
            StringAssert.Contains("Side Route Pickups: 2", text);
            StringAssert.Contains("Boost Pad Uses: 3", text);
        }

        [Test]
        public void SaveData_RecordRunSummary_AppendsSnapshotsAndTrimsHistory()
        {
            var data = new SaveData();

            for (var i = 0; i < SaveData.RunHistoryLimit + 3; i++)
            {
                data.RecordRunSummary(new RunSummary
                {
                    districtId = i % 2 == 0 ? "dock" : "reed",
                    resultLabel = $"Run {i}",
                    boostPadUseCount = i,
                });
            }

            Assert.AreEqual(SaveData.RunHistoryLimit, data.runHistory.Count);
            Assert.AreEqual("Run 14", data.lastRunSummary.resultLabel);
            Assert.AreEqual("Run 3", data.runHistory[0].resultLabel);
            Assert.AreEqual("Run 14", data.runHistory[data.runHistory.Count - 1].resultLabel);
        }

        [Test]
        public void SaveData_GetDistrictOperationsComparisonSummary_AggregatesRecentDistrictRuns()
        {
            var data = new SaveData();
            data.RecordRunSummary(new RunSummary
            {
                districtId = "dock",
                completed = true,
                seedPodDelta = -6,
                bioPressCleanWaterConverted = 2,
                boostPadUseCount = 3,
                objectiveReadyAtSeconds = 30f,
                sideRoutePickupCount = 3,
                elevatedRoutePickupCount = 1,
            });
            data.RecordRunSummary(new RunSummary
            {
                districtId = "reed",
                completed = false,
                seedPodDelta = -2,
                bioPressCleanWaterConverted = 1,
                boostPadUseCount = 1,
                objectiveReadyAtSeconds = 0f,
                coreRoutePickupCount = 2,
            });
            data.RecordRunSummary(new RunSummary
            {
                districtId = "dock",
                completed = true,
                seedPodDelta = -4,
                bioPressCleanWaterConverted = 3,
                boostPadUseCount = 5,
                objectiveReadyAtSeconds = 24f,
                elevatedRoutePickupCount = 4,
            });

            var text = data.GetDistrictOperationsComparisonSummary("dock");

            StringAssert.Contains("Recent Runs: 2", text);
            StringAssert.Contains("Completed: 2/2", text);
            StringAssert.Contains("Avg SeedPod Delta: -5.0", text);
            StringAssert.Contains("Avg Bio Press Water: 2.5", text);
            StringAssert.Contains("Avg Boost Uses: 4.0", text);
            StringAssert.Contains("Avg Objective Ready: 27.0s", text);
            StringAssert.Contains("Dominant Route: Elevated", text);
        }

        [Test]
        public void RunSummary_GetObjectiveSummary_FallsBackToHoldoutFormatting()
        {
            var summary = new RunSummary
            {
                objectiveType = ExpeditionObjectiveType.HoldOut,
                objectiveTargetSeconds = 5f,
                objectiveElapsedSeconds = 5f,
            };

            var text = summary.GetObjectiveSummary();

            StringAssert.StartsWith("Hold the route", text);
            StringAssert.Contains("Holdout: 5 / 5s", text);
        }

        [Test]
        public void RunSummary_GetObjectiveSummary_UsesBundleForLegacyResourceObjective()
        {
            var district = ScriptableObject.CreateInstance<DistrictDef>();
            district.displayName = "Reed Fields";
            district.objectiveType = ExpeditionObjectiveType.CollectResource;
            district.objectiveResourceType = ResourceType.Scrap;
            district.objectiveTargetAmount = 12;

            var summary = new RunSummary
            {
                districtId = "reed_fields",
                scrapCollected = 6,
            };

            Assert.AreEqual(
                "Recover 12 Scrap from Reed Fields before returning.\nScrap: 6 / 12",
                summary.GetObjectiveSummary(new DistrictContentBundle(1, district, null, null)));
        }

        [Test]
        public void RunSummary_GetObjectiveSummary_UsesBundleForLegacyHoldoutObjective()
        {
            var district = ScriptableObject.CreateInstance<DistrictDef>();
            district.displayName = "Tidal Vault";
            district.objectiveType = ExpeditionObjectiveType.HoldOut;
            district.objectiveHoldSeconds = 15f;

            var summary = new RunSummary
            {
                districtId = "tidal_vault",
                durationSeconds = 9.6f,
            };

            Assert.AreEqual(
                "Hold the route in Tidal Vault for 15 seconds, then fall back to the beacon.\nHoldout: 10 / 15s",
                summary.GetObjectiveSummary(new DistrictContentBundle(2, district, null, null)));
        }

        [Test]
        public void ContentPaths_GetDistrictPath_ClampsAndWraps()
        {
            Assert.AreEqual(ContentPaths.DefaultDistrict, ContentPaths.GetDistrictPath(-1));
            Assert.AreEqual(ContentPaths.DefaultDistrict, ContentPaths.GetDistrictPath(0));
            Assert.AreEqual(ContentPaths.ReedDistrict, ContentPaths.GetDistrictPath(1));
            Assert.AreEqual(ContentPaths.VaultDistrict, ContentPaths.GetDistrictPath(2));
            Assert.AreEqual(ContentPaths.DefaultDistrict, ContentPaths.GetDistrictPath(6));
        }

        [Test]
        public void ContentPaths_GetHubZonePath_ClampsAndWraps()
        {
            Assert.AreEqual(ContentPaths.DefaultHubZone, ContentPaths.GetHubZonePath(-1));
            Assert.AreEqual(ContentPaths.DefaultHubZone, ContentPaths.GetHubZonePath(0));
            Assert.AreEqual(ContentPaths.ReedHubZone, ContentPaths.GetHubZonePath(1));
            Assert.AreEqual(ContentPaths.VaultHubZone, ContentPaths.GetHubZonePath(2));
            Assert.AreEqual(ContentPaths.DefaultHubZone, ContentPaths.GetHubZonePath(6));
        }

        [Test]
        public void ContentPaths_GetQuestPath_ClampsAndWraps()
        {
            Assert.AreEqual(ContentPaths.DefaultQuest, ContentPaths.GetQuestPath(-1));
            Assert.AreEqual(ContentPaths.DefaultQuest, ContentPaths.GetQuestPath(0));
            Assert.AreEqual(ContentPaths.ReedQuest, ContentPaths.GetQuestPath(1));
            Assert.AreEqual(ContentPaths.VaultQuest, ContentPaths.GetQuestPath(2));
            Assert.AreEqual(ContentPaths.NarrowsQuest, ContentPaths.GetQuestPath(3));
            Assert.AreEqual(ContentPaths.ArcadeQuest, ContentPaths.GetQuestPath(4));
            Assert.AreEqual(ContentPaths.CrownQuest, ContentPaths.GetQuestPath(5));
            Assert.AreEqual(ContentPaths.DefaultQuest, ContentPaths.GetQuestPath(6));
        }

        [Test]
        public void ContentPaths_GetDistrictPath_WrapsAcrossAllDistricts()
        {
            Assert.AreEqual(ContentPaths.DefaultDistrict, ContentPaths.GetDistrictPath(0));
            Assert.AreEqual(ContentPaths.ReedDistrict, ContentPaths.GetDistrictPath(1));
            Assert.AreEqual(ContentPaths.VaultDistrict, ContentPaths.GetDistrictPath(2));
            Assert.AreEqual(ContentPaths.NarrowsDistrict, ContentPaths.GetDistrictPath(3));
            Assert.AreEqual(ContentPaths.ArcadeDistrict, ContentPaths.GetDistrictPath(4));
            Assert.AreEqual(ContentPaths.CrownDistrict, ContentPaths.GetDistrictPath(5));
            Assert.AreEqual(ContentPaths.DefaultDistrict, ContentPaths.GetDistrictPath(6));
        }

        [Test]
        public void SeedPodRefineryRules_CanRefine_ReturnsFalse_WhenHarborPumpNotInstalled()
        {
            Assert.IsFalse(SeedPodRefineryRules.CanRefine(0, 999));
        }

        [Test]
        public void SeedPodRefineryRules_CanRefine_ReturnsFalse_WhenSeedPodsAreInsufficient()
        {
            Assert.IsFalse(SeedPodRefineryRules.CanRefine(1, SeedPodRefineryRules.SeedPodCost - 1));
        }

        [Test]
        public void SeedPodRefineryRules_CanRefine_ReturnsTrue_WhenRequirementsAreMet()
        {
            Assert.IsTrue(SeedPodRefineryRules.CanRefine(1, SeedPodRefineryRules.SeedPodCost));
        }

        [Test]
        public void SeedPodRefineryRules_TryRefine_ReturnsExpectedResourceDelta()
        {
            var success = SeedPodRefineryRules.TryRefine(1, SeedPodRefineryRules.SeedPodCost, out var seedPodDelta, out var cleanWaterDelta);

            Assert.IsTrue(success);
            Assert.AreEqual(-SeedPodRefineryRules.SeedPodCost, seedPodDelta);
            Assert.AreEqual(SeedPodRefineryRules.CleanWaterGain, cleanWaterDelta);
        }
    }
}
