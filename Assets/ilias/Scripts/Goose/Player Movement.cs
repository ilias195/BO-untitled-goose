using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _acceleration = 6f;
    [SerializeField] private float _friction = 0.5f;

    [SerializeField] private Transform _playerTransform;

    private Vector3 _velocity;

    private float _currentSpeed;

    private Rigidbody _rb;

    private Vector3 targetPoint;
    private bool hasTarget = false;

    private float lastClickTime = 0f;
    private readonly float doubleClickThreshold = 0.25f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _currentSpeed = 0f;
    }

    void Update()
    {
        HandleMouseInput();
        HandleSpeedModes();

        Debug.Log("Current Speed = " + _currentSpeed);

        if (_currentSpeed == 3f)
        {
            GooseAnimations.PlayWalkingAnimation();
        }
        else if (_currentSpeed == 6f)
        {
            GooseAnimations.PlayRunAnimation();
        }
        else if(_currentSpeed == 1f)
        {
            GooseAnimations.PlayCrouchingAnimation();
        }
        else if (_currentSpeed == 0f)
        {
            GooseAnimations.PlayIdleAnimation();
        }
    }

    void HandleMouseInput()
    {
        // Double-click detection
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime <= doubleClickThreshold)
            {
                _currentSpeed = 6f; // run on double click
            }

            lastClickTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - lastClickTime >= doubleClickThreshold)
            {
                _currentSpeed = 3f;
            }

            lastClickTime = Time.time;
        }

        if(lastClickTime >= doubleClickThreshold && Input.GetMouseButtonUp(0))
        {
            _currentSpeed = 0f;
        }
    }

    void HandleSpeedModes()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            _currentSpeed = 1f;
        }
        else if (Input.GetMouseButton(0))
        {
            if (_currentSpeed != 6f)
                _currentSpeed = 3f; // walk while holding
        }
    }

    void FixedUpdate()
    {
        Vector3 inputDirection = Vector3.zero;

        // If holding left click → move forward
        if (Input.GetMouseButton(0))
        {
            inputDirection = transform.forward;
        }

        // Acceleration / friction handling
        if (inputDirection != Vector3.zero)
        {
            _velocity = Vector3.MoveTowards(
                _velocity,
                inputDirection * _currentSpeed,
                _acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            _velocity = Vector3.MoveTowards(
                _velocity,
                Vector3.zero,
                _friction * Time.fixedDeltaTime
            );
        }

        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }
}

    