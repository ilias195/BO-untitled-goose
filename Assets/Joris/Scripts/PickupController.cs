using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    public Transform holdPosition;
    private GameObject objectInHand = null;
    private Transform speler;
    public Rigidbody rb;
    private Collider heldCollider;
    public float holdDistance = 1.2f;
    public float holdHeight = 1.2f;





    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speler = transform;
    }

    void FixedUpdate()
    {
        rb.MoveRotation(speler.rotation);
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

    }
    void LateUpdate()
    {
        if (objectInHand != null)
        {
            objectInHand.transform.position = holdPosition.position;
            objectInHand.transform.rotation = holdPosition.rotation;
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
                Collider col = objectInHand.GetComponent<Collider>();

                if (rb)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                if (col)
                {
                    col.enabled = false;
                }

                break;
            }
        }
    }



    void Drop()
    {
        if (objectInHand != null)
        {
            Rigidbody rb = objectInHand.GetComponent<Rigidbody>();
            Collider col = objectInHand.GetComponent<Collider>();
             
            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            if (col)
            {
                col.enabled = true;
            }

            objectInHand = null;
        }
    }



}

