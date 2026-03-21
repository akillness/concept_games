using MossHarbor.Gameplay.Player;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class ObjectiveBeacon : MonoBehaviour
    {
        [SerializeField] private Color lockedColor = new(0.5f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color readyColor = new(0.2f, 0.9f, 0.6f, 1f);
        [SerializeField] private float readyRotateSpeed = 30f;
        [SerializeField] private float pulseMin = 1f;
        [SerializeField] private float pulseMax = 1.15f;
        [SerializeField] private float pulseSpeed = 2f;

        private ExpeditionDirector _director;
        private Renderer _renderer;
        private Vector3 _baseScale;

        private void Start()
        {
            _director = FindFirstObjectByType<ExpeditionDirector>();
            _renderer = GetComponentInChildren<Renderer>(true);
            _baseScale = transform.localScale;
            EnsureTriggerCollider();
            RefreshColor();
        }

        private void Update()
        {
            RefreshColor();

            if (_director != null && _director.ObjectiveReady)
            {
                // Rotate and pulse when ready
                transform.Rotate(Vector3.up, readyRotateSpeed * Time.deltaTime, Space.World);
                var pulse = Mathf.Lerp(pulseMin, pulseMax, (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI) + 1f) * 0.5f);
                transform.localScale = _baseScale * pulse;
            }
            else
            {
                // Static when locked
                transform.localScale = _baseScale;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<PlayerController>(out _))
                return;

            if (_director != null && _director.ObjectiveReady)
                _director.ActivateObjectiveBeacon();
        }

        private void RefreshColor()
        {
            if (_renderer == null || _director == null)
                return;

            _renderer.material.color = _director.ObjectiveReady ? readyColor : lockedColor;
        }

        public void SetTheme(Color districtColor)
        {
            lockedColor = Color.Lerp(districtColor, new Color(0.24f, 0.08f, 0.08f, 1f), 0.45f);
            readyColor = Color.Lerp(districtColor, Color.white, 0.35f);
            RefreshColor();
        }

        private void EnsureTriggerCollider()
        {
            foreach (var collider in GetComponentsInChildren<Collider>(true))
                collider.enabled = false;

            var triggerCollider = GetComponent<SphereCollider>();
            if (triggerCollider == null)
                triggerCollider = gameObject.AddComponent<SphereCollider>();

            triggerCollider.center = Vector3.zero;
            triggerCollider.radius = 1.2f;
            triggerCollider.isTrigger = true;
            triggerCollider.enabled = true;
        }
    }
}
