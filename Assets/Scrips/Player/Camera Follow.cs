using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Follow Settings")]
    public float followSpeed = 5f;
    public Vector3 baseOffset = new Vector3(0, 8, -12); // Base perspective
    public float movementRadius = 4f; // Max horizontal shift around player
    public float forwardMovementFactor = 0.5f; // How much camera shifts forward with player movement

    [Header("Zoom Settings")]
    public float minDistance = 5f;
    public float maxDistance = 15f;
    public float baseZoomDistance = 12f; // Default zoom when no POI influence
    public float scrollSensitivity = 5f;
    public float zoomSmooth = 5f;

    [Header("POI Settings")]
    public string poiTag = "POI";
    public float poiRadius = 12f;       // Influence radius
    public float poiMaxOffset = 3f;     // Max horizontal shift
    public float verticalScreenMargin = 0.25f; // Top/bottom margin

    [Header("Camera Angle")]
    public float cameraAngle = 30f; // Degrees downward from horizontal

    [Header("Transparency Settings")]
    public LayerMask obstacleMask;
    public Material transparentMaterial;

    private Camera cam;
    private float targetDistance;
    private float currentDistance;
    private float zoomOffsetFromPOI; // temporary zoom due to POI
    private Vector3 dynamicOffset;   // Horizontal offset (POI + movement)
    private Vector3 movementOffset;  // Forward shift with player movement

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private List<Renderer> currentObstacles = new List<Renderer>();

    private Vector3 previousPlayerPos;

    void Start()
    {
        cam = GetComponent<Camera>();
        currentDistance = targetDistance = baseZoomDistance;
        zoomOffsetFromPOI = 0f;
        dynamicOffset = Vector3.zero;
        movementOffset = Vector3.zero;

        previousPlayerPos = player.position;

        // Set the fixed camera angle from above
        float rad = Mathf.Deg2Rad * cameraAngle;
        transform.rotation = Quaternion.LookRotation(new Vector3(0, -Mathf.Sin(rad), Mathf.Cos(rad)));
    }

    void LateUpdate()
    {
        if (!player) return;

        HandleZoomInput();
        HandlePOIInfluence();
        HandlePlayerMovementOffset();
        ApplyZoom();
        FollowPlayer();
        HandleObstructions();

        previousPlayerPos = player.position;
    }

    void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            baseZoomDistance -= scroll * scrollSensitivity;
            baseZoomDistance = Mathf.Clamp(baseZoomDistance, minDistance, maxDistance);
        }
    }

    void ApplyZoom()
    {
        targetDistance = baseZoomDistance + zoomOffsetFromPOI;
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * zoomSmooth);

        // Smoothly return zoomOffsetFromPOI to zero when POI is not influencing
        zoomOffsetFromPOI = Mathf.Lerp(zoomOffsetFromPOI, 0f, Time.deltaTime * 2f);
    }

    void FollowPlayer()
    {
        Vector3 desiredPos = player.position
                             + baseOffset.normalized * currentDistance
                             + dynamicOffset
                             + movementOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSpeed);
    }

    void HandlePOIInfluence()
    {
        GameObject[] pois = GameObject.FindGameObjectsWithTag(poiTag);
        Vector3 totalOffset = Vector3.zero;
        float totalWeight = 0f;
        float zoomAdjustment = 0f;

        foreach (var poiObj in pois)
        {
            Vector3 toPOI = poiObj.transform.position - player.position;
            Vector3 flatDir = Vector3.ProjectOnPlane(toPOI, transform.up);

            float distance = flatDir.magnitude;
            if (distance > poiRadius) continue;

            float weight = 1f - (distance / poiRadius);

            // Horizontal offset in camera local space
            Vector3 localDir = transform.InverseTransformDirection(flatDir);
            localDir.y = 0f;
            Vector3 worldOffset = transform.TransformDirection(localDir.normalized) * poiMaxOffset * weight;

            totalOffset += worldOffset;
            totalWeight += weight;

            // Vertical check for zoom
            Vector3 screenPos = cam.WorldToViewportPoint(poiObj.transform.position);
            if (screenPos.y < verticalScreenMargin)
                zoomAdjustment = Mathf.Max(zoomAdjustment, verticalScreenMargin - screenPos.y);
            else if (screenPos.y > 1f - verticalScreenMargin)
                zoomAdjustment = Mathf.Max(zoomAdjustment, screenPos.y - (1f - verticalScreenMargin));
        }

        if (totalWeight > 0f)
        {
            totalOffset /= totalWeight;
            totalOffset = Vector3.ClampMagnitude(totalOffset, poiMaxOffset);
            dynamicOffset = Vector3.Lerp(dynamicOffset, totalOffset, Time.deltaTime * 5f);
        }
        else
        {
            dynamicOffset = Vector3.Lerp(dynamicOffset, Vector3.zero, Time.deltaTime * 5f);
        }

        if (zoomAdjustment > 0f)
        {
            zoomOffsetFromPOI = Mathf.Clamp(zoomOffsetFromPOI + zoomAdjustment * 10f, 0f, maxDistance - baseZoomDistance);
        }
    }

    void HandlePlayerMovementOffset()
    {
        Vector3 movementDir = player.position - previousPlayerPos;
        Vector3 flatDir = Vector3.ProjectOnPlane(movementDir, transform.up);

        if (flatDir.sqrMagnitude > 0.001f)
        {
            Vector3 localOffset = transform.InverseTransformDirection(flatDir) * forwardMovementFactor;
            localOffset.y = 0f;

            movementOffset = Vector3.Lerp(movementOffset, transform.TransformDirection(localOffset), Time.deltaTime * 3f);
        }
        else
        {
            movementOffset = Vector3.Lerp(movementOffset, Vector3.zero, Time.deltaTime * 3f);
        }
    }

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

