using System.Collections.Generic;
using System.Linq;
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

            CreateSolidSegment(root, "MainGround", new Vector3(0f, -0.12f, plan.halfLength * 0.08f), new Vector3(plan.halfWidth * 1.9f, 0.6f, plan.halfLength * 1.35f), floorColor);
            CreateSolidSegment(root, "WestLane", new Vector3(-plan.halfWidth * 0.62f, 0.12f, plan.halfLength * 0.28f), new Vector3(plan.halfWidth * 0.42f, 0.35f, plan.halfLength * 0.94f), accentColor);
            CreateSolidSegment(root, "EastLane", new Vector3(plan.halfWidth * 0.62f, 0.12f, plan.halfLength * 0.34f), new Vector3(plan.halfWidth * 0.42f, 0.35f, plan.halfLength * 0.9f), accentColor);
            CreateSolidSegment(root, "NorthBridge", new Vector3(0f, 0.12f, plan.halfLength * 0.44f), new Vector3(plan.halfWidth * 0.72f, 0.3f, plan.halfLength * 0.3f), accentColor);
            CreateSolidSegment(root, "ElevatedDeck", new Vector3(0f, plan.elevatedHeight - 0.38f, plan.halfLength * 0.62f), new Vector3(plan.halfWidth * 1.15f, 0.76f, plan.halfLength * 0.48f), accentColor);
            CreateSolidSegment(root, "BeaconBridge", new Vector3(0f, plan.elevatedHeight - 0.22f, plan.halfLength * 0.77f), new Vector3(plan.halfWidth * 0.42f, 0.32f, plan.halfLength * 0.16f), accentColor);
            CreateSolidSegment(root, "BeaconPlatform", new Vector3(plan.beaconPosition.x, plan.elevatedHeight - 0.12f, plan.beaconPosition.z), new Vector3(4.8f, 0.28f, 5.4f), Color.Lerp(accentColor, Color.white, 0.12f));

            CreateRamp(root, "WestRamp", new Vector3(-plan.halfWidth * 0.18f, plan.elevatedHeight * 0.32f, plan.halfLength * 0.36f), new Vector3(5.6f, 0.6f, 9.8f), -16f, accentColor);
            CreateRamp(root, "EastRamp", new Vector3(plan.halfWidth * 0.18f, plan.elevatedHeight * 0.32f, plan.halfLength * 0.4f), new Vector3(5.6f, 0.6f, 9.8f), -16f, accentColor);

            CreateGuardPillar(root, "WestLookout", new Vector3(-plan.halfWidth * 0.66f, 1.6f, plan.halfLength * 0.64f), districtColor);
            CreateGuardPillar(root, "EastLookout", new Vector3(plan.halfWidth * 0.66f, 1.6f, plan.halfLength * 0.7f), districtColor);

            CreateBoostPad(root, "WestBoostPad", new Vector3(-plan.halfWidth * 0.44f, 0.08f, plan.halfLength * 0.04f), Vector3.forward, boostColor);
            CreateBoostPad(root, "EastBoostPad", new Vector3(plan.halfWidth * 0.44f, 0.08f, plan.halfLength * 0.1f), Vector3.forward, boostColor);

            CreateSweepHazard(root, "CentralSweeper", new Vector3(0f, 0.65f, plan.halfLength * 0.18f), 5.8f, 52f, hazardColor);
            CreateSweepHazard(root, "BridgeSweeper", new Vector3(0f, plan.elevatedHeight + 0.2f, plan.halfLength * 0.56f), 4.1f, -66f, hazardColor);
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

        private static void CreateGuardPillar(Transform parent, string name, Vector3 position, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = new Vector3(1.2f, 1.8f, 1.2f);
            ApplyColor(go, Color.Lerp(color, Color.white, 0.35f));
        }

        private static void CreateBoostPad(Transform parent, string name, Vector3 position, Vector3 forward, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.forward = forward.normalized;
            go.transform.localScale = new Vector3(3.4f, 0.18f, 3.8f);
            ApplyColor(go, color);

            if (go.TryGetComponent<BoxCollider>(out var box))
            {
                box.isTrigger = true;
            }

            var pad = go.AddComponent<TraversalBoostPad>();
            pad.Configure(forward.normalized, 11f, 4f);
        }

        private static void CreateSweepHazard(Transform parent, string name, Vector3 pivotPosition, float orbitRadius, float rotateSpeed, Color color)
        {
            var pivot = new GameObject($"{name}_Pivot").transform;
            pivot.SetParent(parent, false);
            pivot.localPosition = pivotPosition;

            var beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beam.name = name;
            beam.transform.SetParent(parent, false);
            beam.transform.position = pivot.position + Vector3.forward * orbitRadius;
            beam.transform.localScale = new Vector3(0.75f, 1.3f, 6f);
            ApplyColor(beam, color);

            var rigidbody = beam.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;

            if (beam.TryGetComponent<BoxCollider>(out var box))
            {
                box.isTrigger = true;
            }

            var hazard = beam.AddComponent<SweepHazard>();
            hazard.Configure(pivot, rotateSpeed, 8f);
        }

        private static void ApplyColor(GameObject go, Color color)
        {
            if (!go.TryGetComponent<Renderer>(out var renderer))
            {
                return;
            }

            renderer.material.color = color;
        }
    }
}
