using UnityEngine;

namespace MossHarbor.Data
{
    [CreateAssetMenu(menuName = "Moss Harbor/Hub Upgrade Definition", fileName = "HubUpgradeDef_")]
    public sealed class HubUpgradeDef : ScriptableObject
    {
        public string upgradeId = "harbor_pump";
        public string displayName = "Harbor Pump";
        [TextArea] public string description;
        public ResourceType costType = ResourceType.Scrap;
        public int costAmount = 15;
        public int entryCostReduction = 0;
        public int cleanWaterBonus = 0;
        public float timerBonusSeconds = 0f;
        public float bloomMultiplier = 1f;
        public int memoryPearlBonus = 0;
    }
}
