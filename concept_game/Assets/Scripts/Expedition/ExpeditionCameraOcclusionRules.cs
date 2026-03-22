using UnityEngine;

namespace MossHarbor.Expedition
{
    public static class ExpeditionCameraOcclusionRules
    {
        public static Vector3 ResolveAdjustedPosition(
            Vector3 focus,
            Vector3 desiredPosition,
            float probeRadius,
            float padding,
            float minimumDistance)
        {
            var toDesired = desiredPosition - focus;
            var distance = toDesired.magnitude;
            if (distance <= 0.001f)
            {
                return desiredPosition;
            }

            var direction = toDesired / distance;
            if (!Physics.SphereCast(focus, Mathf.Max(0.01f, probeRadius), direction, out var hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                return desiredPosition;
            }

            var resolvedDistance = Mathf.Clamp(hit.distance - padding, minimumDistance, distance);
            return focus + direction * resolvedDistance;
        }
    }
}
