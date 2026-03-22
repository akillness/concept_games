using MossHarbor.Data;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public readonly struct ExpeditionPickupRouteProfile
    {
        public ExpeditionPickupRouteProfile(bool isPriorityRoute, bool isElevatedRoute, float scaleMultiplier, int adjustedAmount, string tierLabel)
        {
            IsPriorityRoute = isPriorityRoute;
            IsElevatedRoute = isElevatedRoute;
            ScaleMultiplier = scaleMultiplier;
            AdjustedAmount = adjustedAmount;
            TierLabel = tierLabel;
        }

        public bool IsPriorityRoute { get; }
        public bool IsElevatedRoute { get; }
        public float ScaleMultiplier { get; }
        public int AdjustedAmount { get; }
        public string TierLabel { get; }
    }

    public static class ExpeditionPickupRouteRules
    {
        public static ExpeditionPickupRouteProfile Resolve(Vector3 position, ExpeditionLevelLayoutPlan plan, ResourceType resourceType, int baseAmount)
        {
            if (plan == null)
            {
                return new ExpeditionPickupRouteProfile(false, false, 1f, baseAmount, "core");
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
                    1.32f,
                    AdjustAmount(baseAmount, resourceType, 2),
                    "elevated");
            }

            if (isSideLaneRoute)
            {
                return new ExpeditionPickupRouteProfile(
                    true,
                    false,
                    1.18f,
                    AdjustAmount(baseAmount, resourceType, 1),
                    "side-lane");
            }

            return new ExpeditionPickupRouteProfile(false, false, 1f, baseAmount, "core");
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
