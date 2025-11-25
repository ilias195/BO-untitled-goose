using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _runningSpeed = 12f;
    [SerializeField] private float _walkingSpeed = 6f;
    
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
        // Reset desired movement direction (local axes)
        Vector3 inputDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) inputDirection += _playerTransform.forward;
        if (Input.GetKey(KeyCode.S)) inputDirection -= _playerTransform.forward;
        if (Input.GetKey(KeyCode.A)) inputDirection -= _playerTransform.right;
        if (Input.GetKey(KeyCode.D)) inputDirection += _playerTransform.right;

        // Determine current speed
        _currentSpeed = Input.GetKey(KeyCode.LeftShift) ? _runningSpeed : _walkingSpeed;

        if (inputDirection != Vector3.zero)
        {
            // Accelerate toward target velocity
            _velocity = Vector3.MoveTowards
                (
                _velocity,inputDirection.normalized * _currentSpeed,_acceleration * Time.fixedDeltaTime
                );
        }
        else
        {
            // Apply friction when no input
            _velocity = Vector3.MoveTowards
                (
                _velocity,Vector3.zero,_friction * Time.fixedDeltaTime
                );
        }

        // Move the player using Rigidbody for proper collisions
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }
}
