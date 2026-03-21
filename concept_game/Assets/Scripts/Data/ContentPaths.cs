namespace MossHarbor.Data
{
    public static class ContentPaths
    {
        public const string DefaultDistrict = "ScriptableObjects/District_Dock";
        public const string ReedDistrict = "ScriptableObjects/District_ReedFields";
        public const string VaultDistrict = "ScriptableObjects/District_TidalVault";
        public const string NarrowsDistrict = "ScriptableObjects/District_GlassNarrows";
        public const string ArcadeDistrict = "ScriptableObjects/District_SunkenArcade";
        public const string CrownDistrict = "ScriptableObjects/District_LighthouseCrown";

        public const string DefaultHubZone = "ScriptableObjects/Zone_Dock";
        public const string ReedHubZone = "ScriptableObjects/Zone_ReedFields";
        public const string VaultHubZone = "ScriptableObjects/Zone_TidalVault";
        public const string NarrowsHubZone = "ScriptableObjects/Zone_GlassNarrows";
        public const string ArcadeHubZone = "ScriptableObjects/Zone_SunkenArcade";
        public const string CrownHubZone = "ScriptableObjects/Zone_LighthouseCrown";

        public const string DefaultTool = "ScriptableObjects/Tool_Vacuum";
        public const string DefaultQuest = "ScriptableObjects/Quest_RestoreDock";
        public const string ReedQuest = "ScriptableObjects/Quest_ClearReedFields";
        public const string VaultQuest = "ScriptableObjects/Quest_OpenTidalVault";
        public const string NarrowsQuest = "ScriptableObjects/Quest_StabilizeNarrows";
        public const string ArcadeQuest = "ScriptableObjects/Quest_RestoreArcade";
        public const string CrownQuest = "ScriptableObjects/Quest_LightTheCrown";
        public const string HarborPumpUpgrade = "ScriptableObjects/Upgrade_HarborPump";
        public const string RouteScannerUpgrade = "ScriptableObjects/Upgrade_RouteScanner";
        public const string PearlResonatorUpgrade = "ScriptableObjects/Upgrade_PearlResonator";

        private static readonly string[] DistrictPaths =
        {
            DefaultDistrict,
            ReedDistrict,
            VaultDistrict,
            NarrowsDistrict,
            ArcadeDistrict,
            CrownDistrict,
        };

        private static readonly string[] HubZonePaths =
        {
            DefaultHubZone,
            ReedHubZone,
            VaultHubZone,
            NarrowsHubZone,
            ArcadeHubZone,
            CrownHubZone,
        };

        private static readonly string[] QuestPaths =
        {
            DefaultQuest,
            ReedQuest,
            VaultQuest,
            NarrowsQuest,
            ArcadeQuest,
            CrownQuest,
        };

        public static int DistrictCount => DistrictPaths.Length;

        public static string GetDistrictPath(int index)
        {
            if (DistrictPaths.Length == 0)
            {
                return DefaultDistrict;
            }

            if (index < 0)
            {
                return DistrictPaths[0];
            }

            return DistrictPaths[index % DistrictPaths.Length];
        }

        public static string GetHubZonePath(int index)
        {
            if (HubZonePaths.Length == 0)
            {
                return DefaultHubZone;
            }

            if (index < 0)
            {
                return HubZonePaths[0];
            }

            return HubZonePaths[index % HubZonePaths.Length];
        }

        public static string GetQuestPath(int index)
        {
            if (QuestPaths.Length == 0)
            {
                return DefaultQuest;
            }

            if (index < 0)
            {
                return QuestPaths[0];
            }

            return QuestPaths[index % QuestPaths.Length];
        }
    }
}
