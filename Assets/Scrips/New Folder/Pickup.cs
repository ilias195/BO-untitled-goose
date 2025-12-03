using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [Header("Pickup settings")]
    [SerializeField] private Transform holdArea;
    private GameObject heldobj;
    private Rigidbody heldobjRB;

    [Header("Physics Parameters")]
    [SerializeField] private float pickupRange = 5.0f;
    [SerializeField] private float pickupForce = 150.0f;

    private void Start()
    {
        if (holdArea == null)
        {
            Debug.LogWarning("MoveObject: 'holdArea' is not assigned. Assign an empty Transform in front of the camera/player.");
        }
    }

    private void Update()
    {
        // single click to pick / drop
        if (Input.GetMouseButtonDown(0))
        {
            if (heldobj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange))
                {
                    pickUpObject(hit.transform.gameObject);
                }
            }
            else
            {
                DropObject();
            }
        }

        // only move object while one is held
        if (heldobj != null)
        {
            moveObject();
        }
    }
    void moveObject()
    {
        if (heldobj == null || heldobjRB == null || holdArea == null)
            return;

        if (Vector3.Distance(heldobj.transform.position, holdArea.position) > 0.1f)
        {
            Vector3 moveDirection = (holdArea.position - heldobj.transform.position);
            heldobjRB.AddForce(moveDirection * pickupForce, ForceMode.Force);
        }
        else
        {
            // damp residual velocity so object stays stable in hold area
            heldobjRB.linearVelocity = Vector3.zero;
        }
    }

    void pickUpObject(GameObject pickObj)
    {
        if (pickObj == null)
            return;

        // try to find a Rigidbody on the hit object first, then on its parents
        var rb = pickObj.GetComponent<Rigidbody>() ?? pickObj.GetComponentInParent<Rigidbody>();
        if (rb == null)
        {
            Debug.Log($"MoveObject: target '{pickObj.name}' has no Rigidbody on itself or in parents.");
            return;
        }

        // optionally ignore kinematic objects (they won't respond to forces)
        if (rb.isKinematic)
        {
            Debug.Log($"MoveObject: target Rigidbody on '{rb.gameObject.name}' is kinematic; cannot pick it up with physics-based mover.");
            return;
        }

        heldobjRB = rb;
        heldobjRB.useGravity = false;
        heldobjRB.linearDamping = 10f; // correct 3D Rigidbody property
        heldobjRB.constraints = RigidbodyConstraints.FreezeRotation;
        heldobjRB.linearVelocity = Vector3.zero;
        heldobjRB.angularVelocity = Vector3.zero;

        // parent the actual object that has the visual (use the pickObj passed in)
        if (holdArea != null)
            pickObj.transform.SetParent(holdArea);
        else
            pickObj.transform.SetParent(null);

        heldobj = pickObj;
    }

    void DropObject()
    {
        if (heldobjRB == null || heldobj == null)
            return;

        heldobjRB.useGravity = true;
        heldobjRB.linearDamping = 1f;
        heldobjRB.constraints = RigidbodyConstraints.None;

        heldobj.transform.SetParent(null);

        heldobjRB = null;
        heldobj = null;
    }
}


