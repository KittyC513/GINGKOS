using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPickDrop : MonoBehaviour
{
    public GameObject item;
    public Transform itemParent;
    bool isPicked;



    // Start is called before the first frame update
    void Start()
    {
        item.GetComponent<Rigidbody>().isKinematic = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            Drop();
        }

        if (isPicked)
        {
            item.transform.position = itemParent.transform.position;
        }
    }
    void Pickup()
    {
        item.GetComponent<Rigidbody>().isKinematic = true;

        item.transform.position = itemParent.transform.position;
        item.transform.rotation = itemParent.transform.rotation;

        item.GetComponent<BoxCollider>().enabled = false;

        item.transform.SetParent(itemParent);

        isPicked = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Pickup();
            Debug.Log("Pick up");
        }
    }


    void Drop()
    {
        itemParent.DetachChildren();
        item.transform.eulerAngles = new Vector3(item.transform.position.x, item.transform.position.z, item.transform.position.y);
        item.GetComponent<Rigidbody>().isKinematic = false;
        item.GetComponent<BoxCollider>().enabled = true;
    }

}
