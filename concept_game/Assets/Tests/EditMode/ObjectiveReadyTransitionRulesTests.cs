using MossHarbor.Expedition;
using NUnit.Framework;

namespace MossHarbor.Tests.EditMode
{
    public sealed class ObjectiveReadyTransitionRulesTests
    {
        [Test]
        public void ResolveHazardMultiplier_ReturnsGraceMultiplierInsideWindow()
        {
            var multiplier = ObjectiveReadyTransitionRules.ResolveHazardMultiplier(true, 0.9f, 2.25f, 0.45f);

            Assert.AreEqual(0.45f, multiplier, 0.001f);
        }

        [Test]
        public void ResolveHazardMultiplier_ReturnsFullStrengthAfterWindow()
        {
            var multiplier = ObjectiveReadyTransitionRules.ResolveHazardMultiplier(true, 2.4f, 2.25f, 0.45f);

            Assert.AreEqual(1f, multiplier, 0.001f);
        }
    }
}
