using MossHarbor.Data;
using MossHarbor.Gameplay.Player;
using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class SimplePickup : MonoBehaviour
    {
        [SerializeField] private ResourceType resourceType = ResourceType.BloomDust;
        [SerializeField] private int amount = 5;
        [SerializeField] private float rotateSpeed = 90f;

        private ExpeditionDirector _director;

        private void Start()
        {
            _director = FindFirstObjectByType<ExpeditionDirector>();
        }

        public void Configure(ResourceType nextResourceType, int nextAmount, float nextRotateSpeed)
        {
            resourceType = nextResourceType;
            amount = nextAmount;
            rotateSpeed = nextRotateSpeed;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<PlayerController>(out _))
            {
                return;
            }

            _director?.Collect(resourceType, amount);
            Destroy(gameObject);
        }
    }
}
