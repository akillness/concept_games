using MossHarbor.Core;
using MossHarbor.Data;
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

        public static void DecorateExpedition(Transform parent)
        {
            DecorateExpedition(parent, null);
        }

        public static void DecorateExpedition(Transform parent, MossHarbor.Data.DistrictDef district)
        {
            if (parent == null)
            {
                return;
            }

            var root = EnsureChildRoot(parent, "RuntimeExpeditionArt");
            ClearChildren(root);
            ApplyExpeditionAtmosphere(district);

            SpawnDecor(root, ArtResourcePaths.EnvironmentVillagePlatform, "ExpeditionBase", new Vector3(0f, -0.04f, 5.4f), new Vector3(0f, 0f, 0f), Vector3.one * 1.18f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRoadWood, "ExpeditionPathEntry", new Vector3(0f, -0.02f, -7.5f), new Vector3(0f, 0f, 0f), Vector3.one * 1.2f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRoadWood, "ExpeditionPathMid", new Vector3(0f, -0.02f, -2.5f), new Vector3(0f, 0f, 0f), Vector3.one * 1.18f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRoadCobble, "ExpeditionBrokenRoadA", new Vector3(0.8f, -0.02f, 3.4f), new Vector3(0f, 12f, 0f), Vector3.one * 1.05f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRoadCobble, "ExpeditionBrokenRoadB", new Vector3(1.2f, -0.02f, 8.4f), new Vector3(0f, -10f, 0f), Vector3.one * 0.82f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentMudPatch, "ExpeditionMudA", new Vector3(6.1f, -0.02f, 3.8f), new Vector3(0f, 20f, 0f), Vector3.one * 1f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentMudPatch, "ExpeditionMudB", new Vector3(-5.6f, -0.02f, 3.1f), new Vector3(0f, -24f, 0f), Vector3.one * 0.96f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentMudPatch, "ExpeditionMudC", new Vector3(4.8f, -0.02f, 10.2f), new Vector3(0f, 42f, 0f), Vector3.one * 0.72f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentPier, "ExpeditionPier", new Vector3(-9.6f, -0.02f, 11.2f), new Vector3(0f, 32f, 0f), Vector3.one * 0.56f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentHubIsland, "ExpeditionIsland", new Vector3(9.8f, -0.05f, 10.8f), new Vector3(0f, -18f, 0f), Vector3.one * 0.24f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRockCluster, "ExpeditionRockClusterA", new Vector3(7.1f, 0f, 6.2f), new Vector3(0f, -42f, 0f), Vector3.one * 0.28f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentRockCluster, "ExpeditionRockClusterB", new Vector3(-9.4f, 0f, 8.8f), new Vector3(0f, 68f, 0f), Vector3.one * 0.2f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentMossPatch, "ExpeditionMossPatchA", new Vector3(3.6f, 0.02f, 11.2f), new Vector3(0f, 0f, 0f), Vector3.one * 0.68f);
            SpawnDecor(root, ArtResourcePaths.EnvironmentMossPatch, "ExpeditionMossPatchB", new Vector3(-4.2f, 0.02f, 7.8f), new Vector3(0f, 75f, 0f), Vector3.one * 0.58f);
            CreateLantern(root, "ExpeditionSignalA", new Vector3(-2.4f, 0f, 2.4f), new Color(0.52f, 0.92f, 0.8f, 1f), 5.6f, 1.8f);
            CreateLantern(root, "ExpeditionSignalB", new Vector3(3.8f, 0f, 10.6f), new Color(0.38f, 0.74f, 0.66f, 1f), 4.8f, 1.6f);
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
