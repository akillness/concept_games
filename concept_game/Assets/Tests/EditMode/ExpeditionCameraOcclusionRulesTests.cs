using MossHarbor.Expedition;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MossHarbor.Tests.EditMode
{
    public sealed class ExpeditionCameraOcclusionRulesTests
    {
        [Test]
        public void ResolveAdjustedPosition_PrefersShoulderOffsetWhenItKeepsTheRouteReadable()
        {
            var blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            try
            {
                blocker.transform.position = new Vector3(0f, 1.45f, -3f);
                blocker.transform.localScale = new Vector3(1.6f, 1.6f, 0.6f);
                Physics.SyncTransforms();

                var focus = new Vector3(0f, 1.45f, 0f);
                var desired = new Vector3(0f, 1.45f, -8f);

                var resolved = ExpeditionCameraOcclusionRules.ResolveAdjustedPosition(focus, desired, 0.2f, 0.25f, 2f);

                Assert.That(Mathf.Abs(resolved.x), Is.GreaterThan(1f));
                Assert.That(resolved.y, Is.EqualTo(focus.y).Within(0.01f));
                Assert.That(resolved.z, Is.LessThan(-4f));
                Assert.That(resolved.z, Is.GreaterThan(desired.z));
            }
            finally
            {
                Object.DestroyImmediate(blocker);
            }
        }

        [Test]
        public void ResolveAdjustedPosition_LiftsCameraWhenShoulderOffsetsAreBlocked()
        {
            var blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            try
            {
                blocker.transform.position = new Vector3(0f, 1.45f, -3f);
                blocker.transform.localScale = new Vector3(6.8f, 1.2f, 0.8f);
                Physics.SyncTransforms();

                var focus = new Vector3(0f, 1.45f, 0f);
                var desired = new Vector3(0f, 1.45f, -8f);

                var resolved = ExpeditionCameraOcclusionRules.ResolveAdjustedPosition(focus, desired, 0.2f, 0.25f, 2f);

                Assert.That(Mathf.Abs(resolved.x), Is.LessThan(0.5f));
                Assert.That(resolved.y, Is.GreaterThan(2.1f));
                Assert.That(resolved.z, Is.LessThan(-4f));
            }
            finally
            {
                Object.DestroyImmediate(blocker);
            }
        }

        [Test]
        public void ResolveAdjustedPosition_PullsCameraForwardWhenEveryCandidateIsBlocked()
        {
            var blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            try
            {
                blocker.transform.position = new Vector3(0f, 1.45f, -3f);
                blocker.transform.localScale = new Vector3(12f, 8f, 0.8f);
                Physics.SyncTransforms();

                var focus = new Vector3(0f, 1.45f, 0f);
                var desired = new Vector3(0f, 1.45f, -8f);

                var resolved = ExpeditionCameraOcclusionRules.ResolveAdjustedPosition(focus, desired, 0.2f, 0.25f, 2f);

                Assert.That(resolved.z, Is.GreaterThan(desired.z));
                Assert.That(resolved.z, Is.LessThan(-1.5f));
            }
            finally
            {
                Object.DestroyImmediate(blocker);
            }
        }
    }
}
