using UnityEngine;

namespace MossHarbor.Data
{
    public static class DifficultyConfig
    {
        public static float TimerMultiplier(DifficultyLevel level)
        {
            switch (level)
            {
                case DifficultyLevel.Easy: return 1.3f;
                case DifficultyLevel.Hard: return 0.8f;
                default: return 1f;
            }
        }

        public static int PickupTargetOffset(DifficultyLevel level)
        {
            switch (level)
            {
                case DifficultyLevel.Easy: return -1;
                case DifficultyLevel.Hard: return 1;
                default: return 0;
            }
        }

        public static float EntryCostMultiplier(DifficultyLevel level)
        {
            switch (level)
            {
                case DifficultyLevel.Easy: return 0.7f;
                case DifficultyLevel.Hard: return 1.3f;
                default: return 1f;
            }
        }

        public static float TwoStarPickupRatioOffset(DifficultyLevel level)
        {
            switch (level)
            {
                case DifficultyLevel.Easy: return -0.10f;
                case DifficultyLevel.Hard: return 0.05f;
                default: return 0f;
            }
        }

        public static float ThreeStarTimeRatioOffset(DifficultyLevel level)
        {
            switch (level)
            {
                case DifficultyLevel.Easy: return 0.10f;
                case DifficultyLevel.Hard: return -0.05f;
                default: return 0f;
            }
        }

        public static float FailResourceRetention(DifficultyLevel level)
        {
            switch (level)
            {
                case DifficultyLevel.Easy: return 0.85f;
                case DifficultyLevel.Hard: return 0.5f;
                default: return 0.7f;
            }
        }

        public static string DisplayName(DifficultyLevel level)
        {
            switch (level)
            {
                case DifficultyLevel.Easy: return "Easy";
                case DifficultyLevel.Hard: return "Hard";
                default: return "Normal";
            }
        }
    }
}
