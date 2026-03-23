using System.Collections;
using System.Reflection;
using MossHarbor.Gameplay.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace MossHarbor.Tests.PlayMode
{
    public sealed class PlayerExternalImpulsePlayModeTests
    {
        [UnityTest]
        public IEnumerator ApplyExternalImpulse_WhenAccumulationDisabled_ReplacesHorizontalLaunchInsteadOfStacking()
        {
            var player = CreatePlayer(Vector3.zero);
            try
            {
                yield return null;
                yield return null;

                var controller = player.GetComponent<PlayerController>();
                Assert.That(controller, Is.Not.Null);

                controller.ApplyExternalImpulse(new Vector3(4f, 0.45f, 0f), accumulate: false);
                yield return null;
                controller.ApplyExternalImpulse(new Vector3(0f, 0.45f, 4f), accumulate: false);
                yield return null;

                var externalVelocity = (Vector3)GetPrivateField(controller, "_externalVelocity");
                var verticalSpeed = (float)GetPrivateField(controller, "_verticalSpeed");

                Assert.That(externalVelocity.magnitude, Is.LessThanOrEqualTo(4.05f));
                Assert.That(externalVelocity.z, Is.GreaterThan(3.8f));
                Assert.That(Mathf.Abs(externalVelocity.x), Is.LessThan(0.2f));
                Assert.That(verticalSpeed, Is.LessThanOrEqualTo(0.5f));
            }
            finally
            {
                SafeDestroy(player);
            }
        }

        private static GameObject CreatePlayer(Vector3 position)
        {
            var player = new GameObject("ExternalImpulseTestPlayer");
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

        private static object GetPrivateField(object target, string fieldName)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Missing private field: {fieldName}");
            return field.GetValue(target);
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
