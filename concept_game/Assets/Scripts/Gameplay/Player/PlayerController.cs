using UnityEngine;

namespace MossHarbor.Gameplay.Player
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private Transform modelRoot;

        private Vector3 _moveDirection;

        private void Update()
        {
            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            _moveDirection = input.normalized;

            if (_moveDirection.sqrMagnitude > 0f)
            {
                transform.position += _moveDirection * (moveSpeed * Time.deltaTime);
                if (modelRoot != null)
                {
                    modelRoot.forward = _moveDirection;
                }
            }
        }
    }
}
