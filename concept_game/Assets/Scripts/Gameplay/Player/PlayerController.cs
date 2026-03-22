using MossHarbor.Data;
using MossHarbor.Art;
using MossHarbor.Expedition;
using MossHarbor.Hub;
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
        [SerializeField] private float externalImpulseDecay = 14f;
        [SerializeField] private Transform modelRoot;
        [Header("Boundary Recovery")]
        [SerializeField] private bool enableBoundaryRecovery = true;
        [SerializeField] private float boundarySafeHeightOffset = 1f;
        [SerializeField] private float boundaryRecoveryCooldownSeconds = 1.25f;

        private CharacterController _controller;
        private Vector3 _currentVelocity;
        private Vector3 _externalVelocity;
        private float _verticalSpeed;
        private Animator _animator;
        private float _currentAnimationSpeed;
        private float _lastBoundaryRecoveryTime = float.NegativeInfinity;
        private HubManager _hubManager;
        private ExpeditionDirector _expeditionDirector;
        private BoundaryRecoveryProfile _boundaryRecoveryProfile = DistrictDef.CreateDefaultBoundaryRecoveryProfile();
        private string _boundaryRecoveryDistrictId = string.Empty;
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

            _hubManager = FindFirstObjectByType<HubManager>();
            _expeditionDirector = FindFirstObjectByType<ExpeditionDirector>();
            RefreshBoundaryRecoveryProfile(force: true);
            TryRecoverBoundary();
        }

        private void Update()
        {
            RefreshBoundaryRecoveryProfile();
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
            _externalVelocity = Vector3.MoveTowards(_externalVelocity, Vector3.zero, externalImpulseDecay * Time.deltaTime);

            // Gravity
            if (_controller.isGrounded && _verticalSpeed < 0f)
                _verticalSpeed = -2f;
            _verticalSpeed += gravity * Time.deltaTime;

            // Apply movement
            var motion = _currentVelocity + _externalVelocity + Vector3.up * _verticalSpeed;
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
            var boundaryProfile = _boundaryRecoveryProfile ?? DistrictDef.CreateDefaultBoundaryRecoveryProfile();
            if (!BoundaryRecoveryRules.ShouldRecover(
                    currentPosition,
                    boundaryProfile.boundaryCenter,
                    boundaryProfile.boundaryHalfExtents,
                    boundaryProfile.floorY,
                    boundaryRecoveryCooldownSeconds,
                    currentTime,
                    _lastBoundaryRecoveryTime))
            {
                return false;
            }

            var safePosition = BoundaryRecoveryRules.ResolveSafePosition(
                boundaryProfile.safePosition,
                boundaryProfile.boundaryCenter,
                boundaryProfile.boundaryHalfExtents,
                boundaryProfile.floorY,
                boundarySafeHeightOffset);

            _controller.enabled = false;
            transform.position = safePosition;
            _controller.enabled = true;
            _currentVelocity = Vector3.zero;
            _externalVelocity = Vector3.zero;
            _verticalSpeed = 0f;
            _currentAnimationSpeed = 0f;
            _lastBoundaryRecoveryTime = currentTime;
            return true;
        }

        public Vector3 FacingDirection
        {
            get
            {
                if (modelRoot != null)
                {
                    var modelForward = Vector3.ProjectOnPlane(modelRoot.forward, Vector3.up);
                    if (modelForward.sqrMagnitude > 0.001f)
                    {
                        return modelForward.normalized;
                    }
                }

                var horizontalVelocity = Vector3.ProjectOnPlane(_currentVelocity + _externalVelocity, Vector3.up);
                if (horizontalVelocity.sqrMagnitude > 0.001f)
                {
                    return horizontalVelocity.normalized;
                }

                return Vector3.forward;
            }
        }

        public void ApplyExternalImpulse(Vector3 worldVelocity)
        {
            if (worldVelocity.sqrMagnitude <= 0f)
            {
                return;
            }

            var horizontalImpulse = Vector3.ProjectOnPlane(worldVelocity, Vector3.up);
            if (horizontalImpulse.sqrMagnitude >= _externalVelocity.sqrMagnitude)
            {
                _externalVelocity = horizontalImpulse;
            }
            else
            {
                _externalVelocity += horizontalImpulse * 0.35f;
            }

            if (worldVelocity.y > 0f)
            {
                _verticalSpeed = Mathf.Max(_verticalSpeed, worldVelocity.y);
            }
            else if (worldVelocity.y < 0f)
            {
                _verticalSpeed += worldVelocity.y;
            }
        }

        public static BoundaryRecoveryProfile ResolveBoundaryRecoveryProfile(
            DistrictDef district,
            BoundaryRecoveryProfile fallback = null)
        {
            if (district != null)
            {
                return district.GetBoundaryRecoveryProfile();
            }

            return fallback ?? DistrictDef.CreateDefaultBoundaryRecoveryProfile();
        }

        private void RefreshBoundaryRecoveryProfile(bool force = false)
        {
            var district = ResolveRuntimeDistrict();
            var districtId = district != null ? district.districtId : string.Empty;
            if (!force && districtId == _boundaryRecoveryDistrictId)
            {
                return;
            }

            _boundaryRecoveryProfile = ResolveBoundaryRecoveryProfile(district, _boundaryRecoveryProfile);
            _boundaryRecoveryDistrictId = districtId;
        }

        private DistrictDef ResolveRuntimeDistrict()
        {
            if (_expeditionDirector != null && _expeditionDirector.RuntimeDistrict != null)
            {
                return _expeditionDirector.RuntimeDistrict;
            }

            if (_hubManager != null && _hubManager.RuntimeDistrict != null)
            {
                return _hubManager.RuntimeDistrict;
            }

            return null;
        }

        private void UpdateAnimatorState()
        {
            if (_animator == null)
            {
                return;
            }

            var isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var currentSpeed = new Vector3(_currentVelocity.x + _externalVelocity.x, 0f, _currentVelocity.z + _externalVelocity.z).magnitude;
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

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit == null)
            {
                return;
            }

            var boostPad = hit.collider.GetComponent<TraversalBoostPad>();
            if (boostPad == null)
            {
                boostPad = hit.collider.GetComponentInParent<TraversalBoostPad>();
            }

            boostPad?.TryBoostPlayer(this);
        }
    }
}
