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
        [SerializeField] private Transform pulseVisual;

        private Vector3 _baseScale;
        private float _nextTriggerTime;
        private ExpeditionDirector _director;

        public void Configure(Vector3 direction, float strength, float lift, Transform visual = null)
        {
            boostDirection = direction;
            boostStrength = strength;
            verticalLift = lift;
            pulseVisual = visual;
        }

        private void Start()
        {
            if (pulseVisual == null)
            {
                pulseVisual = transform;
            }

            _baseScale = pulseVisual.localScale;
            _director = FindFirstObjectByType<ExpeditionDirector>();
        }

        private void Update()
        {
            if (pulseVisual == null)
            {
                return;
            }

            var pulse = Mathf.Lerp(1f, pulseScale, (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI) + 1f) * 0.5f);
            pulseVisual.localScale = new Vector3(_baseScale.x * pulse, _baseScale.y, _baseScale.z * pulse);
        }

        public bool TryBoostPlayer(PlayerController player)
        {
            if (Time.time < _nextTriggerTime)
            {
                return false;
            }

            if (player == null)
            {
                return false;
            }

            var worldDirection = Vector3.ProjectOnPlane(boostDirection, Vector3.up);
            if (worldDirection.sqrMagnitude < 0.001f)
            {
                worldDirection = transform.forward;
            }

            player.ApplyExternalImpulse(worldDirection.normalized * boostStrength + Vector3.up * verticalLift);
            _director ??= FindFirstObjectByType<ExpeditionDirector>();
            _director?.RegisterBoostPadUse(name);
            _nextTriggerTime = Time.time + cooldownSeconds;
            return true;
        }

        public void TryBoostCollider(Collider other)
        {
            if (other != null && other.TryGetComponent<PlayerController>(out var player))
            {
                TryBoostPlayer(player);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            TryBoostCollider(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TryBoostCollider(other);
        }
    }
}
