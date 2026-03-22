using MossHarbor.Gameplay.Player;
using NUnit.Framework;
using UnityEngine;

namespace MossHarbor.Tests.EditMode
{
    public sealed class BoundaryRecoveryRulesTests
    {
        [Test]
        public void ShouldRecover_ReturnsTrue_WhenOutsideAllowedArea()
        {
            var shouldRecover = BoundaryRecoveryRules.ShouldRecover(
                new Vector3(16f, 2f, 0f),
                Vector3.zero,
                new Vector2(10f, 10f),
                -5f,
                1f,
                10f,
                0f);

            Assert.IsTrue(shouldRecover);
        }

        [Test]
        public void ShouldRecover_ReturnsTrue_WhenBelowFloor()
        {
            var shouldRecover = BoundaryRecoveryRules.ShouldRecover(
                new Vector3(0f, -6f, 0f),
                Vector3.zero,
                new Vector2(10f, 10f),
                -5f,
                1f,
                10f,
                0f);

            Assert.IsTrue(shouldRecover);
        }

        [Test]
        public void ShouldRecover_ReturnsFalse_WhenWithinBoundsAndAboveFloor()
        {
            var shouldRecover = BoundaryRecoveryRules.ShouldRecover(
                new Vector3(2f, 1f, -3f),
                Vector3.zero,
                new Vector2(10f, 10f),
                -5f,
                1f,
                10f,
                0f);

            Assert.IsFalse(shouldRecover);
        }

        [Test]
        public void ShouldRecover_ReturnsFalse_DuringCooldown()
        {
            var shouldRecover = BoundaryRecoveryRules.ShouldRecover(
                new Vector3(16f, 1f, 0f),
                Vector3.zero,
                new Vector2(10f, 10f),
                -5f,
                2f,
                1f,
                0f);

            Assert.IsFalse(shouldRecover);
        }

        [Test]
        public void ResolveSafePosition_ClampsInsideBoundsAndAboveFloor()
        {
            var safePosition = BoundaryRecoveryRules.ResolveSafePosition(
                new Vector3(50f, -20f, -50f),
                Vector3.zero,
                new Vector2(10f, 10f),
                -5f,
                2f);

            Assert.That(safePosition.x, Is.EqualTo(10f));
            Assert.That(safePosition.y, Is.EqualTo(-3f));
            Assert.That(safePosition.z, Is.EqualTo(-10f));
        }
    }
}
