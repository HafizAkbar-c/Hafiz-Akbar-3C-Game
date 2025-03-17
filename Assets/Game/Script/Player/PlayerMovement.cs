using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _walkSpeed;
    [SerializeField]
    private float _crouchSpeed;
    [SerializeField]
    private float _sprintSpeed;
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private float _walkSprintTransition;
    [SerializeField]
    private InputManager _playerInput;
    [SerializeField]
    private Transform _groundCheck;
    [SerializeField]
    private float _groundRadius;
    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private Vector3 _upperStepOffset;
    [SerializeField]
    private float _stepCheckerDistance;
    [SerializeField]
    private float stepForce;
    [SerializeField]
    private Transform _climbCheck;
    [SerializeField]
    private float _climbDistance;
    [SerializeField]
    private LayerMask _climbableLayer;
    [SerializeField]
    private Vector3 _climbOffset;
    [SerializeField]
    private float _climbSpeed;
    [SerializeField]
    private Transform _cameraTransform;
    [SerializeField]
    private CameraManager _cameraManager;
    [SerializeField]
    private float _glideSpeed;
    [SerializeField]
    private float _airDrag;
    [SerializeField]
    private Vector3 _glideRotationSpeed;
    [SerializeField]
    private float _minGlideRotationX;
    [SerializeField]
    private float _maxGlideRotationX;
    [SerializeField]
    private float _resetComboInterval;
    [SerializeField]
    private Transform _hitDetector;
    [SerializeField]
    private float _hitDetectorRadius;
    [SerializeField]
    private LayerMask _hitLayer;
    [SerializeField]
    private PlayerAudioManager _audioManager;


    private PlayerStance _playerStance;
    private Rigidbody _rigidbody;
    private float _speed;
    private bool isGrounded;
    private Animator _animator;
    private CapsuleCollider _collider;
    private bool _isPunching;
    private int _combo;
    private Coroutine _resetCombo;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _speed = _walkSpeed;
        _playerStance = PlayerStance.Standing;
        _collider = GetComponent<CapsuleCollider>();
        HideAndLockcursor();
    }

    private void Start()
    {
        _playerInput.OnMoveInput += Move;
        _playerInput.OnSprintInput += Sprint;
        _playerInput.OnJumpInput += Jump;
        _playerInput.OnClimbInput += StartClimb;
        _playerInput.OnCancelInput += CancelClimb;
        _playerInput.OncrouchInput += Crouch;
        _playerInput.OnGlideInput += StartGlide;
        _playerInput.OnCancelGlide += CancelGlide;
        _playerInput.OnPunchInput += Punch;
        _cameraManager.OnchangePerspective += ChangePerspective;
    }

    private void OnDestroy()
    {
        _playerInput.OnMoveInput -= Move;
        _playerInput.OnSprintInput -= Sprint;
        _playerInput.OnJumpInput -= Jump;
        _playerInput.OnClimbInput -= StartClimb;
        _playerInput.OnCancelInput -= CancelClimb;
        _playerInput.OncrouchInput -= Crouch;
        _playerInput.OnGlideInput -= StartGlide;
        _playerInput.OnCancelGlide -= CancelGlide;
        _playerInput.OnPunchInput -= Punch;
        _cameraManager.OnchangePerspective -= ChangePerspective;
    }

    private void Update()
    {
        CheckIsGrounded();
        CheckStep();
        Glide();
    }

    private void HideAndLockcursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Move(Vector2 axisDirection)
    {
        Vector3 movementDirection = Vector3.zero;
        bool isClimbing = _playerStance == PlayerStance.Climbing;
        bool isStanding = _playerStance == PlayerStance.Standing;
        bool isCrouching = _playerStance == PlayerStance.Crouch;
        bool isGliding = _playerStance == PlayerStance.Glide;

        if(isStanding || isCrouching && !_isPunching)
        {
            switch(_cameraManager._cameraState)
            {
                case CameraState.ThirdPerson:
                    if(axisDirection.magnitude >= 0.1)
                    {
                        float rotationAngle = Mathf.Atan2(axisDirection.x, axisDirection.y) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                        transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
                        movementDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
                        _rigidbody.AddForce(movementDirection * _speed * Time.deltaTime);
                    }
                break;
                case CameraState.FirstPerson:
                    transform.rotation = Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f);
                    Vector3 verticalMovement = transform.forward * axisDirection.y;
                    Vector3 horizontalMovement = transform.right * axisDirection.x;
                    movementDirection = verticalMovement + horizontalMovement;
                    _rigidbody.AddForce(movementDirection * _speed * Time.deltaTime);   
                break;
                default:
                break;
            }
            Vector3 velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            _animator.SetFloat("Velocity", velocity.magnitude * axisDirection.magnitude);
            _animator.SetFloat("VelocityX", velocity.magnitude * axisDirection.x);
            _animator.SetFloat("VelocityZ", velocity.magnitude * axisDirection.y);
        }
        else if(isClimbing)
        {
            Vector3 horizontalMovement = transform.right * axisDirection.x;
            Vector3 verticalMovement = transform.up * axisDirection.y;
            movementDirection = horizontalMovement + verticalMovement;
            _rigidbody.AddForce(movementDirection * _speed * Time.deltaTime);
            _rigidbody.AddForce(movementDirection * Time.deltaTime * _climbSpeed);
            Vector3 velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, 0);
            _animator.SetFloat("ClimbVelocityX", velocity.magnitude * axisDirection.x);
            _animator.SetFloat("ClimbVelocityY", velocity.magnitude * axisDirection.y);

        }
        else if(isGliding)
        {
            Vector3 rotationDegree = transform.rotation.eulerAngles;
            rotationDegree.x = Mathf.Clamp(rotationDegree.x, _minGlideRotationX, _maxGlideRotationX);
            rotationDegree.x += axisDirection.y * _glideRotationSpeed.x * Time.deltaTime;
            rotationDegree.y += axisDirection.x * _glideRotationSpeed.y * Time.deltaTime;
            rotationDegree.z += axisDirection.x * _glideRotationSpeed.z * Time.deltaTime;
            transform.rotation = Quaternion.Euler(rotationDegree);  
            
        }
    }

    private void Sprint(bool isSprinting)
    {
        if (isSprinting)
        {
            if(_speed < _sprintSpeed)
            {
                _speed = _speed + _walkSprintTransition * Time.deltaTime;
            }
        }
        else
        {
            if(_speed > _walkSpeed)
            {
                _speed = _speed - _walkSprintTransition * Time.deltaTime;
            }
        }
    }

    private void Jump()
    {
        if(isGrounded)
        {
            Vector3 jumpDirection = Vector3.up;
            _rigidbody.AddForce(jumpDirection * _jumpForce);
            _animator.SetTrigger("Jump");
        }
    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(_groundCheck.position, _groundRadius, _groundLayer);
        _animator.SetBool("IsGrounded", isGrounded);

        if(isGrounded)
        {
            CancelGlide();
        }

    }

    private void CheckStep()
    {
        bool isHitLower = Physics.Raycast(_groundCheck.position, transform.forward, _stepCheckerDistance);
        bool isHitUpper = Physics.Raycast(_groundCheck.position + _upperStepOffset, transform.forward, _stepCheckerDistance);

        if (isHitLower && !isHitUpper)
        {
            _rigidbody.AddForce(0, stepForce * Time.deltaTime, 0);
        }
    }

    private void StartClimb()
    {
        bool isClimbable = Physics.Raycast(_climbCheck.position, transform.forward, out RaycastHit hit, _climbDistance, _climbableLayer);

        bool isNotClimbing = _playerStance != PlayerStance.Climbing;
        
        if(isClimbable && isNotClimbing && isGrounded)
        {
            _collider.center = new Vector3(0, 1.3f, 0);
            Vector3 offset = (transform.forward * _climbOffset.z) + (Vector3.up * _climbOffset.y);
            transform.position = hit.point - offset;
            _playerStance = PlayerStance.Climbing;
            _rigidbody.useGravity = false;
            _speed = _climbSpeed;
            _cameraManager.SetFPSClampedCamera(true, transform.rotation.eulerAngles);
            _cameraManager.SetTPSFieldOfView(70);
            _animator.SetBool("IsClimbing", true);
        }
    
    }

    private void CancelClimb()
    {
        bool isClimbing = _playerStance == PlayerStance.Climbing;
        if(isClimbing)
        {
            _collider.center = new Vector3(0, 0.9f, 0);
            _playerStance = PlayerStance.Standing;
            _rigidbody.useGravity = true;
            transform.position -= transform.forward;
            _speed = _walkSpeed;
            _cameraManager.SetFPSClampedCamera(false, transform.rotation.eulerAngles);
            _cameraManager.SetTPSFieldOfView(40);
            _animator.SetBool("IsClimbing", false);
        }
    }

    private void ChangePerspective()
    {
        _animator.SetTrigger("ChangePerspective");
    }

    private void Crouch()
    {
        if(_playerStance == PlayerStance.Standing)
        {
            _playerStance = PlayerStance.Crouch;
            _animator.SetBool("IsCrouch", true);
            _speed = _crouchSpeed;
            _collider.height = 1.3f;
            _collider.center = new Vector3(0, 0.66f, 0);
        }
        else if(_playerStance == PlayerStance.Crouch)
        {
            _playerStance = PlayerStance.Standing;
            _animator.SetBool("IsCrouch", false);
            _speed = _walkSpeed;
            _collider.height = 1.8f;
            _collider.center = new Vector3(0, 0.9f, 0);
        }
    }

    private void Glide()
    {
        if(_playerStance == PlayerStance.Glide)
        {
            Vector3 playerRotation = transform.rotation.eulerAngles;
            float lift = playerRotation.x;
            Vector3 upForce = transform.up * (lift + _airDrag);
            Vector3 forwardForce = transform.forward * _glideSpeed;
            Vector3 totalForce = upForce + forwardForce;
            _rigidbody.AddForce(totalForce * Time.deltaTime);
        }
    }

    private void StartGlide()
    {
        if(_playerStance != PlayerStance.Glide && !isGrounded)
        {
            _playerStance = PlayerStance.Glide;
            _animator.SetBool("IsGlide", true);
            _cameraManager.SetFPSClampedCamera(true, transform.rotation.eulerAngles);
            _audioManager.playGlideSFX();
        }
    }

    private void CancelGlide()
    {
        if(_playerStance == PlayerStance.Glide)
        {
            _playerStance = PlayerStance.Standing;
            _animator.SetBool("IsGlide", false);
            _cameraManager.SetFPSClampedCamera(false, transform.rotation.eulerAngles);
            _audioManager.stopGlideSFX();
        }
    }

    private void Punch()
    {
        if(!_isPunching && _playerStance == PlayerStance.Standing)
        {
            _isPunching = true;
            if(_combo < 3)
            {
                _combo += 1;
            }
            else
            {
                _combo = 1;
            }
            _animator.SetInteger("Combo", _combo);
            _animator.SetTrigger("Punch");
        }
    }

    private void FinishPunch()
    {
        _isPunching = false;
        if(_resetCombo != null)
        {
            StopCoroutine(_resetCombo);
        }
        _resetCombo = StartCoroutine(ResetCombo());
    }

    private IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(_resetComboInterval);
        _combo = 0;
        _animator.SetInteger("Combo", _combo);
    }

    private void Hit()
    {
        Collider[] hitObjects = Physics.OverlapSphere(_hitDetector.position, _hitDetectorRadius, _hitLayer);

        for(int i = 0; i < hitObjects.Length; i++)
        {
            if(hitObjects[i].gameObject != null)
            Destroy(hitObjects[i].gameObject);
        }
    }


}