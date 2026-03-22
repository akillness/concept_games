namespace MossHarbor.Hub
{
    public static class SeedPodRefineryRules
    {
        public const int SeedPodCost = 6;
        public const int CleanWaterGain = 2;

        public static readonly SeedPodRefineProfile[] CandidateProfiles =
        {
            new SeedPodRefineProfile("baseline", 6, 2),
            new SeedPodRefineProfile("fast-sink", 5, 2),
            new SeedPodRefineProfile("high-yield", 6, 3),
        };

        public static SeedPodRefineProfile BaselineProfile => CandidateProfiles[0];

        public static bool CanRefine(int harborPumpLevel, int currentSeedPods)
        {
            return harborPumpLevel > 0 && currentSeedPods >= SeedPodCost;
        }

        public static bool TryRefine(int harborPumpLevel, int currentSeedPods, out int seedPodDelta, out int cleanWaterDelta)
        {
            if (!CanRefine(harborPumpLevel, currentSeedPods))
            {
                seedPodDelta = 0;
                cleanWaterDelta = 0;
                return false;
            }

            seedPodDelta = -SeedPodCost;
            cleanWaterDelta = CleanWaterGain;
            return true;
        }
    }

    public readonly struct SeedPodRefineProfile
    {
        public SeedPodRefineProfile(string profileId, int seedPodCost, int cleanWaterGain)
        {
            ProfileId = profileId;
            SeedPodCost = seedPodCost;
            CleanWaterGain = cleanWaterGain;
        }

        public string ProfileId { get; }
        public int SeedPodCost { get; }
        public int CleanWaterGain { get; }

        public string RatioLabel => $"{SeedPodCost}:{CleanWaterGain}";
    }
}
