using UnityEngine;

public class PickupController : MonoBehaviour
{
    [Header("Player hold point (drag your hand empty here)")]
    public Transform playerHoldPoint;

    [Header("Pickup settings")]
    public float pickupRange = 2f;

    [Header("Rotation offset when held (degrees)")]
    public Vector3 holdRotationOffset = Vector3.zero;

    [Header("Heavy item settings")]
    public float heavyItemSpeed = 1f;
    public float jointSpring = 500f;
    public float jointDamper = 50f;

    [Header("Hold Point Selection")]
    public int holdPointIndex = 0;

    public bool IsHoldingItem { get; private set; }   // <-- ADDED

    private GameObject objectInHand;
    private ConfigurableJoint joint;
    private Rigidbody playerRb;
    private PlayerMovement playerMovement;
    private bool holdingHeavyItem = false;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        if (!playerRb) Debug.LogError("Player Rigidbody missing!");

        playerMovement = GetComponent<PlayerMovement>();
        if (!playerMovement) Debug.LogError("PlayerMovement script missing!");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (objectInHand == null)
                TryPickup();
            else
                Drop();
        }

        if (joint && playerHoldPoint)
        {
            joint.connectedAnchor = playerRb.transform.InverseTransformPoint(playerHoldPoint.position);
        }
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Pickup")) continue;

            GameObject item = hit.gameObject;
            Rigidbody itemRb = item.GetComponent<Rigidbody>();
            if (!itemRb) continue;

            ItemHoldPoint itemHold = item.GetComponent<ItemHoldPoint>();
            if (itemHold == null || itemHold.holdPoint.Length == 0)
            {
                Debug.LogWarning("ItemHoldPoint missing or no holdPoints assigned!");
                continue;
            }

            if (holdPointIndex < 0 || holdPointIndex >= itemHold.holdPoint.Length)
            {
                Debug.LogWarning("holdPointIndex out of range! Using first hold point.");
                holdPointIndex = 0;
            }

            Transform itemHoldPoint = itemHold.holdPoint[holdPointIndex];

            Vector3 posOffset = playerHoldPoint.position - itemHoldPoint.position;
            item.transform.position += posOffset;

            Quaternion rotationOffset = Quaternion.Euler(holdRotationOffset);
            Quaternion rot = playerHoldPoint.rotation * rotationOffset * Quaternion.Inverse(itemHoldPoint.rotation);
            item.transform.rotation = rot * item.transform.rotation;

            joint = item.AddComponent<ConfigurableJoint>();
            joint.connectedBody = playerRb;

            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = item.transform.InverseTransformPoint(itemHoldPoint.position);
            joint.connectedAnchor = playerRb.transform.InverseTransformPoint(playerHoldPoint.position);

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
            SoftJointLimit linearLimit = new SoftJointLimit { limit = 0.05f };
            joint.linearLimit = linearLimit;

            JointDrive drive = new JointDrive
            {
                positionSpring = jointSpring,
                positionDamper = jointDamper,
                maximumForce = Mathf.Infinity
            };
            joint.xDrive = drive;
            joint.yDrive = drive;
            joint.zDrive = drive;

            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Free;

            objectInHand = item;
            IsHoldingItem = true; // <-- ADDED

            holdingHeavyItem = itemHold.isHeavy;
            if (holdingHeavyItem)
                playerMovement.StartHoldingHeavyItem(heavyItemSpeed, true);

            break;
        }
    }

    void Drop()
    {
        if (joint) Destroy(joint);

        if (holdingHeavyItem)
            playerMovement.StopHoldingHeavyItem();

        objectInHand = null;
        holdingHeavyItem = false;
        IsHoldingItem = false; // <-- ADDED
    }
}