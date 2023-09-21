using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Movement : MonoBehaviour
{
    public CharacterController controller;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement(6);
    }

    #region Player Movement
    void PlayerMovement(float speed)
    {
        //access value from horizontal and vertical inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //get the direction axis.x and axis.z
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        //move function
        if (direction.magnitude >= 0.1f)
        {
            controller.Move(direction * speed * Time.deltaTime);
        }
    }
    #endregion

    #region CameraSizeAdjustment
    private void OnTriggerStay(Collider other)
    {
        //when player collides with clue, the camera width sets to 1
        if (other.CompareTag("Clue"))
        {
            cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            Debug.Log("Set camera to 1");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //when player exits any clue, the camera width sets to 0.5
        if (other.CompareTag("Clue"))
        {
            cam.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
            Debug.Log("Set camera to 0.5");
        }
    }
    #endregion
}
