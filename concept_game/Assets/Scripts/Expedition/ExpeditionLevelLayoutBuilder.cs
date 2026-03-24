using System.Collections.Generic;
using System.Linq;
using MossHarbor.Art;
using MossHarbor.Data;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class ExpeditionLevelLayoutPlan
    {
        public float halfWidth;
        public float halfLength;
        public float elevatedHeight;
        public Vector3 beaconPosition;
        public Vector3[] pickupAnchors = System.Array.Empty<Vector3>();

        public int ElevatedAnchorCount => pickupAnchors.Count(anchor => anchor.y > 1f);
    }

    public static class ExpeditionLevelLayoutBuilder
    {
        public static ExpeditionLevelLayoutPlan CreatePlan(DistrictDef district)
        {
            var radius = district != null ? district.pickupSpawnRadius : 8f;
            var halfWidth = Mathf.Max(18f, radius * 1.9f);
            var halfLength = Mathf.Max(26f, radius * 2.35f);
            var elevatedHeight = 2.35f;

            var anchors = new List<Vector3>
            {
                new(-halfWidth * 0.56f, 0.8f, -halfLength * 0.08f),
                new(halfWidth * 0.54f, 0.8f, -halfLength * 0.04f),
                new(-halfWidth * 0.42f, 0.8f, halfLength * 0.16f),
                new(halfWidth * 0.44f, 0.8f, halfLength * 0.24f),
                new(0f, 0.8f, halfLength * 0.08f),
                new(-halfWidth * 0.24f, 0.8f, halfLength * 0.44f),
                new(halfWidth * 0.26f, 0.8f, halfLength * 0.48f),
                new(0f, 0.8f, halfLength * 0.6f),
                new(-halfWidth * 0.26f, elevatedHeight, halfLength * 0.46f),
                new(halfWidth * 0.28f, elevatedHeight, halfLength * 0.54f),
                new(0f, elevatedHeight, halfLength * 0.68f),
                new(-halfWidth * 0.62f, 0.8f, halfLength * 0.5f),
                new(halfWidth * 0.62f, 0.8f, halfLength * 0.56f),
            };

            return new ExpeditionLevelLayoutPlan
            {
                halfWidth = halfWidth,
                halfLength = halfLength,
                elevatedHeight = elevatedHeight,
                beaconPosition = new Vector3(0f, elevatedHeight + 0.15f, halfLength * 0.82f),
                pickupAnchors = anchors.ToArray(),
            };
        }

        public static void Build(Transform parent, ExpeditionLevelLayoutPlan plan, Color districtColor)
        {
            if (parent == null || plan == null)
            {
                return;
            }

            var root = new GameObject("RuntimeLevelLayout").transform;
            root.SetParent(parent, false);

            var floorColor = Color.Lerp(districtColor, new Color(0.16f, 0.22f, 0.24f, 1f), 0.58f);
            var accentColor = Color.Lerp(districtColor, Color.white, 0.22f);
            var hazardColor = Color.Lerp(districtColor, new Color(1f, 0.45f, 0.18f, 1f), 0.5f);
            var boostColor = Color.Lerp(districtColor, new Color(0.42f, 0.94f, 0.86f, 1f), 0.55f);
            var railColor = Color.Lerp(accentColor, new Color(0.08f, 0.12f, 0.14f, 1f), 0.45f);

            var mainGroundPosition = new Vector3(0f, -0.12f, plan.halfLength * 0.08f);
            var mainGroundScale = new Vector3(plan.halfWidth * 1.9f, 0.6f, plan.halfLength * 1.35f);
            var westLanePosition = new Vector3(-plan.halfWidth * 0.62f, 0.12f, plan.halfLength * 0.28f);
            var westLaneScale = new Vector3(plan.halfWidth * 0.42f, 0.35f, plan.halfLength * 0.94f);
            var eastLanePosition = new Vector3(plan.halfWidth * 0.62f, 0.12f, plan.halfLength * 0.34f);
            var eastLaneScale = new Vector3(plan.halfWidth * 0.42f, 0.35f, plan.halfLength * 0.9f);
            var elevatedDeckPosition = new Vector3(0f, plan.elevatedHeight - 0.38f, plan.halfLength * 0.62f);
            var elevatedDeckScale = new Vector3(plan.halfWidth * 1.15f, 0.76f, plan.halfLength * 0.48f);
            var beaconBridgePosition = new Vector3(0f, plan.elevatedHeight - 0.22f, plan.halfLength * 0.77f);
            var beaconBridgeScale = new Vector3(plan.halfWidth * 0.42f, 0.32f, plan.halfLength * 0.16f);
            var beaconPlatformPosition = new Vector3(plan.beaconPosition.x, plan.elevatedHeight - 0.12f, plan.beaconPosition.z);
            var beaconPlatformScale = new Vector3(4.8f, 0.28f, 5.4f);

            CreateSolidSegment(root, "MainGround", mainGroundPosition, mainGroundScale, floorColor);
            CreateSolidSegment(root, "WestLane", westLanePosition, westLaneScale, accentColor);
            CreateSolidSegment(root, "EastLane", eastLanePosition, eastLaneScale, accentColor);
            CreateSolidSegment(root, "NorthBridge", new Vector3(0f, 0.12f, plan.halfLength * 0.44f), new Vector3(plan.halfWidth * 0.72f, 0.3f, plan.halfLength * 0.3f), accentColor);
            CreateSolidSegment(root, "ElevatedDeck", elevatedDeckPosition, elevatedDeckScale, accentColor);
            CreateSolidSegment(root, "BeaconBridge", beaconBridgePosition, beaconBridgeScale, accentColor);
            CreateSolidSegment(root, "BeaconPlatform", beaconPlatformPosition, beaconPlatformScale, Color.Lerp(accentColor, Color.white, 0.12f));

            var westRampBaseLandingPosition = new Vector3(-plan.halfWidth * 0.18f, 0.14f, plan.halfLength * 0.22f);
            var eastRampBaseLandingPosition = new Vector3(plan.halfWidth * 0.18f, 0.14f, plan.halfLength * 0.26f);
            var rampBaseLandingScale = new Vector3(5.8f, 0.24f, 2.2f);
            var westRampTopLandingPosition = new Vector3(-plan.halfWidth * 0.18f, plan.elevatedHeight - 0.06f, plan.halfLength * 0.53f);
            var eastRampTopLandingPosition = new Vector3(plan.halfWidth * 0.18f, plan.elevatedHeight - 0.06f, plan.halfLength * 0.57f);
            var rampTopLandingScale = new Vector3(5.8f, 0.18f, 2.6f);

            CreateSolidSegment(root, "WestRampBaseLanding", westRampBaseLandingPosition, rampBaseLandingScale, accentColor);
            CreateSolidSegment(root, "EastRampBaseLanding", eastRampBaseLandingPosition, rampBaseLandingScale, accentColor);
            CreateSolidSegment(root, "WestRampTopLanding", westRampTopLandingPosition, rampTopLandingScale, accentColor);
            CreateSolidSegment(root, "EastRampTopLanding", eastRampTopLandingPosition, rampTopLandingScale, accentColor);
            CreateTraversalRamp(root, "WestRamp", westRampBaseLandingPosition, rampBaseLandingScale, westRampTopLandingPosition, rampTopLandingScale, 5.6f, 0.6f, accentColor);
            CreateTraversalRamp(root, "EastRamp", eastRampBaseLandingPosition, rampBaseLandingScale, eastRampTopLandingPosition, rampTopLandingScale, 5.6f, 0.6f, accentColor);

            CreateGuardPillar(root, "WestLookout", new Vector3(-plan.halfWidth * 0.66f, 1.6f, plan.halfLength * 0.64f), districtColor);
            CreateGuardPillar(root, "EastLookout", new Vector3(plan.halfWidth * 0.66f, 1.6f, plan.halfLength * 0.7f), districtColor);

            var westBoostPadPosition = new Vector3(-plan.halfWidth * 0.44f, 0.08f, plan.halfLength * 0.04f);
            var eastBoostPadPosition = new Vector3(plan.halfWidth * 0.44f, 0.08f, plan.halfLength * 0.1f);
            var westBoostTarget = new Vector3(westBoostPadPosition.x, plan.elevatedHeight, plan.halfLength * 0.42f);
            var eastBoostTarget = new Vector3(eastBoostPadPosition.x, plan.elevatedHeight, plan.halfLength * 0.44f);

            CreateBoostPad(root, "WestBoostPad", westBoostPadPosition, westBoostTarget, boostColor);
            CreateBoostPad(root, "EastBoostPad", eastBoostPadPosition, eastBoostTarget, boostColor);

            CreatePerimeterRail(root, "MainGroundWestRail", mainGroundPosition + new Vector3(-(mainGroundScale.x * 0.5f) + 0.2f, 0.52f, 0f), new Vector3(0.4f, 0.95f, mainGroundScale.z * 0.92f), railColor);
            CreatePerimeterRail(root, "MainGroundEastRail", mainGroundPosition + new Vector3((mainGroundScale.x * 0.5f) - 0.2f, 0.52f, 0f), new Vector3(0.4f, 0.95f, mainGroundScale.z * 0.92f), railColor);
            CreatePerimeterRail(root, "MainGroundSouthRail", mainGroundPosition + new Vector3(0f, 0.52f, -(mainGroundScale.z * 0.5f) + 0.2f), new Vector3(mainGroundScale.x * 0.8f, 0.95f, 0.4f), railColor);
            CreatePerimeterRail(root, "WestLaneOuterRail", westLanePosition + new Vector3(-(westLaneScale.x * 0.5f) + 0.12f, 0.54f, 0f), new Vector3(0.32f, 0.95f, westLaneScale.z * 0.94f), railColor);
            CreatePerimeterRail(root, "EastLaneOuterRail", eastLanePosition + new Vector3((eastLaneScale.x * 0.5f) - 0.12f, 0.54f, 0f), new Vector3(0.32f, 0.95f, eastLaneScale.z * 0.94f), railColor);
            CreatePerimeterRail(root, "ElevatedDeckWestRail", elevatedDeckPosition + new Vector3(-(elevatedDeckScale.x * 0.5f) + 0.16f, plan.elevatedHeight + 0.22f, 0f), new Vector3(0.32f, 0.88f, elevatedDeckScale.z * 0.9f), railColor);
            CreatePerimeterRail(root, "ElevatedDeckEastRail", elevatedDeckPosition + new Vector3((elevatedDeckScale.x * 0.5f) - 0.16f, plan.elevatedHeight + 0.22f, 0f), new Vector3(0.32f, 0.88f, elevatedDeckScale.z * 0.9f), railColor);
            CreatePerimeterRail(root, "ElevatedDeckNorthRail", elevatedDeckPosition + new Vector3(0f, plan.elevatedHeight + 0.22f, (elevatedDeckScale.z * 0.5f) - 0.18f), new Vector3(elevatedDeckScale.x * 0.92f, 0.88f, 0.32f), railColor);
            CreatePerimeterRail(root, "BeaconBridgeWestRail", beaconBridgePosition + new Vector3(-(beaconBridgeScale.x * 0.5f) + 0.1f, plan.elevatedHeight + 0.18f, 0f), new Vector3(0.22f, 0.82f, beaconBridgeScale.z * 0.98f), railColor);
            CreatePerimeterRail(root, "BeaconBridgeEastRail", beaconBridgePosition + new Vector3((beaconBridgeScale.x * 0.5f) - 0.1f, plan.elevatedHeight + 0.18f, 0f), new Vector3(0.22f, 0.82f, beaconBridgeScale.z * 0.98f), railColor);
            CreatePerimeterRail(root, "BeaconPlatformWestRail", beaconPlatformPosition + new Vector3(-(beaconPlatformScale.x * 0.5f) + 0.1f, plan.elevatedHeight + 0.18f, 0f), new Vector3(0.24f, 0.82f, beaconPlatformScale.z * 0.92f), railColor);
            CreatePerimeterRail(root, "BeaconPlatformEastRail", beaconPlatformPosition + new Vector3((beaconPlatformScale.x * 0.5f) - 0.1f, plan.elevatedHeight + 0.18f, 0f), new Vector3(0.24f, 0.82f, beaconPlatformScale.z * 0.92f), railColor);
            CreatePerimeterRail(root, "BeaconPlatformNorthRail", beaconPlatformPosition + new Vector3(0f, plan.elevatedHeight + 0.18f, (beaconPlatformScale.z * 0.5f) - 0.1f), new Vector3(beaconPlatformScale.x * 0.92f, 0.82f, 0.24f), railColor);

            CreateSweepHazard(root, "CentralSweeper", new Vector3(0f, 0.65f, plan.halfLength * 0.18f), 5.8f, 52f, hazardColor, 4.25f, 0.6f, 0.72f, 6f);
            CreateSweepHazard(root, "BridgeSweeper", new Vector3(0f, plan.elevatedHeight + 0.2f, plan.halfLength * 0.54f), 2.6f, -54f, hazardColor, 3.25f, 0.35f, 0.8f, 3.5f);
        }

        private static void CreateSolidSegment(Transform parent, string name, Vector3 position, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            ApplyColor(go, color);
        }

        private static void CreateRamp(Transform parent, string name, Vector3 position, Vector3 scale, float pitchDegrees, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localEulerAngles = new Vector3(pitchDegrees, 0f, 0f);
            go.transform.localScale = scale;
            ApplyColor(go, color);
        }

        private static void CreateTraversalRamp(
            Transform parent,
            string name,
            Vector3 baseLandingPosition,
            Vector3 baseLandingScale,
            Vector3 topLandingPosition,
            Vector3 topLandingScale,
            float width,
            float height,
            Color color)
        {
            var start = baseLandingPosition
                        + Vector3.up * (baseLandingScale.y * 0.5f - 0.02f)
                        + Vector3.forward * (baseLandingScale.z * 0.18f);
            var end = topLandingPosition
                      + Vector3.up * (topLandingScale.y * 0.5f - 0.02f)
                      + Vector3.back * (topLandingScale.z * 0.18f);

            var delta = end - start;
            if (delta.sqrMagnitude < 0.001f)
            {
                return;
            }

            var ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = name;
            ramp.transform.SetParent(parent, false);
            ramp.transform.position = start + delta * 0.5f;
            ramp.transform.rotation = Quaternion.LookRotation(delta.normalized, Vector3.up);
            ramp.transform.localScale = new Vector3(width, height, delta.magnitude);
            ApplyColor(ramp, color);
        }

        private static void CreateGuardPillar(Transform parent, string name, Vector3 position, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = new Vector3(1.2f, 1.8f, 1.2f);
            ApplyColor(go, Color.Lerp(color, Color.white, 0.35f));
        }

        private static void CreateBoostPad(Transform parent, string name, Vector3 position, Vector3 targetPosition, Color color)
        {
            var launchDirection = (targetPosition - position);
            var facingDirection = Vector3.ProjectOnPlane(launchDirection, Vector3.up).normalized;
            if (facingDirection.sqrMagnitude < 0.001f)
            {
                facingDirection = Vector3.forward;
            }

            var surface = GameObject.CreatePrimitive(PrimitiveType.Cube);
            surface.name = name;
            surface.transform.SetParent(parent, false);
            surface.transform.localPosition = position;
            surface.transform.forward = facingDirection;
            surface.transform.localScale = new Vector3(3.8f, 0.28f, 4.4f);
            ApplyColor(surface, color);
            var boostPad = surface.AddComponent<TraversalBoostPad>();

            CreateSolidSegment(surface.transform, $"{name}GuideRamp", new Vector3(0f, 0.3f, 2.3f), new Vector3(3f, 0.18f, 1.5f), Color.Lerp(color, Color.white, 0.18f));
            CreateAlignedLiftSegment(parent, $"{name}LiftPath", position + facingDirection * 2.2f + Vector3.up * 0.16f, targetPosition + Vector3.up * 0.06f, 2.4f, 0.42f, Color.Lerp(color, Color.white, 0.08f));
            CreateSolidSegment(parent, $"{name}ExitLanding", targetPosition + new Vector3(0f, -0.08f, 0.48f), new Vector3(3.4f, 0.18f, 1.8f), Color.Lerp(color, Color.white, 0.18f));
            CreateBoostPadDecor(parent, name, position, targetPosition, facingDirection);
            RuntimeArtDirector.CreateLandingMarkerVisual(parent, $"{name}LandingMarker", targetPosition + new Vector3(0f, 0.04f, 0.48f), color, 1f);

            var visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.name = $"{name}PulseVisual";
            visual.transform.SetParent(surface.transform, false);
            visual.transform.localPosition = new Vector3(0f, 0.18f, 0.2f);
            visual.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            visual.transform.localScale = new Vector3(1.6f, 0.08f, 1.72f);
            ApplyColor(visual, Color.Lerp(color, Color.white, 0.12f));
            RemoveCollider(visual);

            var trigger = new GameObject($"{name}Trigger");
            trigger.transform.SetParent(surface.transform, false);
            trigger.transform.localPosition = new Vector3(0f, 0.62f, 0.28f);
            trigger.transform.localRotation = Quaternion.identity;
            var triggerBox = trigger.AddComponent<BoxCollider>();
            triggerBox.isTrigger = true;
            triggerBox.center = new Vector3(0f, 0.15f, 0.55f);
            triggerBox.size = new Vector3(3.45f, 1.4f, 4.2f);
            var triggerBody = trigger.AddComponent<Rigidbody>();
            triggerBody.isKinematic = true;
            triggerBody.useGravity = false;
            var relay = trigger.AddComponent<TraversalBoostPadTriggerRelay>();
            relay.Configure(boostPad);

            boostPad.Configure(facingDirection, 18f, 6.5f, visual.transform);
        }

        private static void CreateBoostPadDecor(Transform parent, string name, Vector3 position, Vector3 targetPosition, Vector3 facingDirection)
        {
            var yaw = Quaternion.LookRotation(facingDirection, Vector3.up).eulerAngles;
            var outward = Vector3.Cross(Vector3.up, facingDirection).normalized;
            var outwardHint = position.x >= 0f ? Vector3.right : Vector3.left;
            if (Vector3.Dot(outward, outwardHint) < 0f)
            {
                outward = -outward;
            }

            RuntimeArtDirector.CreateEnvironmentDecor(parent, ArtResourcePaths.EnvironmentRoadWood, $"{name}DeckDecor", position + Vector3.up * 0.05f, yaw, Vector3.one * 0.42f);
            RuntimeArtDirector.CreateEnvironmentDecor(parent, ArtResourcePaths.EnvironmentVillagePlatform, $"{name}LiftDecor", Vector3.Lerp(position, targetPosition, 0.48f) + Vector3.up * 0.12f, yaw, new Vector3(0.26f, 0.26f, 0.44f));
            RuntimeArtDirector.CreateEnvironmentDecor(parent, ArtResourcePaths.EnvironmentVillagePlatform, $"{name}ExitDecor", targetPosition + new Vector3(0f, -0.1f, 0.44f), yaw, Vector3.one * 0.24f);
            RuntimeArtDirector.CreateEnvironmentDecor(parent, ArtResourcePaths.EnvironmentRockCluster, $"{name}RockClusterA", position + outward * 2.45f + facingDirection * 1.15f, Vector3.zero, Vector3.one * 0.16f);
            RuntimeArtDirector.CreateEnvironmentDecor(parent, ArtResourcePaths.EnvironmentRockCluster, $"{name}RockClusterB", position + outward * 2.1f + facingDirection * 3.05f, new Vector3(0f, 130f, 0f), Vector3.one * 0.14f);
        }

        private static void CreatePerimeterRail(Transform parent, string name, Vector3 position, Vector3 scale, Color color)
        {
            var rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rail.name = name;
            rail.transform.SetParent(parent, false);
            rail.transform.localPosition = position;
            rail.transform.localScale = scale;
            ApplyColor(rail, color);
        }

        private static void CreateAlignedLiftSegment(Transform parent, string name, Vector3 startPosition, Vector3 endPosition, float width, float height, Color color)
        {
            var delta = endPosition - startPosition;
            if (delta.sqrMagnitude < 0.001f)
            {
                return;
            }

            var lift = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lift.name = name;
            lift.transform.SetParent(parent, false);
            lift.transform.position = startPosition + delta * 0.5f;
            lift.transform.rotation = Quaternion.LookRotation(delta.normalized, Vector3.up);
            lift.transform.localScale = new Vector3(width, height, delta.magnitude);
            ApplyColor(lift, color);
        }

        private static void CreateSweepHazard(Transform parent, string name, Vector3 pivotPosition, float orbitRadius, float rotateSpeed, Color color, float pushStrength, float verticalLift, float repulseCooldown, float beamLength)
        {
            var pivot = new GameObject($"{name}_Pivot").transform;
            pivot.SetParent(parent, false);
            pivot.localPosition = pivotPosition;

            var beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beam.name = name;
            beam.transform.SetParent(parent, false);
            beam.transform.position = pivot.position + Vector3.forward * orbitRadius;
            beam.transform.localScale = new Vector3(0.75f, 1.3f, beamLength);
            ApplyColor(beam, color);

            var rigidbody = beam.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;

            if (beam.TryGetComponent<BoxCollider>(out var box))
            {
                box.isTrigger = true;
            }

            var hazard = beam.AddComponent<SweepHazard>();
            hazard.Configure(pivot, rotateSpeed, pushStrength, verticalLift, repulseCooldown);
        }

        private static void ApplyColor(GameObject go, Color color)
        {
            if (!go.TryGetComponent<Renderer>(out var renderer))
            {
                return;
            }

            var baseMaterial = renderer.sharedMaterial;
            var material = baseMaterial != null
                ? new Material(baseMaterial)
                : new Material(Shader.Find("Standard"));
            material.color = color;

            if (Application.isPlaying)
            {
                renderer.material = material;
                return;
            }

            renderer.sharedMaterial = material;
        }

        private static void RemoveCollider(GameObject go)
        {
            if (!go.TryGetComponent<Collider>(out var collider))
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(collider);
                return;
            }

            Object.DestroyImmediate(collider);
        }
    }
}
