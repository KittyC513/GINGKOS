using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Orientation References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    private Transform player1Cam;
    private Vector3 faceDir;

    [SerializeField]
    private float rotationSpeed;
    private Vector2 movement;
    private Vector3 move;


    [Header("Movement References")]
    [SerializeField]
    private InputActionReference movementControl;
    [SerializeField]
    private InputActionReference jumpControl;
    [SerializeField]
    private InputActionReference runControl;
    [SerializeField]
    private InputActionReference aimControl;

    [Header("Movement Variables")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float groundDrag;
    [SerializeField]
    private float playerHeight;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpCd;
    [SerializeField]
    private float airMyltiplier;
    [SerializeField]
    private MovementState state;





    [Header("Ground Check")]
    public LayerMask Ground;
    [SerializeField]
    private bool isGrounded;

    [Header("Jump")]
    bool readyToJump = true;

    [Header("Camera Control")]
    public CameraStyle currentStyle;
    public Transform combatLookAt;

    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject topDownCam;
    public GameObject aimCursor;

    public bool isFreeze;

    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown
    }

    public enum MovementState
    {
        freeze,
        walking,
        running,
        grappling,
        air
    }



    // Start is called before the first frame update
    void Start()
    {
        //turn off the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        player1Cam = Camera.main.transform;
        faceDir = Vector3.zero;
    }

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        runControl.action.Enable();
        aimControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        runControl.action.Disable();
        aimControl.action.Disable();
    }



    // Update is called once per frame
    void Update()
    {
        SpeedControl();
        MoveInput();
        CheckGound();
        
    }

    private void FixedUpdate()
    {
        if (isFreeze)
        {
            rb.velocity = Vector3.zero;

        }
        else
        {
            MovePlayer();
        }

        Rotate();
    }


    #region Player Input
    void MoveInput()
    {

        movement = movementControl.action.ReadValue<Vector2>();
        move = new Vector3(movement.x, 0, movement.y);


        if (jumpControl.action.triggered && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCd);
        }

        if (aimControl.action.IsPressed())
        {
            SwitchCameraStyle(CameraStyle.Combat);
            Debug.Log("combat camera");
        } else
        {
            SwitchCameraStyle(CameraStyle.Basic);
            Debug.Log("basic camera");
        }
    }

    #endregion

    #region Player Movement and Camera & Player Orientation
    void MovePlayer()
    {
        //rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;
       
        move.y = 0;


        if (currentStyle == CameraStyle.Basic)
        {
            //move = player1Cam.forward * move.z + player1Cam.right * move.x;
            move = player1Cam.forward * move.z + player1Cam.right * move.x;

            if (move != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, move.normalized, Time.deltaTime * rotationSpeed);
            }
        }else if(currentStyle == CameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            //player1Cam.forward = dirToCombatLookAt.normalized;
        
            playerObj.forward = dirToCombatLookAt.normalized;
            Debug.Log("dirToCombatLookAt" + dirToCombatLookAt);
            Debug.Log("playerObj" + playerObj.forward);

            //playerObj.forward = Vector3.Slerp(playerObj.forward, move, Time.deltaTime * rotationSpeed);

        }
        //rotate player object by player movement input value


        if (isGrounded)
        {
            rb.AddForce(move.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(move.normalized * moveSpeed * 10f *airMyltiplier, ForceMode.Force);
        }

    }

    /*
    void StateHandler()
    {
        if (isFreeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        else
        {
            moveSpeed = 7;
        }
    }
    */

    void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topDownCam.SetActive(false);
        aimCursor.SetActive(false);

        if(newStyle == CameraStyle.Basic)
        {
            thirdPersonCam.SetActive(true);
        }

        if (newStyle == CameraStyle.Combat)
        {
            combatCam.SetActive(true);
            aimCursor.SetActive(true);
        }

        if (newStyle == CameraStyle.Topdown)
        {
            topDownCam.SetActive(true);
        }

        currentStyle = newStyle;
    }

    #endregion

    #region Player facing direction
    void Rotate()
    {
        if (movement != Vector2.zero)
        {
            //float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + player1Cam.eulerAngles.y;
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + orientation.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }
    #endregion

    #region Ground Check
    void CheckGound()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);

        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        Debug.Log("is grounded = " + isGrounded);
    }

    #endregion

    #region Speed Control
    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //set up a max speed
        if(flatVel. magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

    }
    #endregion

    #region Player Jump
    void Jump()
    {
        //set y velocity to zero
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    #endregion


    #region Grappling Function


    #endregion

}
