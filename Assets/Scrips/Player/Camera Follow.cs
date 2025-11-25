using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Follow Settings")]
    public float followSpeed = 5f;       // Speed of camera catching up
    public Vector3 offset = new Vector3(0, 10, -10);
    public float lagDistance = 2f;       // How much the camera lags behind player movement

    [Header("Transparency Settings")]
    public LayerMask obstacleMask;
    public Material transparentMaterial;  // assign your pre-made fade material here

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private List<Renderer> currentObstacles = new List<Renderer>();

    private Vector3 _cameraTargetPos;    // The lagging target point
    private Vector3 _velocityRef = Vector3.zero;

    void Start()
    {
        _cameraTargetPos = player.position;
    }

    void LateUpdate()
    {
        if (!player) return;

        // --------- LAGGING TARGET POSITION ---------
        // Move the target point towards the player's current position
        _cameraTargetPos = Vector3.Lerp(_cameraTargetPos, player.position, followSpeed * Time.deltaTime);

        // Apply offset
        Vector3 desiredPos = _cameraTargetPos + offset;

        // Smooth camera follows the lagging target
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _velocityRef, 1f / followSpeed);

        // Look at the lagging target (optional: can look at exact player position instead)
        transform.LookAt(_cameraTargetPos);

        // Handle objects blocking view
        HandleObstructions();
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
