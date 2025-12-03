using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Follow Settings")]
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0, 5, -8);

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float zoomedInFOV = 30f;     // Closest zoom
    public float zoomedOutFOV = 70f;    // Farthest zoom
    public float scrollSensitivity = 10f; // How fast scroll affects zoom

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("Transparency Settings")]
    public LayerMask obstacleMask;
    public Material transparentMaterial;

    private Camera cam;
    private float targetFOV;
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private List<Renderer> currentObstacles = new List<Renderer>();

    void Start()
    {
        cam = GetComponent<Camera>();
        targetFOV = cam.fieldOfView;
    }

    void LateUpdate()
    {
        if (!player) return;

        FollowPlayer();
        RotateHorizontally();
        HandleZoomInput();
        ApplyZoom();
        HandleObstructions();
    }

    // ---------------- FOLLOW PLAYER ----------------
    void FollowPlayer()
    {
        Vector3 targetPos = player.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );
    }

    // ---------------- ROTATION ----------------
    void RotateHorizontally()
    {
        Vector3 direction = player.position - transform.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // ---------------- KEY + SCROLL ZOOM ----------------
    void HandleZoomInput()
    {
            // Key-based zoom (still works)
        /*if (Input.GetKey(KeyCode.S))
            targetFOV = zoomedInFOV;*/

        /*if (Input.GetKey(KeyCode.A))
            targetFOV = zoomedOutFOV;*/

        // Scroll wheel zoom

    float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < 0f)
            {
                // Scroll up → zoom OUT
                targetFOV = zoomedOutFOV;
            }

            else if (scroll > 0f)
        {
            // Scroll down → zoom IN
            targetFOV = zoomedInFOV;
        }
    }

        void ApplyZoom()
    {
        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            zoomSpeed * Time.deltaTime
        );
    }

    // ---------------- OBSTRUCTION HANDLING ----------------
    void HandleObstructions()
    {
        Vector3 dir = player.position - transform.position;
        float dist = dir.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir.normalized, dist, obstacleMask);

        List<Renderer> newBlocked = new List<Renderer>();

        foreach (RaycastHit hit in hits)
        {
            Renderer r = hit.collider.GetComponent<Renderer>();
            if (!r) continue;

            newBlocked.Add(r);

            if (!currentObstacles.Contains(r))
                SwapToTransparent(r);
        }

        foreach (Renderer old in currentObstacles)
        {
            if (!newBlocked.Contains(old))
                RestoreOriginal(old);
        }

        currentObstacles = newBlocked;
    }

    void SwapToTransparent(Renderer r)
    {
        if (!originalMaterials.ContainsKey(r))
            originalMaterials[r] = r.materials;

        Material[] mats = new Material[r.materials.Length];
        for (int i = 0; i < mats.Length; i++)
            mats[i] = transparentMaterial;

        r.materials = mats;
    }

    void RestoreOriginal(Renderer r)
    {
        if (originalMaterials.ContainsKey(r))
        {
            r.materials = originalMaterials[r];
            originalMaterials.Remove(r);
        }
    }
}

