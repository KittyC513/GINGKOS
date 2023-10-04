using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform tailTip;
    public LayerMask whatIsGrappleable;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    public CharacterController controller;

    bool grappling;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovingFunction(6);
        if (Input.GetKeyDown(KeyCode.E)) StartGrapple();

        if(grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    #region Player Movement
    void PlayerMovingFunction(float speed)
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

    #region Grappling Function
    void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }


    }
    void ExecuteGrapple()
    {

    }

    void StopGrapple()
    {
        grappling = false;

        grapplingCdTimer = grapplingCd;
    }

    #endregion

}
