using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _runningSpeed = 12f;
    [SerializeField] private float _walkingSpeed = 6f;
    [SerializeField] private float _crouchingSpeed = 3f;

    [SerializeField] private float _acceleration = 3f;
    [SerializeField] private float _friction = 3f;

    [SerializeField] private Transform _playerTransform;

    private Vector3 _velocity;
    private float _currentSpeed;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true; // prevent tipping over
        _currentSpeed = _walkingSpeed;
    }

    void FixedUpdate()
    {
        // Movement input
        Vector3 inputDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            inputDirection += _playerTransform.forward;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            inputDirection -= _playerTransform.forward;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            inputDirection -= _playerTransform.right;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            inputDirection += _playerTransform.right;

        // Determine current speed (CROUCH → WALK → RUN priority)
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            _currentSpeed = _crouchingSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)))
        {
            _currentSpeed = _runningSpeed;
        }
        else
        {
            _currentSpeed = _walkingSpeed;
        }

        // Apply movement / acceleration
        if (inputDirection != Vector3.zero)
        {
            _velocity = Vector3.MoveTowards(
                _velocity,
                inputDirection.normalized * _currentSpeed,
                _acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            // Apply friction when no movement input
            _velocity = Vector3.MoveTowards(
                _velocity,
                Vector3.zero,
                _friction * Time.fixedDeltaTime
            );
        }

        // Move player using Rigidbody
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }
}
