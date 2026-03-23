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
            var probe = Mathf.Max(0.01f, probeRadius);
            if (!Physics.SphereCast(focus, probe, direction, out var hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                return desiredPosition;
            }

            var right = Vector3.Cross(Vector3.up, direction);
            if (right.sqrMagnitude < 0.0001f)
            {
                right = Vector3.Cross(Vector3.forward, direction);
            }

            if (right.sqrMagnitude < 0.0001f)
            {
                right = Vector3.right;
            }

            right.Normalize();

            var shoulderStep = Mathf.Max(probe + padding, 0.75f);
            var heightStep = Mathf.Max(probe + padding, 0.6f);

            if (TryResolveCandidate(focus, direction, distance, probe, right * shoulderStep, out var resolvedPosition))
            {
                return resolvedPosition;
            }

            if (TryResolveCandidate(focus, direction, distance, probe, -right * shoulderStep, out resolvedPosition))
            {
                return resolvedPosition;
            }

            if (TryResolveCandidate(focus, direction, distance, probe, Vector3.up * heightStep, out resolvedPosition))
            {
                return resolvedPosition;
            }

            if (TryResolveCandidate(focus, direction, distance, probe, right * shoulderStep + Vector3.up * heightStep, out resolvedPosition))
            {
                return resolvedPosition;
            }

            if (TryResolveCandidate(focus, direction, distance, probe, -right * shoulderStep + Vector3.up * heightStep, out resolvedPosition))
            {
                return resolvedPosition;
            }

            if (TryResolveCandidate(focus, direction, distance, probe, right * (shoulderStep * 1.5f), out resolvedPosition))
            {
                return resolvedPosition;
            }

            if (TryResolveCandidate(focus, direction, distance, probe, -right * (shoulderStep * 1.5f), out resolvedPosition))
            {
                return resolvedPosition;
            }

            if (TryResolveCandidate(focus, direction, distance, probe, Vector3.up * (heightStep * 1.5f), out resolvedPosition))
            {
                return resolvedPosition;
            }

            if (TryResolveCandidate(focus, direction, distance, probe, right * (shoulderStep * 1.5f) + Vector3.up * (heightStep * 1.25f), out resolvedPosition))
            {
                return resolvedPosition;
            }

            if (TryResolveCandidate(focus, direction, distance, probe, -right * (shoulderStep * 1.5f) + Vector3.up * (heightStep * 1.25f), out resolvedPosition))
            {
                return resolvedPosition;
            }

            var resolvedDistance = Mathf.Clamp(hit.distance - padding, minimumDistance, distance);
            return focus + direction * resolvedDistance;
        }

        private static bool TryResolveCandidate(
            Vector3 focus,
            Vector3 direction,
            float distance,
            float probeRadius,
            Vector3 offset,
            out Vector3 resolvedPosition)
        {
            var candidateDirection = direction + offset;
            if (candidateDirection.sqrMagnitude < 0.0001f)
            {
                resolvedPosition = focus;
                return false;
            }

            candidateDirection.Normalize();
            if (Physics.SphereCast(focus, probeRadius, candidateDirection, out _, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                resolvedPosition = focus;
                return false;
            }

            resolvedPosition = focus + candidateDirection * distance;
            return true;
        }
    }
}
