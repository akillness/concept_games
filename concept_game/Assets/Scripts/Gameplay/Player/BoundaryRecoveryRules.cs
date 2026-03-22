using UnityEngine;

namespace MossHarbor.Gameplay.Player
{
    public static class BoundaryRecoveryRules
    {
        public static bool ShouldRecover(
            Vector3 currentPosition,
            Vector3 boundaryCenter,
            Vector2 boundaryHalfExtents,
            float floorY,
            float cooldownSeconds,
            float currentTime,
            float lastRecoveryTime)
        {
            if (cooldownSeconds > 0f && currentTime - lastRecoveryTime < cooldownSeconds)
            {
                return false;
            }

            return IsBelowFloor(currentPosition, floorY) || IsOutsideBounds(currentPosition, boundaryCenter, boundaryHalfExtents);
        }

        public static bool IsBelowFloor(Vector3 currentPosition, float floorY)
        {
            return currentPosition.y < floorY;
        }

        public static bool IsOutsideBounds(Vector3 currentPosition, Vector3 boundaryCenter, Vector2 boundaryHalfExtents)
        {
            var minX = boundaryCenter.x - Mathf.Abs(boundaryHalfExtents.x);
            var maxX = boundaryCenter.x + Mathf.Abs(boundaryHalfExtents.x);
            var minZ = boundaryCenter.z - Mathf.Abs(boundaryHalfExtents.y);
            var maxZ = boundaryCenter.z + Mathf.Abs(boundaryHalfExtents.y);

            return currentPosition.x < minX
                || currentPosition.x > maxX
                || currentPosition.z < minZ
                || currentPosition.z > maxZ;
        }

        public static Vector3 ResolveSafePosition(
            Vector3 desiredSafePosition,
            Vector3 boundaryCenter,
            Vector2 boundaryHalfExtents,
            float floorY,
            float safeHeightOffset)
        {
            var minX = boundaryCenter.x - Mathf.Abs(boundaryHalfExtents.x);
            var maxX = boundaryCenter.x + Mathf.Abs(boundaryHalfExtents.x);
            var minZ = boundaryCenter.z - Mathf.Abs(boundaryHalfExtents.y);
            var maxZ = boundaryCenter.z + Mathf.Abs(boundaryHalfExtents.y);
            var minY = floorY + Mathf.Max(0.1f, safeHeightOffset);

            return new Vector3(
                Mathf.Clamp(desiredSafePosition.x, minX, maxX),
                Mathf.Max(desiredSafePosition.y, minY),
                Mathf.Clamp(desiredSafePosition.z, minZ, maxZ));
        }
    }
}
