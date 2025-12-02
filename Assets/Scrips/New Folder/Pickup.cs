using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [Header("Pickup settings")]
    [SerializeField] Transform holdArea;
    private GameObject heldobj;
    private Rigidbody heldobjRB;

    [Header("Physics Parameters")]
    [SerializeField] private float pickupRange = 5.0f;
    [SerializeField] private float pickupForce = 150.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    private void Update()
    {
        if (Get.GetMouseButtonDown(0))
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
        if (heldobj == null)
        {
            moveObject();
        }
    }
    void moveObject()
    {
        if (Vector3.Distance(heldobj.transform.position, holdArea.position) > 0.1f)
        {

            Vector3 moveDirection = (holdArea.position - heldobj.transform.position);
            heldobjRB.AddForce(moveDirection * pickupForce);
        }
    }
    void pickUpObject(GameObject pickObj)
    {
        if (pickObj.GetComponent<Rigidbody>())
        {
            heldobjRB = pickObj.GetComponent<Rigidbody>();
            heldobjRB.useGravity = false;
            heldobjRB.linearDamping = 10;
            heldobjRB.constraints = RigidbodyConstraints.FreezeRotation;

            heldobjRB.transform.parent = holdArea;
            heldobj = pickObj;

        }
    }
    void DropObject()
    {

        heldobjRB.useGravity = true;
        heldobjRB.linearDamping = 1;
        heldobjRB.constraints = RigidbodyConstraints.None;

        heldobjRB.transform.parent = null;
        heldobj = null;


    }
}


