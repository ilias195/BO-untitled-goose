using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float doubleClickTime = 0.25f;

    private float lastClickTime = 0f;
    private bool allowRotation = false;

    void Update()
    {
        HandleMouseClickLogic();

        if (!allowRotation)
            return;

        RotateTowardMouse();
    }

    void HandleMouseClickLogic()
    {
        // Detect new click
        if (Input.GetMouseButtonDown(0))
        {
            // Double-click detected
            if (Time.time - lastClickTime <= doubleClickTime)
            {
                allowRotation = true;   // Run + rotate
            }

            lastClickTime = Time.time;

            // Always rotate on hold
            allowRotation = true;
        }

        // Stop rotating when mouse is released
        if (Input.GetMouseButtonUp(0))
        {
            allowRotation = false;
        }
    }

    void RotateTowardMouse()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

        // Ground plane at player's feet
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            Vector3 dir = hitPoint - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
