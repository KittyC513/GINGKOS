using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField] private Transform debugHitPointTransform;
    public CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMoveFunction(6);
    }
    #region Player Movement
    void PlayerMoveFunction(float speed)
    {
        float turnSmoothTime = 0.1f;
        float turnSmoothVelocity = 5f;

        //access input value of horizontal and vertical
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //gain direction of movement
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //execute the move function
        if(direction.magnitude >= 0.1f)
        {
            //facing direction while doing movement
            float targetAngle = Mathf.Atan2(direction.x, direction.z) *Mathf.Rad2Deg;
            //smoothly turning direction
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
    #endregion

    #region Grappling Function
    void GrapplingHookStart()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(Physics.Raycast(this.transform.position, this.transform.forward, out RaycastHit raycastHit))
            {
                //Hit something
                debugHitPointTransform.position = raycastHit.point;

            }
               
        }
    }
    #endregion

}
