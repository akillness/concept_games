using UnityEngine;

namespace MossHarbor.Data
{
    public static class StarRatingCalculator
    {
        public static int Calculate(DistrictDef district, RunSummary summary, DifficultyLevel difficulty = DifficultyLevel.Normal)
        {
            if (district == null || summary == null || !summary.completed)
                return 0;

            var stars = 1;

            var totalPickups = district.bloomPickupCount + district.scrapPickupCount + district.seedPodPickupCount;
            var pickupRatio = totalPickups > 0 ? (float)summary.pickupsCollected / totalPickups : 1f;
            var twoStarThreshold = Mathf.Clamp01(district.twoStarPickupRatio + DifficultyConfig.TwoStarPickupRatioOffset(difficulty));
            if (pickupRatio >= twoStarThreshold)
                stars = 2;

            if (stars >= 2)
            {
                var totalTime = district.runTimerSeconds;
                var timeRatio = totalTime > 0f ? summary.durationSeconds / totalTime : 1f;
                var threeStarThreshold = Mathf.Clamp01(district.threeStarTimeRatio + DifficultyConfig.ThreeStarTimeRatioOffset(difficulty));
                if (timeRatio <= threeStarThreshold)
                    stars = 3;
            }

            return stars;
        }
    }
}
