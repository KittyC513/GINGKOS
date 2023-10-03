using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingTail : MonoBehaviour
{

    [Header("Reference")]
    private PlayerController pc;
    public Transform cam;
    public Transform tail;
    public LayerMask grappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    [SerializeField]
    private InputActionReference shootControl;

    public bool grappling;

    private void OnEnable()
    {
        shootControl.action.Enable();
    }

    private void OnDisable()
    {
        shootControl.action.Disable();
    }

    private void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (shootControl.action.triggered)
        {
            StartGrapple();
        }

        if(grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, tail.position);
        }
    }
    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;
            
        grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappleable))
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

    }
    void StopGrapple()
    {
        //when grappling ends, the cooldown timer return to zero 
        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }
    private void EndGrapple()
    {

    }





}