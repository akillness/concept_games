using UnityEngine;

namespace MossHarbor.Data
{
    [CreateAssetMenu(menuName = "Moss Harbor/District Definition", fileName = "DistrictDef_")]
    public sealed class DistrictDef : ScriptableObject
    {
        public string districtId = "dock";
        public string displayName = "Dock";
        [TextArea] public string description;
        public int recommendedPower = 1;
        public int requiredStars;
        public int expeditionEntryCost = 10;
        public int targetPickupCount = 3;
        public ExpeditionObjectiveType objectiveType = ExpeditionObjectiveType.CollectPickups;
        public ResourceType objectiveResourceType = ResourceType.BloomDust;
        public int objectiveTargetAmount = 3;
        public float objectiveHoldSeconds = 60f;
        public float runTimerSeconds = 180f;
        public int completionBonusBloomDust = 30;
        public int completionBonusScrap = 8;
        [Header("Boundary Recovery")]
        public BoundaryRecoveryProfile boundaryRecovery = new();
        public int bloomPickupCount = 2;
        public int bloomPickupAmount = 10;
        public int scrapPickupCount = 1;
        public int scrapPickupAmount = 4;
        public float pickupSpawnRadius = 8f;
        public Vector3 beaconPosition = new(0f, 0.75f, 8f);
        public Color districtColor = Color.cyan;

        [Header("Environment")]
        public Color fogColor = new Color(0.55f, 0.72f, 0.78f, 1f);
        public float fogDensity = 0.022f;
        public Color ambientColor = new Color(0.42f, 0.56f, 0.62f, 1f);
        public Color sunColor = new Color(0.82f, 0.92f, 0.96f, 1f);
        public float sunIntensity = 1.35f;

        [Header("Pickup Distribution")]
        public int seedPodPickupCount = 0;
        public int seedPodPickupAmount = 3;

        [Header("Star Rating")]
        public float twoStarPickupRatio = 0.8f;
        public float threeStarTimeRatio = 0.6f;

        public BoundaryRecoveryProfile GetBoundaryRecoveryProfile()
        {
            boundaryRecovery ??= CreateDefaultBoundaryRecoveryProfile();
            return boundaryRecovery;
        }

        public static BoundaryRecoveryProfile CreateDefaultBoundaryRecoveryProfile()
        {
            return new BoundaryRecoveryProfile();
        }
    }

    [System.Serializable]
    public sealed class BoundaryRecoveryProfile
    {
        public Vector3 boundaryCenter = Vector3.zero;
        public Vector2 boundaryHalfExtents = new(180f, 180f);
        public Vector3 safePosition = new(0f, 1f, 0f);
        public float floorY = -12f;
    }
}
