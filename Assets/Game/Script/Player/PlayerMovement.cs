using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _walkSpeed;
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

    private PlayerStance _playerStance;


    private Rigidbody _rigidbody;

    private float _speed;
    private bool isGrounded;
    private void CheckStep()
    {
        bool isHitLower = Physics.Raycast(_groundCheck.position, transform.forward, _stepCheckerDistance);
        bool isHitUpper = Physics.Raycast(_groundCheck.position + _upperStepOffset, transform.forward, _stepCheckerDistance);

        if (isHitLower && !isHitUpper)
        {
            _rigidbody.AddForce(0, stepForce * Time.deltaTime, 0);
        }
    }


    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _speed = _walkSpeed;
        _playerStance = PlayerStance.Standing;
    }
    private void Start()
    {
        _playerInput.OnMoveInput += Move;
        _playerInput.OnSprintInput += Sprint;
        _playerInput.OnJumpInput += Jump;
        _playerInput.OnClimbInput += StartClimb;
        _playerInput.OnCancelInput += CancelClimb;
    }

    private void OnDestroy()
    {
        _playerInput.OnMoveInput -= Move;
        _playerInput.OnSprintInput -= Sprint;
        _playerInput.OnJumpInput -= Jump;
        _playerInput.OnClimbInput -= StartClimb;
        _playerInput.OnCancelInput -= CancelClimb;
    }

    private void Update()
    {
        CheckIsGrounded();
        CheckStep();
    }

    private void Move(Vector2 axisDirection)
    {
        Vector3 movementDirection = Vector3.zero;
        bool isClimbing = _playerStance == PlayerStance.Climbing;
        bool isStanding = _playerStance == PlayerStance.Standing;

        if(isStanding)
        {

            if(axisDirection.magnitude >= 0.1)
            {
               float rotationAngle = Mathf.Atan2(axisDirection.x, axisDirection.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
                movementDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
                _rigidbody.velocity = movementDirection * _speed * Time.deltaTime;
            }
        }
        else if(isClimbing)
        {
            Vector3 horizontalMovement = transform.right * axisDirection.x;
            Vector3 verticalMovement = transform.up * axisDirection.y;
            movementDirection = horizontalMovement + verticalMovement;
            _rigidbody.AddForce(movementDirection * _speed * Time.deltaTime);
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
            _rigidbody.AddForce(jumpDirection * _jumpForce * Time.deltaTime);
        }
    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(_groundCheck.position, _groundRadius, _groundLayer);
    }

    private void StartClimb()
    {
        bool isClimbable = Physics.Raycast(_climbCheck.position, transform.forward, out RaycastHit hit, _climbDistance, _climbableLayer);

        bool isNotClimbing = _playerStance != PlayerStance.Climbing;
        
        if(isClimbable && isNotClimbing && isGrounded)
        {
            Vector3 offset = (transform.forward * _climbOffset.z) + (Vector3.up * _climbOffset.y);
            transform.position = hit.point - offset;
            _playerStance = PlayerStance.Climbing;
            _rigidbody.useGravity = false;
            _speed = _climbSpeed;
        }
    
    }

    private void CancelClimb()
    {
        bool isClimbing = _playerStance == PlayerStance.Climbing;
        if(isClimbing)
        {
            _playerStance = PlayerStance.Standing;
            _rigidbody.useGravity = true;
            transform.position -= transform.forward;
            _speed = _walkSpeed;
        }
    }
}