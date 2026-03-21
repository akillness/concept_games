using System.Collections.Generic;
using MossHarbor.Data;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class ObjectiveService
    {
        private readonly DistrictDef _district;
        private readonly float _holdSecondsOverride;
        private readonly Dictionary<ResourceType, int> _resourceTotals = new();
        private int _pickupCount;
        private float _elapsedSeconds;

        public ObjectiveService(DistrictDef district, float holdSecondsOverride = 0f)
        {
            _district = district;
            _holdSecondsOverride = holdSecondsOverride;
            RecalculateCompletion();
        }

        public ExpeditionObjectiveType ObjectiveType => _district != null ? _district.objectiveType : ExpeditionObjectiveType.CollectPickups;
        public ResourceType ObjectiveResourceType => _district != null ? _district.objectiveResourceType : ResourceType.BloomDust;
        public int PickupCount => _pickupCount;
        public float ElapsedSeconds => _elapsedSeconds;
        public bool IsComplete { get; private set; }

        public int TargetAmount
        {
            get
            {
                if (_district == null)
                {
                    return 3;
                }

                if (_district.objectiveTargetAmount > 0)
                {
                    return _district.objectiveTargetAmount;
                }

                return Mathf.Max(1, _district.targetPickupCount);
            }
        }

        public float TargetHoldSeconds
        {
            get
            {
                if (_holdSecondsOverride > 0f)
                {
                    return _holdSecondsOverride;
                }

                if (_district == null)
                {
                    return 60f;
                }

                if (_district.objectiveHoldSeconds > 0f)
                {
                    return _district.objectiveHoldSeconds;
                }

                return Mathf.Max(30f, _district.runTimerSeconds * 0.5f);
            }
        }

        public void RegisterCollection(ResourceType resourceType, int amount)
        {
            _pickupCount++;
            _resourceTotals[resourceType] = GetCollectedAmount(resourceType) + Mathf.Max(0, amount);
            RecalculateCompletion();
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f || IsComplete)
            {
                return;
            }

            _elapsedSeconds += deltaTime;
            RecalculateCompletion();
        }

        public int GetCollectedAmount(ResourceType resourceType)
        {
            return _resourceTotals.TryGetValue(resourceType, out var amount) ? amount : 0;
        }

        public string GetInstructionText(string districtName, string fallbackObjectiveText)
        {
            switch (ObjectiveType)
            {
                case ExpeditionObjectiveType.CollectResource:
                    return $"Recover {TargetAmount} {ObjectiveResourceType} from {districtName} before returning.";
                case ExpeditionObjectiveType.HoldOut:
                    return $"Hold the route in {districtName} for {TargetHoldSeconds:0} seconds, then fall back to the beacon.";
                default:
                    return fallbackObjectiveText;
            }
        }

        public string GetProgressText()
        {
            switch (ObjectiveType)
            {
                case ExpeditionObjectiveType.CollectResource:
                    return $"{ObjectiveResourceType}: {GetCollectedAmount(ObjectiveResourceType)} / {TargetAmount}";
                case ExpeditionObjectiveType.HoldOut:
                    return $"Holdout: {Mathf.Clamp(_elapsedSeconds, 0f, TargetHoldSeconds):0} / {TargetHoldSeconds:0}s";
                default:
                    return $"Pickups: {PickupCount} / {TargetAmount}";
            }
        }

        private void RecalculateCompletion()
        {
            switch (ObjectiveType)
            {
                case ExpeditionObjectiveType.CollectResource:
                    IsComplete = GetCollectedAmount(ObjectiveResourceType) >= TargetAmount;
                    break;
                case ExpeditionObjectiveType.HoldOut:
                    IsComplete = _elapsedSeconds >= TargetHoldSeconds;
                    break;
                default:
                    IsComplete = PickupCount >= TargetAmount;
                    break;
            }
        }
    }
}
