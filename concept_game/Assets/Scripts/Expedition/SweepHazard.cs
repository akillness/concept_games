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

            player.ApplyExternalImpulse(pushDirection.normalized * pushStrength + Vector3.up * 1.8f);
            _nextRepulseTime = Time.time + repulseCooldown;
        }
    }
}
