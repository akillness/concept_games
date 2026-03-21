using MossHarbor.Art;
using UnityEngine;

namespace MossHarbor.Gameplay.Player
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private float sprintMultiplier = 1.45f;
        [SerializeField] private float rotationSharpness = 12f;
        [SerializeField] private float animationDampTime = 0.08f;
        [SerializeField] private Transform modelRoot;

        private Vector3 _moveDirection;
        private Animator _animator;
        private float _currentAnimationSpeed;
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int SprintHash = Animator.StringToHash("Sprint");
        private static readonly int GroundedHash = Animator.StringToHash("Grounded");
        private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");

        private void Start()
        {
            if (modelRoot == null)
            {
                modelRoot = RuntimeArtDirector.AttachPlayerVisual(transform);
            }

            if (modelRoot != null)
            {
                DisableRootRenderers();
                _animator = modelRoot.GetComponentInChildren<Animator>(true);
            }
            else
            {
                _animator = GetComponentInChildren<Animator>(true);
            }
        }

        private void Update()
        {
            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            _moveDirection = input.normalized;
            var isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var effectiveMoveSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
            var targetAnimationSpeed = _moveDirection.sqrMagnitude > 0f ? (isSprinting ? 1f : 0.55f) : 0f;

            if (_moveDirection.sqrMagnitude > 0f)
            {
                transform.position += _moveDirection * (effectiveMoveSpeed * Time.deltaTime);
                if (modelRoot != null)
                {
                    var targetRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
                    modelRoot.rotation = Quaternion.Slerp(modelRoot.rotation, targetRotation, rotationSharpness * Time.deltaTime);
                }
            }

            if (_animator == null)
            {
                return;
            }

            _currentAnimationSpeed = Mathf.Lerp(_currentAnimationSpeed, targetAnimationSpeed, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.001f, animationDampTime)));
            _animator.SetFloat(SpeedHash, _currentAnimationSpeed);
            _animator.SetBool(SprintHash, isSprinting && _moveDirection.sqrMagnitude > 0f);
            _animator.SetBool(GroundedHash, true);
            _animator.SetFloat(VerticalSpeedHash, 0f);
        }

        private void DisableRootRenderers()
        {
            foreach (var renderer in GetComponents<Renderer>())
            {
                renderer.enabled = false;
            }
        }
    }
}
