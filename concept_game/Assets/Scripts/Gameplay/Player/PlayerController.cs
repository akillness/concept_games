using MossHarbor.Art;
using UnityEngine;

namespace MossHarbor.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintMultiplier = 1.6f;
        [SerializeField] private float rotationSharpness = 12f;
        [SerializeField] private float animationDampTime = 0.08f;
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private float deceleration = 22f;
        [SerializeField] private float gravity = -15f;
        [SerializeField] private Transform modelRoot;

        private CharacterController _controller;
        private Vector3 _currentVelocity;
        private float _verticalSpeed;
        private Animator _animator;
        private float _currentAnimationSpeed;
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int SprintHash = Animator.StringToHash("Sprint");
        private static readonly int GroundedHash = Animator.StringToHash("Grounded");
        private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            if (_controller == null)
                _controller = gameObject.AddComponent<CharacterController>();

            _controller.height = 2f;
            _controller.radius = 0.4f;
            _controller.center = new Vector3(0f, 1f, 0f);

            if (modelRoot == null)
                modelRoot = RuntimeArtDirector.AttachPlayerVisual(transform);

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
            var moveDirection = input.normalized;
            var isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var targetSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

            // Smooth acceleration/deceleration
            var targetVelocity = moveDirection * targetSpeed;
            var accelRate = moveDirection.sqrMagnitude > 0f ? acceleration : deceleration;
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVelocity, accelRate * Time.deltaTime);

            // Gravity
            if (_controller.isGrounded && _verticalSpeed < 0f)
                _verticalSpeed = -2f;
            _verticalSpeed += gravity * Time.deltaTime;

            // Apply movement
            var motion = _currentVelocity + Vector3.up * _verticalSpeed;
            _controller.Move(motion * Time.deltaTime);

            // Rotation
            if (_currentVelocity.sqrMagnitude > 0.01f && modelRoot != null)
            {
                var horizontalVelocity = new Vector3(_currentVelocity.x, 0f, _currentVelocity.z);
                if (horizontalVelocity.sqrMagnitude > 0.01f)
                {
                    var targetRotation = Quaternion.LookRotation(horizontalVelocity, Vector3.up);
                    modelRoot.rotation = Quaternion.Slerp(modelRoot.rotation, targetRotation, rotationSharpness * Time.deltaTime);
                }
            }

            // Animation
            if (_animator == null)
                return;

            var currentSpeed = new Vector3(_currentVelocity.x, 0f, _currentVelocity.z).magnitude;
            var targetAnimationSpeed = currentSpeed > 0.1f ? (isSprinting ? 1f : 0.55f) : 0f;
            _currentAnimationSpeed = Mathf.Lerp(_currentAnimationSpeed, targetAnimationSpeed, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.001f, animationDampTime)));
            _animator.SetFloat(SpeedHash, _currentAnimationSpeed);
            _animator.SetBool(SprintHash, isSprinting && currentSpeed > 0.1f);
            _animator.SetBool(GroundedHash, _controller.isGrounded);
            _animator.SetFloat(VerticalSpeedHash, _verticalSpeed);
        }

        private void DisableRootRenderers()
        {
            foreach (var renderer in GetComponents<Renderer>())
                renderer.enabled = false;
        }
    }
}
