using MossHarbor.Gameplay.Player;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class ObjectiveBeacon : MonoBehaviour
    {
        [SerializeField] private Color lockedColor = new(0.5f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color readyColor = new(0.2f, 0.9f, 0.6f, 1f);

        private ExpeditionDirector _director;
        private MeshRenderer _renderer;

        private void Start()
        {
            _director = FindFirstObjectByType<ExpeditionDirector>();
            _renderer = GetComponent<MeshRenderer>();
            RefreshColor();
        }

        private void Update()
        {
            RefreshColor();
            transform.Rotate(Vector3.up, 45f * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<PlayerController>(out _))
            {
                return;
            }

            if (_director != null && _director.ObjectiveReady)
            {
                _director.ActivateObjectiveBeacon();
            }
        }

        private void RefreshColor()
        {
            if (_renderer == null || _director == null)
            {
                return;
            }

            _renderer.material.color = _director.ObjectiveReady ? readyColor : lockedColor;
        }

        public void SetTheme(Color districtColor)
        {
            lockedColor = Color.Lerp(districtColor, new Color(0.24f, 0.08f, 0.08f, 1f), 0.45f);
            readyColor = Color.Lerp(districtColor, Color.white, 0.35f);
            RefreshColor();
        }
    }
}
