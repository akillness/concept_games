using MossHarbor.Gameplay.Player;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class TraversalBoostPad : MonoBehaviour
    {
        [SerializeField] private Vector3 boostDirection = Vector3.forward;
        [SerializeField] private float boostStrength = 11f;
        [SerializeField] private float verticalLift = 4f;
        [SerializeField] private float cooldownSeconds = 0.75f;
        [SerializeField] private float pulseScale = 1.08f;
        [SerializeField] private float pulseSpeed = 2.4f;

        private Vector3 _baseScale;
        private float _nextTriggerTime;

        public void Configure(Vector3 direction, float strength, float lift)
        {
            boostDirection = direction;
            boostStrength = strength;
            verticalLift = lift;
        }

        private void Start()
        {
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            var pulse = Mathf.Lerp(1f, pulseScale, (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI) + 1f) * 0.5f);
            transform.localScale = new Vector3(_baseScale.x, _baseScale.y, _baseScale.z * pulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Time.time < _nextTriggerTime)
            {
                return;
            }

            if (!other.TryGetComponent<PlayerController>(out var player))
            {
                return;
            }

            var worldDirection = transform.TransformDirection(boostDirection.normalized);
            player.ApplyExternalImpulse(worldDirection * boostStrength + Vector3.up * verticalLift);
            _nextTriggerTime = Time.time + cooldownSeconds;
        }
    }
}
