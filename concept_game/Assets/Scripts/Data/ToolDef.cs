using UnityEngine;

namespace MossHarbor.Data
{
    [CreateAssetMenu(menuName = "Moss Harbor/Tool Definition", fileName = "ToolDef_")]
    public sealed class ToolDef : ScriptableObject
    {
        public string toolId = "vacuum";
        public string displayName = "Vacuum";
        public float cleanPower = 10f;
        public float cooldownSeconds = 0.5f;
        public ResourceType unlockCostType = ResourceType.BloomDust;
        public int unlockCost = 50;
    }
}
