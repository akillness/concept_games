using MossHarbor.Core;
using MossHarbor.Data;
using MossHarbor.Expedition;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MossHarbor.Art
{
    public static class RuntimeArtDirector
    {
        private static readonly Color HubFogColor = new(0.76f, 0.9f, 0.84f, 1f);
        private static readonly Color ExpeditionFogColor = new(0.3f, 0.42f, 0.4f, 1f);
        private static readonly Color HubAmbientColor = new(0.62f, 0.74f, 0.66f, 1f);
        private static readonly Color ExpeditionAmbientColor = new(0.24f, 0.34f, 0.32f, 1f);
        private static readonly Color HubSunColor = new(1f, 0.93f, 0.8f, 1f);
        private static readonly Color ExpeditionSunColor = new(0.76f, 0.9f, 0.86f, 1f);
        private static Texture2D _uvFallbackTexture;

        public static void DecorateHub(Transform parent)
        {
            if (parent == null)
            {
                return;
            }

            var root = EnsureChildRoot(parent, "RuntimeHubArt");
            ClearChildren(root);
            ApplyHubAtmosphere();

            SpawnDecor(root, ArtResourcePaths.EnvironmentVillagePlatform, "HubPlatform", new Vector3(0f, -0.04f, -2f), new Vector3(0f, 0f, 0f), Vector3.one * 1.15f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRoadCobble, "HubRoadCenter", new Vector3(0f, -0.01f, 10f), new Vector3(0f, 0f, 0f), Vector3.one * 1.15f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRoadCobble, "HubRoadBranch", new Vector3(-6f, -0.01f, 14f), new Vector3(0f, 45f, 0f), Vector3.one);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRoadWood, "HubDockPlankA", new Vector3(8f, -0.02f, 2f), new Vector3(0f, 90f, 0f), Vector3.one * 1.1f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRoadWood, "HubDockPlankB", new Vector3(11f, -0.02f, 2f), new Vector3(0f, 90f, 0f), Vector3.one * 1.1f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentHubIsland, "HubIsland", new Vector3(-14f, -0.05f, 6f), new Vector3(0f, 20f, 0f), Vector3.one * 0.95f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentPier, "HubPier", new Vector3(12f, -0.02f, -4f), new Vector3(0f, -35f, 0f), Vector3.one);
            SpawnDecor(root, ArtResourcePaths.EnvironmentMossPatch, "HubMossPatchNorth", new Vector3(10f, 0.02f, 10f), new Vector3(0f, 0f, 0f), Vector3.one * 1.25f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentMossPatch, "HubMossPatchSouth", new Vector3(-8f, 0.02f, 4f), new Vector3(0f, 125f, 0f), Vector3.one * 0.95f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRockCluster, "HubRockCluster", new Vector3(-12f, 0f, -10f), new Vector3(0f, 145f, 0f), Vector3.one * 0.9f);
            CreateLantern(root, "HubLanternA", new Vector3(-4f, 0f, 8f), new Color(1f, 0.84f, 0.56f, 1f), 6f, 1.7f);
            CreateLantern(root, "HubLanternB", new Vector3(5f, 0f, 12f), new Color(1f, 0.78f, 0.48f, 1f), 6.5f, 1.7f);
            CreateLantern(root, "HubLanternC", new Vector3(9f, 0f, -1f), new Color(1f, 0.8f, 0.54f, 1f), 5.8f, 1.6f);
            StyleGroundPlane(new Color(0.34f, 0.47f, 0.41f, 1f), new Vector3(1.7f, 1f, 1.7f));
        }

        public static void DecorateHub(Transform parent, SaveService saveService)
        {
            if (parent == null) return;

            // Call existing decoration
            DecorateHub(parent);

            if (saveService == null) return;

            var root = parent.Find("RuntimeHubArt");
            if (root == null) return;

            // Add restoration visuals based on zone states
            AddRestorationVisuals(root, saveService);
        }

        private static void AddRestorationVisuals(Transform root, SaveService saveService)
        {
            var restorationRoot = EnsureChildRoot(root, "RestorationVisuals");
            ClearChildren(restorationRoot);

            // Dock zone restored — add dock lights
            if (saveService.IsHubZoneRestored("dock"))
            {
                CreateLantern(restorationRoot, "DockRestoredLight", new Vector3(12f, 0f, 0f), new Color(0.4f, 0.95f, 0.85f, 1f), 7f, 1.9f);
            }

            // Reed Fields zone restored — add vegetation patch
            if (saveService.IsHubZoneRestored("reed_fields"))
            {
                SpawnDecor(restorationRoot, ArtResourcePaths.EnvironmentMossPatch, "ReedRestoredPatch", new Vector3(-6f, 0.02f, 16f), new Vector3(0f, 30f, 0f), Vector3.one * 1.1f);
                CreateLantern(restorationRoot, "ReedRestoredLight", new Vector3(-8f, 0f, 14f), new Color(0.45f, 0.78f, 0.42f, 1f), 5f, 1.5f);
            }

            // Tidal Vault zone restored — add platform
            if (saveService.IsHubZoneRestored("tidal_vault"))
            {
                SpawnDecor(restorationRoot, ArtResourcePaths.EnvironmentVillagePlatform, "VaultRestoredPlatform", new Vector3(6f, -0.04f, 16f), new Vector3(0f, -15f, 0f), Vector3.one * 0.7f);
                CreateLantern(restorationRoot, "VaultRestoredLight", new Vector3(6f, 0f, 16f), new Color(0.28f, 0.58f, 0.82f, 1f), 6f, 1.6f);
            }

            // Glass Narrows zone restored — add crystal light
            if (saveService.IsHubZoneRestored("glass_narrows"))
            {
                CreateLantern(restorationRoot, "NarrowsRestoredLight", new Vector3(-10f, 0f, 20f), new Color(0.72f, 0.85f, 0.92f, 1f), 8f, 2.0f);
            }

            // Sunken Arcade zone restored — add warm glow
            if (saveService.IsHubZoneRestored("sunken_arcade"))
            {
                SpawnDecor(restorationRoot, ArtResourcePaths.EnvironmentRoadCobble, "ArcadeRestoredPath", new Vector3(0f, -0.01f, 22f), new Vector3(0f, 0f, 0f), Vector3.one * 0.9f);
                CreateLantern(restorationRoot, "ArcadeRestoredLight", new Vector3(0f, 0f, 22f), new Color(0.92f, 0.62f, 0.28f, 1f), 7f, 1.8f);
            }

            // Lighthouse Crown zone restored — add beacon
            if (saveService.IsHubZoneRestored("lighthouse_crown"))
            {
                CreateLantern(restorationRoot, "CrownRestoredBeacon", new Vector3(0f, 0f, 26f), new Color(1f, 0.95f, 0.8f, 1f), 12f, 2.5f);
            }
        }

        private readonly struct ExpeditionBackdropProfile
        {
            public ExpeditionBackdropProfile(
                string primaryBackdrop,
                string secondaryBackdrop,
                string entryPath,
                string patchPrimary,
                string patchSecondary,
                float primaryScale,
                float secondaryScale,
                float patchScale,
                Color signalTint)
            {
                PrimaryBackdrop = primaryBackdrop;
                SecondaryBackdrop = secondaryBackdrop;
                EntryPath = entryPath;
                PatchPrimary = patchPrimary;
                PatchSecondary = patchSecondary;
                PrimaryScale = primaryScale;
                SecondaryScale = secondaryScale;
                PatchScale = patchScale;
                SignalTint = signalTint;
            }

            public string PrimaryBackdrop { get; }
            public string SecondaryBackdrop { get; }
            public string EntryPath { get; }
            public string PatchPrimary { get; }
            public string PatchSecondary { get; }
            public float PrimaryScale { get; }
            public float SecondaryScale { get; }
            public float PatchScale { get; }
            public Color SignalTint { get; }
        }

        public static void DecorateExpedition(Transform parent)
        {
            DecorateExpedition(parent, null, null);
        }

        public static void DecorateExpedition(Transform parent, MossHarbor.Data.DistrictDef district)
        {
            DecorateExpedition(parent, district, null);
        }

        public static void DecorateExpedition(Transform parent, MossHarbor.Data.DistrictDef district, ExpeditionLevelLayoutPlan layoutPlan)
        {
            if (parent == null)
            {
                return;
            }

            var root = EnsureChildRoot(parent, "RuntimeExpeditionArt");
            ClearChildren(root);
            ApplyExpeditionAtmosphere(district);

            layoutPlan ??= ExpeditionLevelLayoutBuilder.CreatePlan(district);
            DecorateExpeditionBackdrop(root, district, layoutPlan);
            StyleGroundPlane(new Color(0.22f, 0.33f, 0.31f, 1f), new Vector3(2.35f, 1f, 2.65f));
        }

        public static Transform AttachPlayerVisual(Transform playerRoot)
        {
            if (playerRoot == null)
            {
                return null;
            }

            var existing = playerRoot.Find("RuntimePlayerVisual");
            if (existing != null)
            {
                return existing;
            }

            var prefab = Resources.Load<GameObject>(ArtResourcePaths.CharacterPlayerAvatar);
            if (prefab == null)
            {
                return null;
            }

            var instance = Object.Instantiate(prefab, playerRoot);
            instance.name = "RuntimePlayerVisual";
            instance.transform.localPosition = new Vector3(0f, -0.5f, 0f);
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one * 1.05f;

            DisableColliders(instance);
            StabilizeRigidbodies(instance);
            ConfigureAnimators(instance);
            EnsureRuntimeUvFallback(instance);
            NormalizeRendererMaterials(instance, null, false, true);
            return instance.transform;
        }

        public static GameObject CreatePickupVisual(Transform parent, string objectName, Vector3 worldPosition, ResourceType resourceType, Color tint)
        {
            var path = resourceType == ResourceType.Scrap ? ArtResourcePaths.PropScrapPickup : ArtResourcePaths.PropBloomPickup;
            var prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                return null;
            }

            var instance = Object.Instantiate(prefab, parent);
            instance.name = objectName;
            instance.transform.position = worldPosition;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = resourceType == ResourceType.Scrap ? Vector3.one * 1.4f : Vector3.one * 1.9f;

            DisableColliders(instance);
            EnsureTriggerCollider(instance, resourceType == ResourceType.Scrap ? 0.7f : 0.9f);
            NormalizeRendererMaterials(instance, tint, resourceType == ResourceType.BloomDust);
            return instance;
        }

        public static GameObject CreateObjectiveBeaconVisual(Transform parent, Vector3 worldPosition)
        {
            var prefab = Resources.Load<GameObject>(ArtResourcePaths.PropObjectiveBeacon);
            if (prefab == null)
            {
                return null;
            }

            var instance = Object.Instantiate(prefab, parent);
            instance.name = "ObjectiveBeacon";
            instance.transform.position = worldPosition;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one * 2.8f;

            DisableColliders(instance);
            EnsureTriggerCollider(instance, 1.2f);
            NormalizeRendererMaterials(instance, new Color(0.35f, 0.85f, 0.74f, 1f), true);
            return instance;
        }

        public static GameObject CreateRouteSignalVisual(Transform parent, string objectName, Vector3 worldPosition, Color tint, float scaleMultiplier, bool elevated)
        {
            return CreateGameplayMarkerVisual(
                parent,
                objectName,
                worldPosition + Vector3.up * (elevated ? 1.2f : 1.05f),
                Color.Lerp(tint, Color.white, 0.3f),
                0.26f * scaleMultiplier,
                0.82f * scaleMultiplier,
                elevated ? 4.8f : 3.8f,
                elevated ? 1.75f : 1.35f);
        }

        public static GameObject CreateLandingMarkerVisual(Transform parent, string objectName, Vector3 worldPosition, Color tint, float scaleMultiplier)
        {
            return CreateGameplayMarkerVisual(
                parent,
                objectName,
                worldPosition + Vector3.up * 0.08f,
                Color.Lerp(tint, Color.white, 0.24f),
                0.42f * scaleMultiplier,
                1.05f * scaleMultiplier,
                5.8f,
                1.65f);
        }

        public static GameObject CreateEnvironmentDecor(Transform parent, string resourcePath, string objectName, Vector3 worldPosition, Vector3 worldEulerAngles, Vector3 localScale)
        {
            if (parent == null || string.IsNullOrWhiteSpace(resourcePath))
            {
                return null;
            }

            var prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab == null)
            {
                return null;
            }

            var instance = Object.Instantiate(prefab, parent);
            instance.name = objectName;
            instance.transform.position = worldPosition;
            instance.transform.eulerAngles = worldEulerAngles;
            instance.transform.localScale = localScale;
            DisableColliders(instance);
            StabilizeRigidbodies(instance);
            NormalizeRendererMaterials(instance);
            return instance;
        }

        private static void DecorateExpeditionBackdrop(Transform root, DistrictDef district, ExpeditionLevelLayoutPlan layoutPlan)
        {
            if (root == null || layoutPlan == null)
            {
                return;
            }

            var profile = ResolveExpeditionBackdropProfile(district);
            var shoulderX = layoutPlan.halfWidth * 0.74f;
            var edgeX = layoutPlan.halfWidth * 0.9f;
            var southZ = -layoutPlan.halfLength * 0.34f;
            var lowerMidZ = layoutPlan.halfLength * 0.04f;
            var upperMidZ = layoutPlan.halfLength * 0.44f;
            var northZ = layoutPlan.halfLength * 0.76f;
            var backdropZ = layoutPlan.beaconPosition.z + 3.2f;

            SpawnDecor(root, profile.EntryPath, "ExpeditionShoulderEntryWest", new Vector3(-shoulderX, -0.02f, southZ), new Vector3(0f, 12f, 0f), new Vector3(0.82f, 0.82f, 0.9f));
            SpawnDecor(root, profile.EntryPath, "ExpeditionShoulderEntryEast", new Vector3(shoulderX, -0.02f, southZ + 1.5f), new Vector3(0f, -14f, 0f), new Vector3(0.84f, 0.84f, 0.94f));
            SpawnDecor(root, profile.PatchPrimary, "ExpeditionShoulderPatchWest", new Vector3(-edgeX, -0.02f, lowerMidZ), new Vector3(0f, 38f, 0f), Vector3.one * profile.PatchScale);
            SpawnDecor(root, profile.PatchPrimary, "ExpeditionShoulderPatchEast", new Vector3(edgeX, -0.02f, upperMidZ), new Vector3(0f, -28f, 0f), Vector3.one * (profile.PatchScale * 0.92f));
            SpawnDecor(root, profile.PatchSecondary, "ExpeditionFlankAccentWest", new Vector3(-shoulderX, 0f, northZ), new Vector3(0f, 132f, 0f), Vector3.one * (profile.PatchScale * 0.66f));
            SpawnDecor(root, profile.PatchSecondary, "ExpeditionFlankAccentEast", new Vector3(shoulderX, 0f, northZ - 1.8f), new Vector3(0f, -114f, 0f), Vector3.one * (profile.PatchScale * 0.62f));
            SpawnDecor(root, profile.PrimaryBackdrop, "ExpeditionBackdropWest", new Vector3(-edgeX, -0.05f, backdropZ), new Vector3(0f, 24f, 0f), Vector3.one * profile.PrimaryScale);
            SpawnDecor(root, profile.PrimaryBackdrop, "ExpeditionBackdropEast", new Vector3(edgeX, -0.05f, backdropZ - 1.6f), new Vector3(0f, -32f, 0f), Vector3.one * (profile.PrimaryScale * 0.96f));
            SpawnDecor(root, profile.SecondaryBackdrop, "ExpeditionBackdropCenterLeft", new Vector3(-shoulderX * 0.56f, -0.04f, backdropZ + 1.3f), new Vector3(0f, -8f, 0f), Vector3.one * profile.SecondaryScale);
            SpawnDecor(root, profile.SecondaryBackdrop, "ExpeditionBackdropCenterRight", new Vector3(shoulderX * 0.56f, -0.04f, backdropZ + 0.4f), new Vector3(0f, 16f, 0f), Vector3.one * (profile.SecondaryScale * 0.94f));

            CreateLantern(root, "ExpeditionSignalWest", new Vector3(-shoulderX, 0f, layoutPlan.halfLength * 0.18f), profile.SignalTint, 4.8f, 1.55f);
            CreateLantern(root, "ExpeditionSignalEast", new Vector3(shoulderX, 0f, layoutPlan.halfLength * 0.58f), Color.Lerp(profile.SignalTint, Color.white, 0.12f), 4.8f, 1.5f);
            CreateLantern(root, "ExpeditionSignalNorthWest", new Vector3(-edgeX * 0.88f, 0f, northZ + 1f), Color.Lerp(profile.SignalTint, district != null ? district.districtColor : ExpeditionSunColor, 0.35f), 4.2f, 1.35f);
            CreateLantern(root, "ExpeditionSignalNorthEast", new Vector3(edgeX * 0.88f, 0f, northZ - 0.2f), Color.Lerp(profile.SignalTint, district != null ? district.districtColor : ExpeditionSunColor, 0.28f), 4.2f, 1.35f);
        }

        private static ExpeditionBackdropProfile ResolveExpeditionBackdropProfile(DistrictDef district)
        {
            var districtId = district != null ? district.districtId : string.Empty;
            return districtId switch
            {
                "reed_fields" => new ExpeditionBackdropProfile(
                    ArtResourcePaths.EnvironmentMossPatch,
                    ArtResourcePaths.EnvironmentPier,
                    ArtResourcePaths.EnvironmentRoadWood,
                    ArtResourcePaths.EnvironmentMossPatch,
                    ArtResourcePaths.EnvironmentMudPatch,
                    0.92f,
                    0.56f,
                    0.88f,
                    new Color(0.54f, 0.88f, 0.58f, 1f)),
                "tidal_vault" => new ExpeditionBackdropProfile(
                    ArtResourcePaths.EnvironmentVillagePlatform,
                    ArtResourcePaths.EnvironmentHubIsland,
                    ArtResourcePaths.EnvironmentRoadCobble,
                    ArtResourcePaths.EnvironmentRockCluster,
                    ArtResourcePaths.EnvironmentMossPatch,
                    0.72f,
                    0.38f,
                    0.52f,
                    new Color(0.42f, 0.72f, 0.94f, 1f)),
                "glass_narrows" => new ExpeditionBackdropProfile(
                    ArtResourcePaths.EnvironmentRockCluster,
                    ArtResourcePaths.EnvironmentHubIsland,
                    ArtResourcePaths.EnvironmentRoadCobble,
                    ArtResourcePaths.EnvironmentRockCluster,
                    ArtResourcePaths.EnvironmentMossPatch,
                    0.56f,
                    0.28f,
                    0.44f,
                    new Color(0.76f, 0.88f, 0.96f, 1f)),
                "sunken_arcade" => new ExpeditionBackdropProfile(
                    ArtResourcePaths.EnvironmentVillagePlatform,
                    ArtResourcePaths.EnvironmentRoadCobble,
                    ArtResourcePaths.EnvironmentRoadCobble,
                    ArtResourcePaths.EnvironmentMudPatch,
                    ArtResourcePaths.EnvironmentRockCluster,
                    0.62f,
                    0.74f,
                    0.56f,
                    new Color(0.94f, 0.72f, 0.38f, 1f)),
                "lighthouse_crown" => new ExpeditionBackdropProfile(
                    ArtResourcePaths.EnvironmentPier,
                    ArtResourcePaths.EnvironmentVillagePlatform,
                    ArtResourcePaths.EnvironmentRoadWood,
                    ArtResourcePaths.EnvironmentRockCluster,
                    ArtResourcePaths.EnvironmentMossPatch,
                    0.86f,
                    0.46f,
                    0.52f,
                    new Color(0.96f, 0.9f, 0.68f, 1f)),
                _ => new ExpeditionBackdropProfile(
                    ArtResourcePaths.EnvironmentPier,
                    ArtResourcePaths.EnvironmentVillagePlatform,
                    ArtResourcePaths.EnvironmentRoadWood,
                    ArtResourcePaths.EnvironmentMudPatch,
                    ArtResourcePaths.EnvironmentRockCluster,
                    0.82f,
                    0.44f,
                    0.54f,
                    new Color(0.52f, 0.92f, 0.8f, 1f)),
            };
        }

        private static Transform EnsureChildRoot(Transform parent, string childName)
        {
            var existing = parent.Find(childName);
            if (existing != null)
            {
                return existing;
            }

            var root = new GameObject(childName).transform;
            root.SetParent(parent, false);
            return root;
        }

        private static void ClearChildren(Transform root)
        {
            for (var i = root.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(root.GetChild(i).gameObject);
            }
        }

        private static void SpawnDecor(Transform parent, string resourcePath, string objectName, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale)
        {
            var prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab == null)
            {
                return;
            }

            var instance = Object.Instantiate(prefab, parent);
            instance.name = objectName;
            instance.transform.localPosition = localPosition;
            instance.transform.localEulerAngles = localEulerAngles;
            instance.transform.localScale = localScale;
            DisableColliders(instance);
            StabilizeRigidbodies(instance);
            NormalizeRendererMaterials(instance);
        }

        private static GameObject CreateGameplayMarkerVisual(Transform parent, string objectName, Vector3 worldPosition, Color tint, float ringRadius, float height, float lightRange, float lightIntensity)
        {
            var root = new GameObject(objectName);
            root.transform.SetParent(parent, false);
            root.transform.position = worldPosition;

            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Ring";
            ring.transform.SetParent(root.transform, false);
            ring.transform.localPosition = Vector3.zero;
            ring.transform.localScale = new Vector3(ringRadius, 0.03f, ringRadius);
            ApplyPrimitiveTint(ring, tint, true);

            var beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beam.name = "Beam";
            beam.transform.SetParent(root.transform, false);
            beam.transform.localPosition = new Vector3(0f, height * 0.5f, 0f);
            beam.transform.localScale = new Vector3(ringRadius * 0.16f, height * 0.5f, ringRadius * 0.16f);
            ApplyPrimitiveTint(beam, tint, true);

            var orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = "Orb";
            orb.transform.SetParent(root.transform, false);
            orb.transform.localPosition = new Vector3(0f, height, 0f);
            orb.transform.localScale = Vector3.one * ringRadius * 0.95f;
            ApplyPrimitiveTint(orb, Color.Lerp(tint, Color.white, 0.28f), true);

            var pointLight = root.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.color = tint;
            pointLight.range = lightRange;
            pointLight.intensity = lightIntensity;
            pointLight.shadows = LightShadows.None;

            DisableColliders(root);
            return root;
        }

        private static void CreateLantern(Transform parent, string objectName, Vector3 localPosition, Color lightColor, float range, float intensity)
        {
            var lanternRoot = new GameObject(objectName).transform;
            lanternRoot.SetParent(parent, false);
            lanternRoot.localPosition = localPosition;

            var post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            post.name = "Post";
            post.transform.SetParent(lanternRoot, false);
            post.transform.localPosition = new Vector3(0f, 1.1f, 0f);
            post.transform.localScale = new Vector3(0.08f, 1.1f, 0.08f);
            if (post.TryGetComponent<Renderer>(out var postRenderer))
            {
                postRenderer.material.color = new Color(0.28f, 0.22f, 0.16f, 1f);
            }

            var orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = "LightOrb";
            orb.transform.SetParent(lanternRoot, false);
            orb.transform.localPosition = new Vector3(0f, 2.2f, 0f);
            orb.transform.localScale = Vector3.one * 0.26f;
            if (orb.TryGetComponent<Renderer>(out var orbRenderer))
            {
                orbRenderer.material.color = lightColor;
            }

            var pointLight = lanternRoot.gameObject.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.color = lightColor;
            pointLight.range = range;
            pointLight.intensity = intensity;
            pointLight.shadows = LightShadows.None;

            DisableColliders(lanternRoot.gameObject);
        }

        private static void ApplyHubAtmosphere()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.012f;
            RenderSettings.fogColor = HubFogColor;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = HubAmbientColor;
            ConfigureSceneLighting(new Vector3(36f, 134f, 0f), HubSunColor, 1.45f, HubFogColor);
        }

        private static void ApplyExpeditionAtmosphere(MossHarbor.Data.DistrictDef district = null)
        {
            var fogColor = district != null ? district.fogColor : ExpeditionFogColor;
            var fogDensity = district != null ? district.fogDensity : 0.019f;
            var ambientColor = district != null ? district.ambientColor : ExpeditionAmbientColor;
            var sunColor = district != null ? district.sunColor : ExpeditionSunColor;
            var sunIntensity = district != null ? district.sunIntensity : 1.5f;

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogColor = fogColor;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = ambientColor;
            ConfigureSceneLighting(new Vector3(46f, 134f, 0f), sunColor, sunIntensity, fogColor);
        }

        private static void DisableColliders(GameObject root)
        {
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
            {
                collider.enabled = false;
            }
        }

        private static void EnsureTriggerCollider(GameObject root, float radius)
        {
            var collider = root.GetComponent<SphereCollider>();
            if (collider == null)
            {
                collider = root.AddComponent<SphereCollider>();
            }

            collider.radius = radius;
            collider.isTrigger = true;
            collider.enabled = true;
        }

        private static void StabilizeRigidbodies(GameObject root)
        {
            foreach (var body in root.GetComponentsInChildren<Rigidbody>(true))
            {
                body.isKinematic = true;
                body.useGravity = false;
            }
        }

        private static void ConfigureAnimators(GameObject root)
        {
            foreach (var animator in root.GetComponentsInChildren<Animator>(true))
            {
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                animator.updateMode = AnimatorUpdateMode.Normal;
                animator.Rebind();
            }
        }

        private static void EnsureRuntimeUvFallback(GameObject root)
        {
            if (root == null)
            {
                return;
            }

            foreach (var meshFilter in root.GetComponentsInChildren<MeshFilter>(true))
            {
                if (MeshUvGenerator.EnsureRuntimeUvMesh(meshFilter.sharedMesh, out var runtimeMesh))
                {
                    meshFilter.sharedMesh = runtimeMesh;
                }
            }

            foreach (var skinnedMeshRenderer in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (MeshUvGenerator.EnsureRuntimeUvMesh(skinnedMeshRenderer.sharedMesh, out var runtimeMesh))
                {
                    skinnedMeshRenderer.sharedMesh = runtimeMesh;
                }
            }
        }

        private static void ConfigureSceneLighting(Vector3 sunEulerAngles, Color sunColor, float intensity, Color backgroundColor)
        {
            foreach (var light in Object.FindObjectsOfType<Light>(true))
            {
                if (light.type != LightType.Directional)
                {
                    continue;
                }

                light.transform.rotation = Quaternion.Euler(sunEulerAngles);
                light.color = sunColor;
                light.intensity = intensity;
                light.shadows = LightShadows.Soft;
                light.shadowStrength = 0.72f;
                light.shadowBias = 0.05f;
                break;
            }

            foreach (var camera in Object.FindObjectsOfType<Camera>(true))
            {
                if (!camera.CompareTag("MainCamera"))
                {
                    continue;
                }

                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = backgroundColor;
                break;
            }
        }

        private static void StyleGroundPlane(Color color, Vector3 scale)
        {
            var ground = GameObject.Find("Ground");
            if (ground == null)
            {
                return;
            }

            ground.transform.localScale = scale;
            if (!ground.TryGetComponent<Renderer>(out var renderer))
            {
                return;
            }

            var material = new Material(Shader.Find("Standard"))
            {
                color = color,
                name = "RuntimeGround_BuiltIn"
            };
            material.SetFloat("_Glossiness", 0.08f);
            renderer.material = material;
        }

        private static void NormalizeRendererMaterials(GameObject root)
        {
            NormalizeRendererMaterials(root, null, false, false);
        }

        private static void NormalizeRendererMaterials(GameObject root, Color? tintOverride, bool emissive)
        {
            NormalizeRendererMaterials(root, tintOverride, emissive, false);
        }

        private static void NormalizeRendererMaterials(GameObject root, Color? tintOverride, bool emissive, bool allowUvFallback)
        {
            var standardShader = Shader.Find("Standard");
            if (root == null || standardShader == null)
            {
                return;
            }

            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                var originalMaterials = renderer.sharedMaterials;
                if (originalMaterials == null || originalMaterials.Length == 0)
                {
                    continue;
                }

                var runtimeMaterials = new Material[originalMaterials.Length];
                for (var i = 0; i < originalMaterials.Length; i++)
                {
                    runtimeMaterials[i] = CreateBuiltInMaterial(originalMaterials[i], standardShader, tintOverride, emissive, allowUvFallback);
                }

                renderer.materials = runtimeMaterials;
            }
        }

        private static Material CreateBuiltInMaterial(Material source, Shader standardShader, Color? tintOverride, bool emissive, bool allowUvFallback)
        {
            var material = new Material(standardShader)
            {
                name = source != null ? $"{source.name}_BuiltInRuntime" : "BuiltInRuntimeMaterial"
            };

            var albedo = source != null ? FindTexture(source, "_MainTex", "_BaseMap", "_Texture_Albedo", "_Albedo_Texture", "_Outside_Texture") : null;
            var normal = source != null ? FindTexture(source, "_BumpMap") : null;
            var metallic = source != null ? FindTexture(source, "_MetallicGlossMap") : null;
            var occlusion = source != null ? FindTexture(source, "_OcclusionMap") : null;
            var baseColor = tintOverride ?? (source != null ? FindColor(source, "_Color", "_BaseColor", "_Color_Inside", "_Color_inside_two", "_EmissionColor", "_EmmisionColor", "_Emmision_Color") : Color.white);

            if (albedo != null)
            {
                material.mainTexture = albedo;
            }
            else if (allowUvFallback)
            {
                material.mainTexture = GetOrCreateUvFallbackTexture();
            }

            material.color = baseColor;
            material.SetFloat("_Glossiness", emissive ? 0.45f : 0.16f);
            material.SetFloat("_Metallic", emissive ? 0.08f : 0.02f);

            if (normal != null)
            {
                material.SetTexture("_BumpMap", normal);
                material.EnableKeyword("_NORMALMAP");
            }

            if (metallic != null)
            {
                material.SetTexture("_MetallicGlossMap", metallic);
                material.EnableKeyword("_METALLICGLOSSMAP");
            }

            if (occlusion != null)
            {
                material.SetTexture("_OcclusionMap", occlusion);
            }

            if (emissive)
            {
                var emissionColor = tintOverride ?? (source != null ? FindColor(source, "_EmissionColor", "_EmmisionColor", "_Emmision_Color", "_Color") : Color.white);
                material.SetColor("_EmissionColor", emissionColor * 0.45f);
                material.EnableKeyword("_EMISSION");
            }

            return material;
        }

        private static void ApplyPrimitiveTint(GameObject target, Color color, bool emissive)
        {
            if (!target.TryGetComponent<Renderer>(out var renderer))
            {
                return;
            }

            var material = new Material(Shader.Find("Standard"))
            {
                color = color
            };
            material.SetFloat("_Glossiness", emissive ? 0.32f : 0.12f);
            if (emissive)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.35f);
            }

            renderer.material = material;
        }

        private static Texture FindTexture(Material material, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (material.HasProperty(propertyName))
                {
                    var texture = material.GetTexture(propertyName);
                    if (texture != null)
                    {
                        return texture;
                    }
                }
            }

            return null;
        }

        private static Color FindColor(Material material, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (material.HasProperty(propertyName))
                {
                    return material.GetColor(propertyName);
                }
            }

            return Color.white;
        }

        private static Texture2D GetOrCreateUvFallbackTexture()
        {
            if (_uvFallbackTexture != null)
            {
                return _uvFallbackTexture;
            }

            const int size = 128;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "Generated_UV_Fallback_Checker",
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Point
            };

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var checker = ((x / 16) + (y / 16)) % 2 == 0;
                    var baseColor = checker ? new Color(0.92f, 0.22f, 0.36f, 1f) : new Color(0.18f, 0.72f, 0.88f, 1f);
                    var u = x / (float)(size - 1);
                    var v = y / (float)(size - 1);
                    var uvTint = new Color(u, v, 1f - u * 0.4f, 1f);
                    texture.SetPixel(x, y, Color.Lerp(baseColor, uvTint, 0.25f));
                }
            }

            texture.Apply(false, true);
            _uvFallbackTexture = texture;
            return _uvFallbackTexture;
        }
    }
}
