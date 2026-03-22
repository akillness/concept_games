using UnityEngine;

namespace MossHarbor.Expedition
{
    public sealed class TraversalBoostPadTriggerRelay : MonoBehaviour
    {
        [SerializeField] private TraversalBoostPad boostPad;

        public void Configure(TraversalBoostPad pad)
        {
            boostPad = pad;
        }

        private void OnTriggerEnter(Collider other)
        {
            boostPad?.TryBoostCollider(other);
        }

        private void OnTriggerStay(Collider other)
        {
            boostPad?.TryBoostCollider(other);
        }
    }
}
