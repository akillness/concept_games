using System;
using System.Linq;

namespace MossHarbor.Hub
{
    public readonly struct SeedPodRefineryExperimentPlan
    {
        public SeedPodRefineryExperimentPlan(
            string planId,
            int[] seedPodsPerRun,
            int startingInventory = 0,
            int maxRefinesPerRun = 1,
            int targetInventoryMin = 20,
            int targetInventoryMax = 30,
            int overflowThreshold = 50)
        {
            PlanId = string.IsNullOrWhiteSpace(planId) ? "unnamed-plan" : planId;
            SeedPodsPerRun = seedPodsPerRun ?? Array.Empty<int>();
            StartingInventory = Math.Max(0, startingInventory);
            MaxRefinesPerRun = Math.Max(0, maxRefinesPerRun);
            TargetInventoryMin = Math.Max(0, targetInventoryMin);
            TargetInventoryMax = Math.Max(TargetInventoryMin, targetInventoryMax);
            OverflowThreshold = Math.Max(TargetInventoryMax, overflowThreshold);
        }

        public string PlanId { get; }
        public int[] SeedPodsPerRun { get; }
        public int StartingInventory { get; }
        public int MaxRefinesPerRun { get; }
        public int TargetInventoryMin { get; }
        public int TargetInventoryMax { get; }
        public int OverflowThreshold { get; }
        public int Runs => SeedPodsPerRun?.Length ?? 0;

        public static SeedPodRefineryExperimentPlan CreateQAFifteenRunPlan()
        {
            return new SeedPodRefineryExperimentPlan(
                planId: "qa-15-run-reed-vault-narrows",
                seedPodsPerRun: new[] { 6, 2, 3, 6, 2, 3, 6, 2, 3, 6, 2, 3, 6, 2, 3 },
                maxRefinesPerRun: 1,
                targetInventoryMin: 20,
                targetInventoryMax: 30,
                overflowThreshold: 50);
        }
    }

    public readonly struct SeedPodRefineryExperimentResult
    {
        public SeedPodRefineryExperimentResult(
            SeedPodRefineProfile profile,
            string planId,
            int runs,
            int finalInventory,
            float averageInventory,
            int peakInventory,
            int totalRefines,
            int totalCleanWaterConverted,
            int inTargetBandRuns,
            int overflowRuns,
            int targetBandDistance)
        {
            Profile = profile;
            PlanId = planId;
            Runs = runs;
            FinalInventory = finalInventory;
            AverageInventory = averageInventory;
            PeakInventory = peakInventory;
            TotalRefines = totalRefines;
            TotalCleanWaterConverted = totalCleanWaterConverted;
            InTargetBandRuns = inTargetBandRuns;
            OverflowRuns = overflowRuns;
            TargetBandDistance = targetBandDistance;
        }

        public SeedPodRefineProfile Profile { get; }
        public string PlanId { get; }
        public int Runs { get; }
        public int FinalInventory { get; }
        public float AverageInventory { get; }
        public int PeakInventory { get; }
        public int TotalRefines { get; }
        public int TotalCleanWaterConverted { get; }
        public int InTargetBandRuns { get; }
        public int OverflowRuns { get; }
        public int TargetBandDistance { get; }

        public string ToSummaryLine()
        {
            return $"{Profile.ProfileId} ({Profile.RatioLabel}) | runs={Runs} | final={FinalInventory} | avg={AverageInventory:0.0} | peak={PeakInventory} | refines={TotalRefines} | water={TotalCleanWaterConverted} | bandHits={InTargetBandRuns} | overflow={OverflowRuns} | distance={TargetBandDistance}";
        }
    }

    public static class SeedPodRefineryExperiment
    {
        public static SeedPodRefineryExperimentPlan DefaultQAFifteenRunPlan => SeedPodRefineryExperimentPlan.CreateQAFifteenRunPlan();

        public static SeedPodRefineryExperimentResult Simulate(
            SeedPodRefineProfile profile,
            SeedPodRefineryExperimentPlan plan,
            int harborPumpLevel = 1)
        {
            var inventory = plan.StartingInventory;
            var totalInventory = 0;
            var peakInventory = inventory;
            var totalRefines = 0;
            var totalCleanWaterConverted = 0;
            var inTargetBandRuns = 0;
            var overflowRuns = 0;
            var targetBandDistance = 0;

            var runs = plan.SeedPodsPerRun ?? Array.Empty<int>();
            for (var i = 0; i < runs.Length; i++)
            {
                inventory += Math.Max(0, runs[i]);

                var remainingRefines = plan.MaxRefinesPerRun;
                while (remainingRefines > 0 &&
                       SeedPodRefineryRules.TryRefine(harborPumpLevel, inventory, profile, out var seedPodDelta, out var cleanWaterDelta))
                {
                    inventory += seedPodDelta;
                    totalRefines++;
                    totalCleanWaterConverted += cleanWaterDelta;
                    remainingRefines--;
                }

                peakInventory = Math.Max(peakInventory, inventory);
                totalInventory += inventory;
                if (inventory >= plan.TargetInventoryMin && inventory <= plan.TargetInventoryMax)
                {
                    inTargetBandRuns++;
                }

                if (inventory > plan.OverflowThreshold)
                {
                    overflowRuns++;
                }

                targetBandDistance += DistanceFromTargetBand(inventory, plan.TargetInventoryMin, plan.TargetInventoryMax);
            }

            var averageInventory = runs.Length > 0 ? (float)totalInventory / runs.Length : 0f;
            return new SeedPodRefineryExperimentResult(
                profile,
                plan.PlanId,
                runs.Length,
                inventory,
                averageInventory,
                peakInventory,
                totalRefines,
                totalCleanWaterConverted,
                inTargetBandRuns,
                overflowRuns,
                targetBandDistance);
        }

        public static SeedPodRefineryExperimentResult[] CompareCandidateProfiles(
            SeedPodRefineryExperimentPlan plan,
            int harborPumpLevel = 1)
        {
            return SeedPodRefineryRules.CandidateProfiles
                .Select(profile => Simulate(profile, plan, harborPumpLevel))
                .OrderBy(result => result.OverflowRuns)
                .ThenBy(result => result.TargetBandDistance)
                .ThenBy(result => result.FinalInventory)
                .ThenByDescending(result => result.TotalCleanWaterConverted)
                .ToArray();
        }

        public static string BuildComparisonReport(
            SeedPodRefineryExperimentPlan plan,
            int harborPumpLevel = 1)
        {
            return string.Join("\n", CompareCandidateProfiles(plan, harborPumpLevel)
                .Select(result => result.ToSummaryLine()));
        }

        private static int DistanceFromTargetBand(int inventory, int min, int max)
        {
            if (inventory < min)
            {
                return min - inventory;
            }

            if (inventory > max)
            {
                return inventory - max;
            }

            return 0;
        }
    }
}
