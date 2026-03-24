using System.Collections.Generic;
using System.Linq;
using MossHarbor.Data;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public static class ExpeditionPickupSpawnPlanner
    {
        public static Vector3[] BuildOrderedAnchors(ExpeditionLevelLayoutPlan plan, DistrictDef district = null)
        {
            if (plan == null || plan.pickupAnchors == null || plan.pickupAnchors.Length == 0)
            {
                return System.Array.Empty<Vector3>();
            }

            var core = new Queue<Vector3>();
            var side = new Queue<Vector3>();
            var elevated = new Queue<Vector3>();

            foreach (var anchor in plan.pickupAnchors)
            {
                switch (ExpeditionPickupRouteRules.Resolve(anchor, plan, ResourceType.BloomDust, 1).Tier)
                {
                    case ExpeditionRouteTier.Elevated:
                        elevated.Enqueue(anchor);
                        break;
                    case ExpeditionRouteTier.SideLane:
                        side.Enqueue(anchor);
                        break;
                    default:
                        core.Enqueue(anchor);
                        break;
                }
            }

            var ordered = new List<Vector3>(plan.pickupAnchors.Length);
            var groundQuota = ResolveGroundQuota(district, core.Count + side.Count);

            while (ordered.Count < groundQuota && (core.Count > 0 || side.Count > 0))
            {
                AppendNext(core, ordered, 2);
                AppendNext(side, ordered, 1);
                AppendNext(core, ordered, 1);
            }

            while (core.Count > 0 || side.Count > 0 || elevated.Count > 0)
            {
                AppendNext(core, ordered, 1);
                AppendNext(side, ordered, 1);
                AppendNext(elevated, ordered, 1);
            }

            return ordered.Count > 0 ? ordered.ToArray() : plan.pickupAnchors.ToArray();
        }

        private static int ResolveGroundQuota(DistrictDef district, int availableGroundAnchors)
        {
            if (district == null)
            {
                return Mathf.Min(availableGroundAnchors, 4);
            }

            var totalPickups = Mathf.Max(0, district.bloomPickupCount + district.scrapPickupCount + district.seedPodPickupCount);
            if (totalPickups == 0 || availableGroundAnchors == 0)
            {
                return 0;
            }

            var requiredPickupSlots = district.objectiveType switch
            {
                ExpeditionObjectiveType.CollectPickups => district.targetPickupCount,
                ExpeditionObjectiveType.CollectResource => ResolveObjectiveResourcePickupSlots(district),
                _ => Mathf.Min(totalPickups, 5),
            };

            return Mathf.Clamp(requiredPickupSlots, 0, Mathf.Min(totalPickups, availableGroundAnchors));
        }

        private static int ResolveObjectiveResourcePickupSlots(DistrictDef district)
        {
            if (district == null)
            {
                return 0;
            }

            return district.objectiveResourceType switch
            {
                ResourceType.BloomDust => district.bloomPickupCount,
                ResourceType.Scrap => district.bloomPickupCount + district.scrapPickupCount,
                ResourceType.SeedPod => district.bloomPickupCount + district.scrapPickupCount + district.seedPodPickupCount,
                _ => Mathf.Min(district.bloomPickupCount + district.scrapPickupCount + district.seedPodPickupCount, 5),
            };
        }

        private static void AppendNext(Queue<Vector3> source, List<Vector3> destination, int count)
        {
            for (var index = 0; index < count && source.Count > 0; index++)
            {
                destination.Add(source.Dequeue());
            }
        }
    }
}
