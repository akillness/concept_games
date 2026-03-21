namespace MossHarbor.Data
{
    public sealed class DistrictContentBundle
    {
        public DistrictContentBundle(int selectionIndex, DistrictDef district, HubZoneDef hubZone, QuestDef quest)
        {
            SelectionIndex = selectionIndex;
            District = district;
            HubZone = hubZone;
            Quest = quest;
        }

        public int SelectionIndex { get; }
        public DistrictDef District { get; }
        public HubZoneDef HubZone { get; }
        public QuestDef Quest { get; }

        public string DistrictId
        {
            get
            {
                if (District != null && !string.IsNullOrWhiteSpace(District.districtId))
                {
                    return District.districtId;
                }

                if (HubZone != null && !string.IsNullOrWhiteSpace(HubZone.zoneId))
                {
                    return HubZone.zoneId;
                }

                return Quest != null ? Quest.districtId : string.Empty;
            }
        }
    }
}
