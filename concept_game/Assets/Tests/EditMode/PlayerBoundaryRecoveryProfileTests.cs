using MossHarbor.Data;
using MossHarbor.Gameplay.Player;
using NUnit.Framework;
using UnityEngine;

namespace MossHarbor.Tests.EditMode
{
    public sealed class PlayerBoundaryRecoveryProfileTests
    {
        [Test]
        public void LoadDefaultDistrict_BakesBoundaryRecoveryProfileDefaults()
        {
            var bundle = DistrictContentCatalog.LoadDefault();
            var profile = bundle.District.boundaryRecovery;

            Assert.NotNull(profile);
            Assert.That(profile.boundaryCenter, Is.EqualTo(Vector3.zero));
            Assert.That(profile.boundaryHalfExtents, Is.EqualTo(new Vector2(180f, 180f)));
            Assert.That(profile.safePosition, Is.EqualTo(new Vector3(0f, 1f, 0f)));
            Assert.That(profile.floorY, Is.EqualTo(-12f));
        }

        [Test]
        public void ResolveBoundaryRecoveryProfile_ReturnsDistrictProfile()
        {
            var district = ScriptableObject.CreateInstance<DistrictDef>();
            district.boundaryRecovery = new BoundaryRecoveryProfile
            {
                boundaryCenter = new Vector3(9f, 2f, -4f),
                boundaryHalfExtents = new Vector2(22f, 18f),
                safePosition = new Vector3(3f, 6f, 7f),
                floorY = -9f,
            };

            var profile = PlayerController.ResolveBoundaryRecoveryProfile(district);

            Assert.That(profile.boundaryCenter, Is.EqualTo(new Vector3(9f, 2f, -4f)));
            Assert.That(profile.boundaryHalfExtents, Is.EqualTo(new Vector2(22f, 18f)));
            Assert.That(profile.safePosition, Is.EqualTo(new Vector3(3f, 6f, 7f)));
            Assert.That(profile.floorY, Is.EqualTo(-9f));
        }

        [Test]
        public void ResolveBoundaryRecoveryProfile_FallsBackToLegacyDefaults()
        {
            var profile = PlayerController.ResolveBoundaryRecoveryProfile(null);

            Assert.That(profile.boundaryCenter, Is.EqualTo(Vector3.zero));
            Assert.That(profile.boundaryHalfExtents, Is.EqualTo(new Vector2(180f, 180f)));
            Assert.That(profile.safePosition, Is.EqualTo(new Vector3(0f, 1f, 0f)));
            Assert.That(profile.floorY, Is.EqualTo(-12f));
        }
    }
}
