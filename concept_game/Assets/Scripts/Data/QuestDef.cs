using UnityEngine;

namespace MossHarbor.Data
{
    [CreateAssetMenu(menuName = "Moss Harbor/Quest Definition", fileName = "QuestDef_")]
    public sealed class QuestDef : ScriptableObject
    {
        public string questId = "restore_dock";
        public string districtId = "dock";
        public string displayName = "Restore Dock";
        [TextArea] public string objectiveText;
        public int requiredDistrictStars;
        public ResourceType rewardType = ResourceType.BloomDust;
        public int rewardAmount = 50;
    }
}
