using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingTail : MonoBehaviour
{

    [Header("Grappling variables")]
    public ThirdPersonMovement Tpm;
    [SerializeField]
    private InputActionReference grapplingControl;
    [SerializeField]
    private float maxDistance = 100f;
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private Transform tailTip;
    [SerializeField]
    private LayerMask grappleable;
    LineRenderer lr;

    [Header("Grappling")]
    [SerializeField]
    private float maxGrappleDistance;
    [SerializeField]
    private float grappleDelayTime;
    [SerializeField]
    private float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    [SerializeField]
    private float grapplingCd;
    [SerializeField]
    private float grapplingCdTimer;
    private bool isGrappling;


    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Tpm = GetComponent<ThirdPersonMovement>();
    }
    private void OnEnable()
    {
        grapplingControl.action.Enable();

    }

    private void OnDisable()
    {
        grapplingControl.action.Disable();

    }

    // Update is called once per frame
    void Update()
    {

        if (grapplingControl.action.triggered)
        {
            StartGrapple();
        }else if (!grapplingControl.action.triggered)
        {
            StopGrapple();
        }

        if(grapplingCd > 0)
        {
            grapplingCd -= Time.deltaTime;
        }


    }

    private void LateUpdate()
    {
        if (isGrappling)
        {
            lr.SetPosition(0, tailTip.position);
        }
    }

    void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;
        
        isGrappling = true;

        //Tpm.isFreeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, grappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);

        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);



    }

    void ExecuteGrapple()
    {
        //Tpm.isFreeze = false;
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYpos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYpos + overshootYAxis;

        if(grapplePointRelativeYpos < 0)
        {
            highestPointOnArc = overshootYAxis;
            Tpm.JumpToPosition(grapplePoint, highestPointOnArc);
        }

        Invoke(nameof(StopGrapple), 1f);
            
           
    }
    

    void StopGrapple()
    {
        //Tpm.isFreeze = false;

        isGrappling = false;
        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }


    //Kinematic equations
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));
        return velocityXZ + velocityY;
    }

}
