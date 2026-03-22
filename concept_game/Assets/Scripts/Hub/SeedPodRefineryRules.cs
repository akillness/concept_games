namespace MossHarbor.Hub
{
    public static class SeedPodRefineryRules
    {
        public const int SeedPodCost = 6;
        public const int CleanWaterGain = 2;

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
}
