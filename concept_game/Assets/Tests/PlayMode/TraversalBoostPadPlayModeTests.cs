using System.Collections;
using System.Reflection;
using MossHarbor.Data;
using MossHarbor.Expedition;
using MossHarbor.Gameplay.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace MossHarbor.Tests.PlayMode
{
    public sealed class TraversalBoostPadPlayModeTests
    {
        [UnityTest]
        public IEnumerator WestBoostPad_LaunchesPlayerTowardUpperLanding()
        {
            var root = new GameObject("TraversalBoostPadPlayModeRoot");
            GameObject player = null;
            try
            {
                var district = ScriptableObject.CreateInstance<DistrictDef>();
                district.pickupSpawnRadius = 8f;
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);
                ExpeditionLevelLayoutBuilder.Build(root.transform, plan, Color.cyan);

                var startPosition = new Vector3(-plan.halfWidth * 0.44f, 0.32f, plan.halfLength * 0.04f - 2.6f);
                player = CreatePlayer(startPosition);
                yield return null;

                var controller = player.GetComponent<CharacterController>();
                Assert.That(controller, Is.Not.Null);

                for (var frame = 0; frame < 90; frame++)
                {
                    controller.Move(Vector3.forward * 0.18f);
                    yield return null;
                }

                var peakY = player.transform.position.y;
                var peakZ = player.transform.position.z;
                for (var frame = 0; frame < 120; frame++)
                {
                    peakY = Mathf.Max(peakY, player.transform.position.y);
                    peakZ = Mathf.Max(peakZ, player.transform.position.z);
                    yield return null;
                }

                Assert.GreaterOrEqual(peakY, plan.elevatedHeight - 0.15f);
                Assert.GreaterOrEqual(peakZ, plan.halfLength * 0.38f);
            }
            finally
            {
                if (player != null)
                {
                    SafeDestroy(player);
                }

                SafeDestroy(root);
            }
        }

        private static GameObject CreatePlayer(Vector3 position)
        {
            var player = new GameObject("TraversalBoostPadTestPlayer");
            player.transform.position = position;
            player.AddComponent<CharacterController>();

            var modelRoot = new GameObject("ModelRoot").transform;
            modelRoot.SetParent(player.transform, false);

            var controller = player.AddComponent<PlayerController>();
            SetPrivateField(controller, "modelRoot", modelRoot);
            SetPrivateField(controller, "enableBoundaryRecovery", false);
            return player;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Missing private field: {fieldName}");
            field.SetValue(target, value);
        }

        private static void SafeDestroy(Object target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(target);
                return;
            }

            Object.DestroyImmediate(target);
        }
    }
}
