using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPickDrop : MonoBehaviour
{
    [SerializeField]
    private LayerMask pickableMask;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform itemContainer;

    private ObjectGrabbable objectGrabbable;

    public CharacterInventory cI;
 

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        //press "E" to pick the item when player facing the pickable items
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(objectGrabbable == null)
            {
                float pickDistance = 2f;
                if (Physics.Raycast(player.position, player.forward, out RaycastHit raycastHit, pickDistance, pickableMask))
                {
                    if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                    {
                        //transform the item
                        objectGrabbable.Grab(itemContainer);

                    }

                }
            }
            else
            {
                objectGrabbable.Drop();
                objectGrabbable = null;
            }
            
        }
    }

}
