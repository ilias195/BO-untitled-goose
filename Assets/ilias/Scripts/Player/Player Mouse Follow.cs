using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [SerializeField] private Camera _cam; // Assign in Inspector
    [SerializeField] private float rotationSpeed = 10f; // Higher = faster turning

    void Update()
    {
        // 1. Ray from camera to mouse
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

        // 2. Ground plane at player's height
        Plane ground = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        // 3. If ray hits the plane:
        if (ground.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            // Direction towards mouse (on flat axis)
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.0001f)
            {
                // Target rotation
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Smooth rotation (lag)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

}
