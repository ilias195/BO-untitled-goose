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

    private float lastClickTime = 0f;
    private readonly float doubleClickThreshold = 0.25f;

    // --- Pickup integration ---
    private bool holdingHeavyItem = false;
    private float heavyItemSpeed = 1f; // crouching speed for heavy items
    private bool reverseControls = false; // reverse forward/backward input for heavy items

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

        // Play crouch animation if holding a heavy item
        if (holdingHeavyItem)
        {
            GooseAnimations.PlayCrouchingAnimation();
        }
        else
        {
            if (_currentSpeed == 3f)
                GooseAnimations.PlayWalkingAnimation();
            else if (_currentSpeed == 4.1f)
                GooseAnimations.PlayRunAnimation();
            else if (_currentSpeed == 1f)
                GooseAnimations.PlayCrouchingAnimation();
            else
                GooseAnimations.PlayIdleAnimation();
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime <= doubleClickThreshold)
                _currentSpeed = 4.1f; // run on double click

            lastClickTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - lastClickTime >= doubleClickThreshold)
                _currentSpeed = 3f;

            lastClickTime = Time.time;
        }

        if (lastClickTime >= doubleClickThreshold && Input.GetMouseButtonUp(0))
            _currentSpeed = 0f;
    }

    void HandleSpeedModes()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            _currentSpeed = 1f; // crouch
        }
        else if (Input.GetMouseButton(0))
        {
            if (!holdingHeavyItem)
            {
                if (_currentSpeed != 4.1f)
                    _currentSpeed = 3f; // normal walk
            }
            else
            {
                // Cap speed to heavy item crouch speed
                _currentSpeed = heavyItemSpeed;
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 inputDirection = Vector3.zero;

        if (Input.GetMouseButton(0))
        {
            inputDirection = transform.forward;

            // Reverse controls if holding a heavy item
            if (reverseControls)
                inputDirection = -inputDirection;
        }

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

    // ---------------- Pickup Integration ----------------

    /// <summary>
    /// Called when a heavy item is picked up
    /// </summary>
    public void StartHoldingHeavyItem(float crouchSpeed, bool reverseMovement = true)
    {
        holdingHeavyItem = true;
        heavyItemSpeed = crouchSpeed;
        _currentSpeed = heavyItemSpeed;
        reverseControls = reverseMovement; // forward input moves backward
    }

    /// <summary>
    /// Called when a heavy item is dropped
    /// </summary>
    public void StopHoldingHeavyItem()
    {
        holdingHeavyItem = false;
        _currentSpeed = 0f;
        reverseControls = false;
    }
}