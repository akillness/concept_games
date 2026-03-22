using MossHarbor.Data;
using MossHarbor.Gameplay.Player;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class ExpeditionCameraDirector : MonoBehaviour
    {
        [SerializeField] private Vector3 followOffset = new(0f, 18f, -12f);
        [SerializeField] private float followSmooth = 7f;
        [SerializeField] private float cueBlendSpeed = 8f;
        [SerializeField] private float pickupCueCooldown = 1.25f;
        [SerializeField] private float elevatedFollowHeightBonus = 2.4f;
        [SerializeField] private float elevatedFollowPullback = 2.2f;

        private PlayerController _player;
        private Transform _beacon;
        private CameraCue _activeCue;
        private float _cueEndTime;
        private float _nextPickupCueTime;

        private enum CameraCue
        {
            Follow,
            PickupFocus,
            ObjectiveReady,
            BeaconActivate,
        }

        public void Configure(PlayerController player, Transform beacon)
        {
            _player = player;
            _beacon = beacon;
        }

        public void SetBeacon(Transform beacon)
        {
            _beacon = beacon;
        }

        public void RegisterPickupCue(ResourceType resourceType, Transform collector = null)
        {
            if (Time.time < _nextPickupCueTime)
            {
                return;
            }

            if (resourceType != ResourceType.SeedPod && resourceType != ResourceType.BloomDust)
            {
                return;
            }

            if (collector != null && collector.TryGetComponent<PlayerController>(out var player))
            {
                _player = player;
            }

            _activeCue = CameraCue.PickupFocus;
            _cueEndTime = Time.time + 0.65f;
            _nextPickupCueTime = Time.time + pickupCueCooldown;
        }

        public void PlayObjectiveReadyCue(Transform beacon)
        {
            _beacon = beacon != null ? beacon : _beacon;
            _activeCue = CameraCue.ObjectiveReady;
            _cueEndTime = Time.time + 1.25f;
        }

        public void PlayBeaconActivatedCue()
        {
            _activeCue = CameraCue.BeaconActivate;
            _cueEndTime = Time.time + 1.1f;
        }

        private void LateUpdate()
        {
            if (_player == null)
            {
                _player = FindFirstObjectByType<PlayerController>();
                if (_player == null)
                {
                    return;
                }
            }

            if (_activeCue != CameraCue.Follow && Time.time > _cueEndTime)
            {
                _activeCue = CameraCue.Follow;
            }

            ResolvePose(out var targetPosition, out var targetRotation);
            var blend = (_activeCue == CameraCue.Follow ? followSmooth : cueBlendSpeed) * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, 1f - Mathf.Exp(-blend));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f - Mathf.Exp(-blend));
        }

        private void ResolvePose(out Vector3 position, out Quaternion rotation)
        {
            switch (_activeCue)
            {
                case CameraCue.PickupFocus:
                    ResolvePickupFocus(out position, out rotation);
                    break;
                case CameraCue.ObjectiveReady:
                    ResolveObjectiveReveal(out position, out rotation);
                    break;
                case CameraCue.BeaconActivate:
                    ResolveBeaconActivation(out position, out rotation);
                    break;
                default:
                    ResolveFollow(out position, out rotation);
                    break;
            }
        }

        private void ResolveFollow(out Vector3 position, out Quaternion rotation)
        {
            var focus = _player.transform.position + Vector3.up * 1.4f;
            var facing = ResolveFacing();
            var elevatedBlend = ResolveElevationBlend();
            var adaptiveOffset = followOffset
                + Vector3.up * (elevatedBlend * elevatedFollowHeightBonus)
                - facing * (elevatedBlend * elevatedFollowPullback);
            var lookTarget = focus + facing * Mathf.Lerp(2.5f, 4.4f, elevatedBlend) + Vector3.up * (elevatedBlend * 0.45f);
            position = focus + adaptiveOffset;
            rotation = Quaternion.LookRotation((lookTarget - position).normalized, Vector3.up);
        }

        private void ResolvePickupFocus(out Vector3 position, out Quaternion rotation)
        {
            var facing = _player.FacingDirection;
            var focus = _player.transform.position + Vector3.up * 1.35f;
            position = focus - facing * 2.2f + Vector3.up * 0.55f;
            var lookTarget = focus + facing * 4.2f;
            rotation = Quaternion.LookRotation((lookTarget - position).normalized, Vector3.up);
        }

        private void ResolveObjectiveReveal(out Vector3 position, out Quaternion rotation)
        {
            var beaconPosition = _beacon != null ? _beacon.position : _player.transform.position + _player.FacingDirection * 8f;
            var dirToBeacon = Vector3.ProjectOnPlane(beaconPosition - _player.transform.position, Vector3.up).normalized;
            if (dirToBeacon.sqrMagnitude < 0.001f)
            {
                dirToBeacon = ResolveFacing();
            }

            var focus = _player.transform.position + Vector3.up * 1.8f;
            var shoulder = Vector3.Cross(Vector3.up, dirToBeacon).normalized;
            position = focus - dirToBeacon * 2.35f + shoulder * 0.9f + Vector3.up * 0.62f;
            var lookTarget = Vector3.Lerp(focus + dirToBeacon * 2f, beaconPosition + Vector3.up * 1.1f, 0.78f);
            rotation = Quaternion.LookRotation((lookTarget - position).normalized, Vector3.up);
        }

        private void ResolveBeaconActivation(out Vector3 position, out Quaternion rotation)
        {
            var facing = ResolveFacing();
            var focus = _player.transform.position + Vector3.up * 1.5f;
            position = focus - facing * 1.7f + Vector3.up * 0.68f + Vector3.right * 0.55f;
            var lookTarget = focus + facing * 5.1f + Vector3.up * 0.3f;
            rotation = Quaternion.LookRotation((lookTarget - position).normalized, Vector3.up);
        }

        private Vector3 ResolveFacing()
        {
            var facing = _player.FacingDirection;
            if (facing.sqrMagnitude <= 0.001f)
            {
                return Vector3.forward;
            }

            return facing.normalized;
        }

        private float ResolveElevationBlend()
        {
            return Mathf.InverseLerp(0.35f, 2.6f, _player.transform.position.y);
        }
    }
}
