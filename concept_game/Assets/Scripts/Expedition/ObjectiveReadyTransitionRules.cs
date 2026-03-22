using UnityEngine;

namespace MossHarbor.Expedition
{
    public static class ObjectiveReadyTransitionRules
    {
        public static float ResolveHazardMultiplier(bool objectiveReady, float secondsSinceReady, float graceSeconds, float graceMultiplier)
        {
            if (!objectiveReady || secondsSinceReady < 0f || graceSeconds <= 0f)
            {
                return 1f;
            }

            if (secondsSinceReady >= graceSeconds)
            {
                return 1f;
            }

            return Mathf.Clamp(graceMultiplier, 0.05f, 1f);
        }
    }
}
