using UnityEngine;

namespace MossHarbor.Data
{
    [CreateAssetMenu(menuName = "Moss Harbor/Hub Zone Definition", fileName = "HubZoneDef_")]
    public sealed class HubZoneDef : ScriptableObject
    {
        public string zoneId = "dock";
        public string districtId = "dock";
        public string displayName = "Sunken Dock";
        [TextArea] public string description;
        public bool defaultRestored;
    }
}
