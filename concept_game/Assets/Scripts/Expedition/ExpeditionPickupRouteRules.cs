using MossHarbor.Data;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public enum ExpeditionRouteTier
    {
        Core = 0,
        SideLane = 1,
        Elevated = 2,
    }

    public readonly struct ExpeditionPickupRouteProfile
    {
        public ExpeditionPickupRouteProfile(bool isPriorityRoute, bool isElevatedRoute, float scaleMultiplier, float signalScaleMultiplier, int adjustedAmount, ExpeditionRouteTier tier)
        {
            IsPriorityRoute = isPriorityRoute;
            IsElevatedRoute = isElevatedRoute;
            ScaleMultiplier = scaleMultiplier;
            SignalScaleMultiplier = signalScaleMultiplier;
            AdjustedAmount = adjustedAmount;
            Tier = tier;
        }

        public bool IsPriorityRoute { get; }
        public bool IsElevatedRoute { get; }
        public float ScaleMultiplier { get; }
        public float SignalScaleMultiplier { get; }
        public int AdjustedAmount { get; }
        public ExpeditionRouteTier Tier { get; }
        public string TierLabel => Tier switch
        {
            ExpeditionRouteTier.Elevated => "elevated",
            ExpeditionRouteTier.SideLane => "side-lane",
            _ => "core"
        };
    }

    public static class ExpeditionPickupRouteRules
    {
        public static ExpeditionPickupRouteProfile Resolve(Vector3 position, ExpeditionLevelLayoutPlan plan, ResourceType resourceType, int baseAmount)
        {
            if (plan == null)
            {
                return new ExpeditionPickupRouteProfile(false, false, 1f, 1f, baseAmount, ExpeditionRouteTier.Core);
            }

            var elevatedThreshold = Mathf.Max(1.25f, plan.elevatedHeight * 0.75f);
            var sideLaneThreshold = plan.halfWidth * 0.52f;
            var routeStartThreshold = plan.halfLength * 0.12f;

            var isElevatedRoute = position.y >= elevatedThreshold;
            var isSideLaneRoute = Mathf.Abs(position.x) >= sideLaneThreshold && position.z >= routeStartThreshold;

            if (isElevatedRoute)
            {
                return new ExpeditionPickupRouteProfile(
                    true,
                    true,
                    1.34f,
                    1.22f,
                    AdjustAmount(baseAmount, resourceType, 2),
                    ExpeditionRouteTier.Elevated);
            }

            if (isSideLaneRoute)
            {
                return new ExpeditionPickupRouteProfile(
                    true,
                    false,
                    1.24f,
                    1.18f,
                    AdjustAmount(baseAmount, resourceType, 2),
                    ExpeditionRouteTier.SideLane);
            }

            return new ExpeditionPickupRouteProfile(false, false, 1f, 1f, baseAmount, ExpeditionRouteTier.Core);
        }

        private static int AdjustAmount(int baseAmount, ResourceType resourceType, int bonusSteps)
        {
            var bonusPerStep = resourceType switch
            {
                ResourceType.BloomDust => 4,
                ResourceType.Scrap => 2,
                ResourceType.SeedPod => 1,
                _ => 1
            };

            return Mathf.Max(1, baseAmount + bonusPerStep * bonusSteps);
        }
    }
}
