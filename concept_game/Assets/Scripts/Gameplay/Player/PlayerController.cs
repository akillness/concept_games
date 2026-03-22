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
        [Header("Boundary Recovery")]
        [SerializeField] private bool enableBoundaryRecovery = true;
        [SerializeField] private Vector3 boundaryCenter = Vector3.zero;
        [SerializeField] private Vector2 boundaryHalfExtents = new Vector2(180f, 180f);
        [SerializeField] private float boundaryFloorY = -12f;
        [SerializeField] private Vector3 boundarySafePosition = new Vector3(0f, 1f, 0f);
        [SerializeField] private float boundarySafeHeightOffset = 1f;
        [SerializeField] private float boundaryRecoveryCooldownSeconds = 1.25f;

        private CharacterController _controller;
        private Vector3 _currentVelocity;
        private float _verticalSpeed;
        private Animator _animator;
        private float _currentAnimationSpeed;
        private float _lastBoundaryRecoveryTime = float.NegativeInfinity;
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

            TryRecoverBoundary();
        }

        private void Update()
        {
            if (TryRecoverBoundary())
            {
                UpdateAnimatorState();
                return;
            }

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

            if (TryRecoverBoundary())
            {
                UpdateAnimatorState();
                return;
            }

            UpdateAnimatorState();
        }

        private void DisableRootRenderers()
        {
            foreach (var renderer in GetComponents<Renderer>())
                renderer.enabled = false;
        }

        private bool TryRecoverBoundary()
        {
            if (!enableBoundaryRecovery || _controller == null)
            {
                return false;
            }

            var currentTime = Time.time;
            var currentPosition = transform.position;
            if (!BoundaryRecoveryRules.ShouldRecover(
                    currentPosition,
                    boundaryCenter,
                    boundaryHalfExtents,
                    boundaryFloorY,
                    boundaryRecoveryCooldownSeconds,
                    currentTime,
                    _lastBoundaryRecoveryTime))
            {
                return false;
            }

            var safePosition = BoundaryRecoveryRules.ResolveSafePosition(
                boundarySafePosition,
                boundaryCenter,
                boundaryHalfExtents,
                boundaryFloorY,
                boundarySafeHeightOffset);

            _controller.enabled = false;
            transform.position = safePosition;
            _controller.enabled = true;
            _currentVelocity = Vector3.zero;
            _verticalSpeed = 0f;
            _currentAnimationSpeed = 0f;
            _lastBoundaryRecoveryTime = currentTime;
            return true;
        }

        private void UpdateAnimatorState()
        {
            if (_animator == null)
            {
                return;
            }

            var isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var currentSpeed = new Vector3(_currentVelocity.x, 0f, _currentVelocity.z).magnitude;
            var targetAnimationSpeed = currentSpeed > 0.1f ? (isSprinting ? 1f : 0.55f) : 0f;
            _currentAnimationSpeed = Mathf.Lerp(_currentAnimationSpeed, targetAnimationSpeed, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.001f, animationDampTime)));
            _animator.SetFloat(SpeedHash, _currentAnimationSpeed);
            _animator.SetBool(SprintHash, isSprinting && currentSpeed > 0.1f);
            _animator.SetBool(GroundedHash, _controller != null && _controller.isGrounded);
            _animator.SetFloat(VerticalSpeedHash, _verticalSpeed);

            if (_currentVelocity.sqrMagnitude > 0.01f && modelRoot != null)
            {
                var horizontalVelocity = new Vector3(_currentVelocity.x, 0f, _currentVelocity.z);
                if (horizontalVelocity.sqrMagnitude > 0.01f)
                {
                    var targetRotation = Quaternion.LookRotation(horizontalVelocity, Vector3.up);
                    modelRoot.rotation = Quaternion.Slerp(modelRoot.rotation, targetRotation, rotationSharpness * Time.deltaTime);
                }
            }
        }
    }
}
