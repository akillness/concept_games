using MossHarbor.Data;
using NUnit.Framework;
using UnityEngine;

namespace MossHarbor.Tests.EditMode
{
    public sealed class DistrictContentCatalogTests
    {
        [Test]
        public void LoadByIndex_ReturnsMatchedDistrictBundle()
        {
            for (var index = 0; index < ContentPaths.DistrictCount; index++)
            {
                var bundle = DistrictContentCatalog.LoadByIndex(index);

                Assert.NotNull(bundle.District, $"Missing district for index {index}");
                Assert.NotNull(bundle.HubZone, $"Missing hub zone for index {index}");
                Assert.NotNull(bundle.Quest, $"Missing quest for index {index}");
                Assert.AreEqual(bundle.District.districtId, bundle.HubZone.districtId, $"Hub zone mismatch for index {index}");
                Assert.AreEqual(bundle.District.districtId, bundle.Quest.districtId, $"Quest mismatch for index {index}");
            }
        }

        [Test]
        public void LoadDefault_ReturnsDockBundle()
        {
            var bundle = DistrictContentCatalog.LoadDefault();

            Assert.NotNull(bundle.District);
            Assert.NotNull(bundle.HubZone);
            Assert.NotNull(bundle.Quest);
            Assert.AreEqual("dock", bundle.District.districtId);
            Assert.AreEqual(bundle.District.districtId, bundle.HubZone.districtId);
            Assert.AreEqual(bundle.District.districtId, bundle.Quest.districtId);
        }

        [Test]
        public void LoadByDistrictId_ReturnsRequestedBundle()
        {
            var bundle = DistrictContentCatalog.LoadByDistrictId("reed_fields");

            Assert.NotNull(bundle.District);
            Assert.NotNull(bundle.HubZone);
            Assert.NotNull(bundle.Quest);
            Assert.AreEqual("reed_fields", bundle.District.districtId);
            Assert.AreEqual(bundle.District.districtId, bundle.HubZone.districtId);
            Assert.AreEqual(bundle.District.districtId, bundle.Quest.districtId);
        }
    }
}
