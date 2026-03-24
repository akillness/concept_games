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
        public IEnumerator WestBoostPad_AppliesLaunchImpulseTowardUpperLanding()
        {
            var root = new GameObject("TraversalBoostPadPlayModeRoot");
            GameObject player = null;
            try
            {
                var district = ScriptableObject.CreateInstance<DistrictDef>();
                district.pickupSpawnRadius = 8f;
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);
                ExpeditionLevelLayoutBuilder.Build(root.transform, plan, Color.cyan);

                var startPosition = new Vector3(-plan.halfWidth * 0.44f, 0.6f, plan.halfLength * 0.04f);
                player = CreatePlayer(startPosition);
                yield return null;
                yield return null;

                var controller = player.GetComponent<CharacterController>();
                var playerController = player.GetComponent<PlayerController>();
                var boostPad = root.transform.Find("RuntimeLevelLayout/WestBoostPad")?.GetComponent<TraversalBoostPad>();
                Assert.That(controller, Is.Not.Null);
                Assert.That(playerController, Is.Not.Null);
                Assert.That(boostPad, Is.Not.Null);

                var boosted = boostPad.TryBoostPlayer(playerController);
                yield return null;

                Assert.That(boosted, Is.True);
                var verticalSpeed = (float)GetPrivateField(playerController, "_verticalSpeed");
                var externalVelocity = (Vector3)GetPrivateField(playerController, "_externalVelocity");
                Assert.GreaterOrEqual(verticalSpeed, 6.5f);
                Assert.LessOrEqual(verticalSpeed, 6.7f);
                Assert.GreaterOrEqual(externalVelocity.z, 18f);
                Assert.LessOrEqual(externalVelocity.z, 18.2f);
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

        [UnityTest]
        public IEnumerator WestBoostPad_ConfiguresReadableExitLandingAheadOfLaunch()
        {
            var root = new GameObject("TraversalBoostPadPlayModeRoot");
            GameObject player = null;
            try
            {
                var district = ScriptableObject.CreateInstance<DistrictDef>();
                district.pickupSpawnRadius = 8f;
                var plan = ExpeditionLevelLayoutBuilder.CreatePlan(district);
                ExpeditionLevelLayoutBuilder.Build(root.transform, plan, Color.cyan);

                var startPosition = new Vector3(-plan.halfWidth * 0.44f, 0.6f, plan.halfLength * 0.04f);
                player = CreatePlayer(startPosition);
                yield return null;
                yield return null;

                var playerController = player.GetComponent<PlayerController>();
                var boostPad = root.transform.Find("RuntimeLevelLayout/WestBoostPad")?.GetComponent<TraversalBoostPad>();
                var landing = root.transform.Find("RuntimeLevelLayout/WestBoostPadExitLanding");
                var landingMarker = root.transform.Find("RuntimeLevelLayout/WestBoostPadLandingMarker");
                Assert.That(playerController, Is.Not.Null);
                Assert.That(boostPad, Is.Not.Null);
                Assert.That(landing, Is.Not.Null);
                Assert.That(landingMarker, Is.Not.Null);

                boostPad.TryBoostPlayer(playerController);
                yield return null;

                var externalVelocity = (Vector3)GetPrivateField(playerController, "_externalVelocity");
                Assert.GreaterOrEqual(externalVelocity.z, 18f);
                Assert.LessOrEqual(externalVelocity.z, 18.2f);
                Assert.Greater(landing.position.z, startPosition.z + 6f);
                Assert.Greater(landingMarker.position.y, landing.position.y);
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
