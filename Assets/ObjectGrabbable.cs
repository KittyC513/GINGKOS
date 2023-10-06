using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    public Rigidbody rb;
    public BoxCollider bC;
    private Transform objectGrabpo;
    [SerializeField]
    private float dropForce, dropUpForce;
    public GameObject player;
    public Transform playerDir;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bC = GetComponent<BoxCollider>();
    }
    // Start is called before the first frame update

    //when a pickable item is picked, it transform to player's head
    public void Grab(Transform objectGrabpo)
    {
        this.objectGrabpo = objectGrabpo;
        //set the gravity to zero when it's picked
        rb.useGravity = false;
        rb.isKinematic = true;
        bC.enabled = false;

    }

    private void FixedUpdate()
    {
        if(objectGrabpo != null)
        {
            float lerpspeed = 10;
            //smooth moving
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabpo.position, Time.deltaTime * lerpspeed); ;
            rb.MovePosition(newPosition);
        }
    }


    public void Drop()
    {
        this.objectGrabpo = null;
        rb.useGravity = true;
        rb.isKinematic = false;
        bC.enabled = true;

        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        rb.AddForce(playerDir.forward * dropForce, ForceMode.Impulse);
        rb.AddForce(playerDir.up * dropUpForce, ForceMode.Impulse);

        float random = Random.Range(-1, 1);
        rb.AddTorque(new Vector3(random, random, random));
    }
}
