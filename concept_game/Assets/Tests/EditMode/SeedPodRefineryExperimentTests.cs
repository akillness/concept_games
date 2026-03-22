using MossHarbor.Hub;
using NUnit.Framework;

namespace MossHarbor.Tests.EditMode
{
    public sealed class SeedPodRefineryExperimentTests
    {
        [Test]
        public void DefaultPlan_UsesFifteenRuns()
        {
            var plan = SeedPodRefineryExperiment.DefaultQAFifteenRunPlan;

            Assert.AreEqual("qa-15-run-reed-vault-narrows", plan.PlanId);
            Assert.AreEqual(15, plan.Runs);
        }

        [Test]
        public void Simulate_BaselineProfile_ProducesStableMetrics()
        {
            var result = SeedPodRefineryExperiment.Simulate(
                SeedPodRefineryRules.BaselineProfile,
                SeedPodRefineryExperiment.DefaultQAFifteenRunPlan);

            Assert.AreEqual(15, result.Runs);
            Assert.AreEqual(1, result.FinalInventory);
            Assert.AreEqual(9, result.TotalRefines);
            Assert.AreEqual(18, result.TotalCleanWaterConverted);
            Assert.AreEqual(5, result.PeakInventory);
            Assert.AreEqual(0, result.OverflowRuns);
            Assert.Greater(result.TargetBandDistance, 0);
        }

        [Test]
        public void Simulate_FastSinkProfile_ReducesFinalInventoryVersusBaseline()
        {
            var plan = SeedPodRefineryExperiment.DefaultQAFifteenRunPlan;
            var baseline = SeedPodRefineryExperiment.Simulate(SeedPodRefineryRules.CandidateProfiles[0], plan);
            var fastSink = SeedPodRefineryExperiment.Simulate(SeedPodRefineryRules.CandidateProfiles[1], plan);

            Assert.Less(fastSink.FinalInventory, baseline.FinalInventory);
            Assert.Greater(fastSink.TotalCleanWaterConverted, baseline.TotalCleanWaterConverted);
        }

        [Test]
        public void Simulate_HighYieldProfile_IncreasesCleanWaterWithoutChangingInventoryCurve()
        {
            var plan = SeedPodRefineryExperiment.DefaultQAFifteenRunPlan;
            var baseline = SeedPodRefineryExperiment.Simulate(SeedPodRefineryRules.CandidateProfiles[0], plan);
            var highYield = SeedPodRefineryExperiment.Simulate(SeedPodRefineryRules.CandidateProfiles[2], plan);

            Assert.AreEqual(baseline.FinalInventory, highYield.FinalInventory);
            Assert.AreEqual(baseline.PeakInventory, highYield.PeakInventory);
            Assert.AreEqual(baseline.TotalRefines, highYield.TotalRefines);
            Assert.Greater(highYield.TotalCleanWaterConverted, baseline.TotalCleanWaterConverted);
        }

        [Test]
        public void CompareCandidateProfiles_SortsBestCandidatesFirst()
        {
            var results = SeedPodRefineryExperiment.CompareCandidateProfiles(
                SeedPodRefineryExperiment.DefaultQAFifteenRunPlan);

            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("high-yield", results[0].Profile.ProfileId);
            Assert.AreEqual("baseline", results[1].Profile.ProfileId);
            Assert.AreEqual("fast-sink", results[2].Profile.ProfileId);
        }
    }
}
