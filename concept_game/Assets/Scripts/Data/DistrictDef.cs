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
        public int bloomPickupCount = 2;
        public int bloomPickupAmount = 10;
        public int scrapPickupCount = 1;
        public int scrapPickupAmount = 4;
        public float pickupSpawnRadius = 8f;
        public Vector3 beaconPosition = new(0f, 0.75f, 8f);
        public Color districtColor = Color.cyan;
    }
}
