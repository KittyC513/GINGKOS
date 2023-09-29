using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingTail : MonoBehaviour
{

    [Header("Grappling variables")]
    [SerializeField]
    private InputActionReference grapplingControl;
    [SerializeField]
    private float maxDistance = 100f;

    private LineRenderer lR;
    private Vector3 grapplePoint;
    public LayerMask Grappleable;
    
    public Transform tailTip, camera, player;
    private SpringJoint joint;



    private void Awake()
    {
        lR = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {

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
        DrawRope();
        if (grapplingControl.action.triggered)
        {
            StartGrapple();
        }else if (!grapplingControl.action.triggered)
        {
            StopGrapple();
        }
    }

    void StartGrapple()
    {
        RaycastHit hit;

        if(Physics.Raycast(origin: camera.position, direction: camera.forward, out hit, maxDistance))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
            Debug.Log("isGrappling = true");
        }
    }
    
    void DrawRope()
    {
        lR.SetPosition(0, tailTip.position);
        lR.SetPosition(1, tailTip.position); 
    }

    void StopGrapple()
    {

    }

}
