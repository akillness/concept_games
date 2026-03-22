using MossHarbor.Data;
using MossHarbor.Gameplay.Player;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class SimplePickup : MonoBehaviour
    {
        [SerializeField] private ResourceType resourceType = ResourceType.BloomDust;
        [SerializeField] private int amount = 5;
        [SerializeField] private float rotateSpeed = 60f;
        [SerializeField] private float bobHeight = 0.3f;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float approachScaleMultiplier = 1.3f;
        [SerializeField] private float approachRange = 3f;
        [SerializeField] private float collectShrinkDuration = 0.15f;
        [SerializeField] private ExpeditionRouteTier routeTier = ExpeditionRouteTier.Core;

        private ExpeditionDirector _director;
        private Vector3 _basePosition;
        private Vector3 _baseScale;
        private Transform _playerTransform;
        private bool _isCollecting;
        private float _collectTimer;

        private void Start()
        {
            _director = FindFirstObjectByType<ExpeditionDirector>();
            _basePosition = transform.position;
            _baseScale = transform.localScale;
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null)
                _playerTransform = player.transform;
        }

        public void Configure(ResourceType nextResourceType, int nextAmount, float nextRotateSpeed, ExpeditionRouteTier nextRouteTier = ExpeditionRouteTier.Core)
        {
            resourceType = nextResourceType;
            amount = nextAmount;
            rotateSpeed = nextRotateSpeed;
            routeTier = nextRouteTier;
        }

        private void Update()
        {
            if (_isCollecting)
            {
                _collectTimer += Time.deltaTime;
                var t = Mathf.Clamp01(_collectTimer / collectShrinkDuration);
                transform.localScale = Vector3.Lerp(_baseScale, Vector3.zero, t);
                if (t >= 1f)
                    Destroy(gameObject);
                return;
            }

            // Rotation
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

            // Vertical bob
            var bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = _basePosition + Vector3.up * bobOffset;

            // Scale on approach
            if (_playerTransform != null)
            {
                var distance = Vector3.Distance(transform.position, _playerTransform.position);
                var proximity = Mathf.Clamp01(1f - distance / approachRange);
                var scaleFactor = Mathf.Lerp(1f, approachScaleMultiplier, proximity);
                transform.localScale = _baseScale * scaleFactor;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isCollecting)
                return;

            if (!other.TryGetComponent<PlayerController>(out _))
                return;

            _director?.Collect(resourceType, amount, other.transform, routeTier);
            _isCollecting = true;
            _collectTimer = 0f;
        }
    }
}
