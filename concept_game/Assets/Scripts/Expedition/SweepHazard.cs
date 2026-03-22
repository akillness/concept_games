using MossHarbor.Gameplay.Player;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class SweepHazard : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 60f;
        [SerializeField] private float pushStrength = 8f;
        [SerializeField] private float repulseCooldown = 0.35f;

        private Transform _pivot;
        private float _nextRepulseTime;
        private ExpeditionDirector _director;

        public void Configure(Transform pivot, float nextRotateSpeed, float nextPushStrength)
        {
            _pivot = pivot;
            rotateSpeed = nextRotateSpeed;
            pushStrength = nextPushStrength;
        }

        private void Update()
        {
            if (_pivot == null)
            {
                return;
            }

            _director ??= FindFirstObjectByType<ExpeditionDirector>();

            transform.RotateAround(_pivot.position, Vector3.up, rotateSpeed * Time.deltaTime);
        }

        private void OnTriggerStay(Collider other)
        {
            if (Time.time < _nextRepulseTime)
            {
                return;
            }

            if (!other.TryGetComponent<PlayerController>(out var player))
            {
                return;
            }

            var pushDirection = Vector3.ProjectOnPlane(other.transform.position - (_pivot != null ? _pivot.position : transform.position), Vector3.up);
            if (pushDirection.sqrMagnitude < 0.01f)
            {
                pushDirection = transform.right;
            }

            var hazardMultiplier = _director != null ? _director.GetObjectiveReadyHazardMultiplier() : 1f;
            player.ApplyExternalImpulse(pushDirection.normalized * (pushStrength * hazardMultiplier) + Vector3.up * (1.8f * hazardMultiplier));
            _nextRepulseTime = Time.time + repulseCooldown;
        }
    }
}
