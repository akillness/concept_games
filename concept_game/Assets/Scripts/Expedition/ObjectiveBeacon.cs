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
        [SerializeField] private float readyLightRange = 8f;
        [SerializeField] private float readyLightIntensity = 2.6f;

        private ExpeditionDirector _director;
        private Renderer _renderer;
        private Vector3 _baseScale;
        private Light _readyLight;
        private Transform _readyHalo;

        private void Start()
        {
            _director = FindFirstObjectByType<ExpeditionDirector>();
            _renderer = GetComponentInChildren<Renderer>(true);
            _baseScale = transform.localScale;
            EnsureTriggerCollider();
            EnsureReadyCueVisual();
            RefreshColor();
        }

        private void Update()
        {
            RefreshColor();

            if (_director != null && _director.ObjectiveReady)
            {
                transform.Rotate(Vector3.up, readyRotateSpeed * Time.deltaTime, Space.World);
                var pulse = Mathf.Lerp(pulseMin, pulseMax, (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI) + 1f) * 0.5f);
                transform.localScale = _baseScale * pulse;
                if (_readyHalo != null)
                {
                    _readyHalo.localScale = Vector3.one * Mathf.Lerp(0.95f, 1.2f, (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI) + 1f) * 0.5f);
                }
            }
            else
            {
                transform.localScale = _baseScale;
                if (_readyHalo != null)
                {
                    _readyHalo.localScale = Vector3.one;
                }
            }

            if (_readyLight != null)
            {
                _readyLight.enabled = _director != null && _director.ObjectiveReady;
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
            readyColor = Color.Lerp(districtColor, Color.white, 0.58f);
            RefreshColor();
            RefreshReadyCueColor();
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

        private void EnsureReadyCueVisual()
        {
            if (_readyHalo == null)
            {
                var halo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                halo.name = "ReadyHalo";
                halo.transform.SetParent(transform, false);
                halo.transform.localPosition = new Vector3(0f, 0.78f, 0f);
                halo.transform.localScale = new Vector3(0.95f, 0.03f, 0.95f);
                if (halo.TryGetComponent<Collider>(out var collider))
                {
                    collider.enabled = false;
                }

                _readyHalo = halo.transform;
            }

            if (_readyLight == null)
            {
                var readyLightRoot = new GameObject("ReadyLight");
                readyLightRoot.transform.SetParent(transform, false);
                readyLightRoot.transform.localPosition = new Vector3(0f, 1.35f, 0f);
                _readyLight = readyLightRoot.AddComponent<Light>();
                _readyLight.type = LightType.Point;
                _readyLight.range = readyLightRange;
                _readyLight.intensity = readyLightIntensity;
                _readyLight.shadows = LightShadows.None;
            }

            RefreshReadyCueColor();
            _readyLight.enabled = false;
        }

        private void RefreshReadyCueColor()
        {
            if (_readyHalo != null && _readyHalo.TryGetComponent<Renderer>(out var renderer))
            {
                var material = renderer.material;
                material.color = readyColor;
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", readyColor * 1.2f);
            }

            if (_readyLight != null)
            {
                _readyLight.color = readyColor;
            }
        }
    }
}
