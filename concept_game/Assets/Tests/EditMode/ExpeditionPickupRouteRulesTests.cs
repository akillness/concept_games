using MossHarbor.Data;
using MossHarbor.Expedition;
using NUnit.Framework;
using UnityEngine;

namespace MossHarbor.Tests.EditMode
{
    public sealed class ExpeditionPickupRouteRulesTests
    {
        [Test]
        public void Resolve_ElevatedRoute_IncreasesAmountAndMarksPriority()
        {
            var plan = new ExpeditionLevelLayoutPlan
            {
                halfWidth = 20f,
                halfLength = 28f,
                elevatedHeight = 2.35f
            };

            var profile = ExpeditionPickupRouteRules.Resolve(new Vector3(0f, 2.35f, 12f), plan, ResourceType.BloomDust, 10);

            Assert.That(profile.IsPriorityRoute, Is.True);
            Assert.That(profile.IsElevatedRoute, Is.True);
            Assert.That(profile.AdjustedAmount, Is.GreaterThan(10));
            Assert.That(profile.ScaleMultiplier, Is.GreaterThan(1f));
            Assert.That(profile.SignalScaleMultiplier, Is.GreaterThan(1f));
        }

        [Test]
        public void Resolve_SideLaneRoute_IncreasesAmountWithoutElevatedFlag()
        {
            var plan = new ExpeditionLevelLayoutPlan
            {
                halfWidth = 20f,
                halfLength = 28f,
                elevatedHeight = 2.35f
            };

            var profile = ExpeditionPickupRouteRules.Resolve(new Vector3(11f, 0.8f, 6f), plan, ResourceType.Scrap, 4);

            Assert.That(profile.IsPriorityRoute, Is.True);
            Assert.That(profile.IsElevatedRoute, Is.False);
            Assert.That(profile.AdjustedAmount, Is.GreaterThanOrEqualTo(8));
            Assert.That(profile.TierLabel, Is.EqualTo("side-lane"));
            Assert.That(profile.SignalScaleMultiplier, Is.GreaterThan(1f));
        }

        [Test]
        public void Resolve_CoreRoute_LeavesAmountUntouched()
        {
            var plan = new ExpeditionLevelLayoutPlan
            {
                halfWidth = 20f,
                halfLength = 28f,
                elevatedHeight = 2.35f
            };

            var profile = ExpeditionPickupRouteRules.Resolve(new Vector3(0f, 0.8f, 1f), plan, ResourceType.SeedPod, 3);

            Assert.That(profile.IsPriorityRoute, Is.False);
            Assert.That(profile.AdjustedAmount, Is.EqualTo(3));
            Assert.That(profile.ScaleMultiplier, Is.EqualTo(1f));
        }
    }
}
