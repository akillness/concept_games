namespace MossHarbor.Hub
{
    public static class SeedPodRefineryRules
    {
        public static readonly SeedPodRefineProfile[] CandidateProfiles =
        {
            new SeedPodRefineProfile("baseline", 6, 2),
            new SeedPodRefineProfile("fast-sink", 5, 2),
            new SeedPodRefineProfile("high-yield", 6, 3),
        };

        public static SeedPodRefineProfile BaselineProfile => CandidateProfiles[0];
        public static int SeedPodCost => BaselineProfile.SeedPodCost;
        public static int CleanWaterGain => BaselineProfile.CleanWaterGain;

        public static bool CanRefine(int harborPumpLevel, int currentSeedPods)
        {
            return CanRefine(harborPumpLevel, currentSeedPods, BaselineProfile);
        }

        public static bool CanRefine(int harborPumpLevel, int currentSeedPods, SeedPodRefineProfile profile)
        {
            return harborPumpLevel > 0 && currentSeedPods >= profile.SeedPodCost;
        }

        public static bool TryRefine(int harborPumpLevel, int currentSeedPods, out int seedPodDelta, out int cleanWaterDelta)
        {
            return TryRefine(harborPumpLevel, currentSeedPods, BaselineProfile, out seedPodDelta, out cleanWaterDelta);
        }

        public static bool TryRefine(
            int harborPumpLevel,
            int currentSeedPods,
            SeedPodRefineProfile profile,
            out int seedPodDelta,
            out int cleanWaterDelta)
        {
            if (!CanRefine(harborPumpLevel, currentSeedPods, profile))
            {
                seedPodDelta = 0;
                cleanWaterDelta = 0;
                return false;
            }

            seedPodDelta = -profile.SeedPodCost;
            cleanWaterDelta = profile.CleanWaterGain;
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
