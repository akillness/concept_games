using MossHarbor.Data;
using MossHarbor.Expedition;
using NUnit.Framework;

namespace MossHarbor.Tests.EditMode
{
    public sealed class ObjectiveServiceTests
    {
        [Test]
        public void CollectPickups_CompletesAtTargetCount()
        {
            var district = ScriptableObjectFactory.CreateDistrict(ExpeditionObjectiveType.CollectPickups, ResourceType.BloomDust, 3, 45f);
            var service = new ObjectiveService(district);

            service.RegisterCollection(ResourceType.BloomDust, 1);
            service.RegisterCollection(ResourceType.Scrap, 1);
            Assert.IsFalse(service.IsComplete);

            service.RegisterCollection(ResourceType.BloomDust, 1);
            Assert.IsTrue(service.IsComplete);
        }

        [Test]
        public void CollectResource_CompletesWhenTargetResourceAmountReached()
        {
            var district = ScriptableObjectFactory.CreateDistrict(ExpeditionObjectiveType.CollectResource, ResourceType.Scrap, 12, 45f);
            var service = new ObjectiveService(district);

            service.RegisterCollection(ResourceType.BloomDust, 10);
            service.RegisterCollection(ResourceType.Scrap, 6);
            Assert.IsFalse(service.IsComplete);

            service.RegisterCollection(ResourceType.Scrap, 6);
            Assert.IsTrue(service.IsComplete);
        }

        [Test]
        public void HoldOut_CompletesWhenEnoughTimePasses()
        {
            var district = ScriptableObjectFactory.CreateDistrict(ExpeditionObjectiveType.HoldOut, ResourceType.BloomDust, 1, 20f);
            var service = new ObjectiveService(district);

            service.Tick(10f);
            Assert.IsFalse(service.IsComplete);

            service.Tick(10f);
            Assert.IsTrue(service.IsComplete);
        }

        [Test]
        public void HoldOut_UsesOverrideHoldSecondsWhenProvided()
        {
            var district = ScriptableObjectFactory.CreateDistrict(ExpeditionObjectiveType.HoldOut, ResourceType.BloomDust, 1, 20f);
            var service = new ObjectiveService(district, 5f);

            service.Tick(4f);
            Assert.IsFalse(service.IsComplete);

            service.Tick(1f);
            Assert.IsTrue(service.IsComplete);
            Assert.AreEqual("Holdout: 5 / 5s", service.GetProgressText());
        }

        private static class ScriptableObjectFactory
        {
            public static DistrictDef CreateDistrict(ExpeditionObjectiveType objectiveType, ResourceType objectiveResourceType, int objectiveTargetAmount, float objectiveHoldSeconds)
            {
                var district = UnityEngine.ScriptableObject.CreateInstance<DistrictDef>();
                district.objectiveType = objectiveType;
                district.objectiveResourceType = objectiveResourceType;
                district.objectiveTargetAmount = objectiveTargetAmount;
                district.objectiveHoldSeconds = objectiveHoldSeconds;
                district.targetPickupCount = objectiveTargetAmount;
                district.runTimerSeconds = 180f;
                return district;
            }
        }
    }
}
