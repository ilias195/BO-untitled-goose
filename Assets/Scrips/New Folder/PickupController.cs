using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    public Transform holdPosition;
    private GameObject objectInHand = null;

    void Update()
    {

        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (objectInHand == null)
                TryPickup();
            else
                Drop();
        }
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Pickup"))
            {
                objectInHand = hit.gameObject;

                Rigidbody rb = objectInHand.GetComponent<Rigidbody>();
                if (rb) rb.isKinematic = true;   

                objectInHand.transform.SetParent(holdPosition);
                objectInHand.transform.position = holdPosition.position;
                objectInHand.transform.rotation = holdPosition.rotation;

                break;
            }
        }
    }


    void Drop()
    {
        if (objectInHand != null)
        {
            objectInHand.transform.SetParent(null);

            Rigidbody rb = objectInHand.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = false;   

            objectInHand = null;
        }
    }

}

