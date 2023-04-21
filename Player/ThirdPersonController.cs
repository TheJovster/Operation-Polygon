using Cinemachine;
using OperationPolygon.Combat;
using OperationPolygon.Core;
using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Crouch speed of the character in m/s")]
        public float CrouchSpeed = 1f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Tooltip("Free look mouse sensitivity value")]
        public float MouseSensitivityFraction = 1f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        [SerializeField]private bool _isCrouching = false;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        //player rotation
        private bool _rotateWithMovement = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private Inputs _input;
        private GameObject _mainCamera;

        //functionality added by me
        private Stamina _stamina;
        private Health _health;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private ThirdPersonShooterController _thirdPersonShooterController;
        private float _aimYHeightOriginal;
        [Header("CinemachineAimController")]
        [SerializeField] private CinemachineVirtualCamera _cmAimCamera;
        [SerializeField] private float _aimYHeightCrouching;
        [Header("Cinemachine FOV Variables")]
        [SerializeField] private CinemachineVirtualCamera _cmFollowCamera;
        private CinemachineImpulseSource _impulseSource;
        private Cinemachine3rdPersonFollow _aimCamFollowComponent;
        private float _followCamOriginalFOV;
        [SerializeField] private float _followCamSprintFOV;
        [SerializeField] private float _sprintEffectTime;

        [Header("Additional Variables")]
        [SerializeField] private float _minMoveSpeed = 2f;
        [SerializeField] private float _maxMoveSpeed = 5f;
        [SerializeField] private float _moveSpeedChangeRate = 10f;
        private float _lastMoveSpeedValue;
        private bool _hasCrouched = false; 

        //others
        private InputDevice _currentDevice;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            _lastMoveSpeedValue = MoveSpeed;
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            _thirdPersonShooterController = GetComponent<ThirdPersonShooterController>();
            _aimYHeightOriginal = _cmAimCamera.transform.position.y;
            _aimCamFollowComponent = _cmAimCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            _aimYHeightOriginal = _aimCamFollowComponent.ShoulderOffset.y;
        }

        private void Start()
        {
            
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _followCamOriginalFOV = _cmFollowCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;
            _impulseSource = GetComponent<CinemachineImpulseSource>();
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<Inputs>();
            _stamina = GetComponent<Stamina>();
            _health = GetComponent<Health>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
            foreach(InputDevice device in InputSystem.devices) 
            {
                //assignment in an if else statement
                //I'm just trying to make sure the code is watertight
                if(device is Mouse && device.enabled) 
                {
                    _currentDevice = device;
                    
                }
                else if(device is Gamepad && device.enabled) 
                {
                    _currentDevice = device;
                }
            }
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            if (_health.IsAlive()) 
            {
                Move();
                Crouch();
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * MouseSensitivityFraction;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * MouseSensitivityFraction;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed && has stamina
            float targetSpeed = _input.sprint && _stamina.HasStamina() && !_isCrouching && Grounded && !_thirdPersonShooterController.IsAiming()
                ? SprintSpeed : MoveSpeed;
            //ControlMoveSpeed is only available with mouse and keyboard for now
            ControlMoveSpeed();

            if (_input.sprint && _stamina.HasStamina() && !_isCrouching && Grounded && Mathf.Abs(_input.move.magnitude) > 0f && !_thirdPersonShooterController.IsAiming()) 
            {
                SprintEffect();
            }
            else 
            {
                CancelSprintEffect();
            }

            if (_isCrouching) 
            {
                targetSpeed = CrouchSpeed;
            }
            //need to refactor for crouching later

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            


            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                if (_rotateWithMovement) 
                {
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
                _stamina.SetIsMoving(true);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

           
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }

            if(_input.move == Vector2.zero) 
            {
                _stamina.SetIsMoving(false);
            }
        }

        private void ControlMoveSpeed() 
        {
            float mouseWheelDelta = Input.mouseScrollDelta.y;
            

            if (mouseWheelDelta < 0f && !_thirdPersonShooterController.IsAiming())
            {
                MoveSpeed += mouseWheelDelta * _moveSpeedChangeRate;
                MoveSpeed = Mathf.Clamp(MoveSpeed, _minMoveSpeed, _maxMoveSpeed);
                _lastMoveSpeedValue = MoveSpeed;
            }
            else if (mouseWheelDelta > 0f && !_thirdPersonShooterController.IsAiming())
            {
                MoveSpeed += mouseWheelDelta * _moveSpeedChangeRate;
                MoveSpeed = Mathf.Clamp(MoveSpeed, _minMoveSpeed, _maxMoveSpeed);
                _lastMoveSpeedValue = MoveSpeed;
            }
            
            if (_thirdPersonShooterController.IsAiming()) 
            {
                MoveSpeed = _minMoveSpeed;
            }
            else 
            {
                MoveSpeed = _lastMoveSpeedValue;
            }
        }

        private void SprintEffect()
        {
            float currentFOV = _cmFollowCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;
            _cmFollowCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView =
                Mathf.Lerp(currentFOV, _followCamSprintFOV, _sprintEffectTime * Time.deltaTime);
            _impulseSource.GenerateImpulse();
            
        }

        private void CancelSprintEffect() 
        {
            float currentFOV = _cmFollowCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;
            _cmFollowCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView =
                Mathf.Lerp(currentFOV, _followCamOriginalFOV, _sprintEffectTime * Time.deltaTime);
            
        }

        private void JumpAndGravity() //changed the name of the method
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump  - commented out code snippet to disable jumping
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && _health.IsAlive())
                {
                    if (_isCrouching) 
                    {
                        _input.jump = false;
                        return;
                    }
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    //Debug.Log("Jump Triggered");

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void Crouch() //written by me
        {
            if (_input.crouch) 
            {
                if (_isCrouching || !_isCrouching) 
                {
                    _input.crouch = false;
                    StartCoroutine(CrouchRoutine());
                }                
            }
        }

        private IEnumerator CrouchRoutine()  //solution was simpler than I thought - all I needed to do was aim for slighltly lower/higher values as checks
        {
            if (_aimCamFollowComponent.ShoulderOffset.y >= 0.225f && !_isCrouching)
            {
                _input.crouch = false;
                _isCrouching = !_isCrouching;
                float elapsedTime = 0f;
                while (elapsedTime < .6f)
                {
                    _hasCrouched = true;
                    elapsedTime += Time.deltaTime; 
                    _aimCamFollowComponent.ShoulderOffset.y = 
                        Mathf.Lerp(_aimCamFollowComponent.ShoulderOffset.y, _aimYHeightCrouching, elapsedTime); 
                    _animator.SetBool("Crouch", true); 
                    yield return null; 
                }
                
            }
            else if (_aimCamFollowComponent.ShoulderOffset.y <= -0.225f && _isCrouching)
            {
                _input.crouch = false;
                _isCrouching = !_isCrouching;
                float elapsedTime = 0f;
                while (elapsedTime < 0.6f)
                {
                    elapsedTime += Time.deltaTime; 
                    _aimCamFollowComponent.ShoulderOffset.y = 
                        Mathf.Lerp(_aimCamFollowComponent.ShoulderOffset.y, _aimYHeightOriginal, elapsedTime); 
                    _animator.SetBool("Crouch", false); 
                    yield return null; 
                }
                
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }


        //code written by me - Jovan Aleksic (TheJovster)
        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        //public getter functions

        public bool IsCrouching() 
        {
            return _isCrouching;
        }

        //public setter methods - maybe I'll need it later

        public void SetMouseSensitivityFraction(float fractionValue)
        {
            MouseSensitivityFraction = fractionValue;
        }

        public void SetRotationWithMovement(bool newRotation) 
        {
            _rotateWithMovement = newRotation;
        }

        public void SetMoveSpeed() 
        {
            MoveSpeed = 1f;
        }

        public void ResetMoveSpeed() 
        {
            MoveSpeed = 2f;
        }

        public void SetSprintSpeed() 
        {
            SprintSpeed = 2.65f;
        }

        public void ResetSprintSpeed() 
        {
            SprintSpeed = 5.335f;
        }



        //animation events


    }
}