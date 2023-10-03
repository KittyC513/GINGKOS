using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header ("Movement Variables")]
    [SerializeField]
    private InputActionReference movementControl;
    [SerializeField]
    private InputActionReference jumpControl;
    [SerializeField]
    private InputActionReference runControl;
    [SerializeField]
    private InputActionReference aimControl;
    [SerializeField]
    private float playerWalkSpeed = 8.5f;
    [SerializeField]
    private float playerRunSpeed = 12;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 4;

    //how long does it take to get from 0 to maximum speed
    [SerializeField]
    private float timeToWalkSpeed = 0.15f;
    [SerializeField]
    private float timeToZero = 0.05f;
    [SerializeField]
    private float timeToRunSpeed = 0.2f;
    private Vector3 faceDir;

    //local speed value
    [SerializeField]
    private float currentSpeed;

    private bool running = false;

    [Space, Header("Jump Variables")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    LayerMask groundLayer;

    private bool isGrounded = false;

    [SerializeField]
    private float groundCheckRadius = 0.33f;
    [SerializeField]
    private float groundCheckDist = 0.75f;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float jumpForce = 7;
    [SerializeField]
    private float jumpDeaccel = 0.5f;

    private bool isJumping = false;


    [Space, Header("Other")]
    [SerializeField]
    private Animator playerAnim;
   
    [SerializeField]
    enum debugMode { off, on }
    [SerializeField]
    debugMode debugToggle = debugMode.off;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform player1Cam;
    Vector2 movement;

    [Space, Header("Player Status")]
    private bool isWalking;

    [Space, Header("Grappling Variables")]
    public Transform tail;

    [Space, Header("Aiming Variables")]
    public GameObject mainCamera;
    public GameObject aimCamera;
    public GameObject aimCursor;






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


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        player1Cam = Camera.main.transform;
        faceDir = Vector3.zero;

        mainCamera.SetActive(true);
        aimCamera.SetActive(false);
        aimCursor.SetActive(false);
    }

    void Update()
    {
        playerAnim.SetFloat("speed", currentSpeed);

        if (isGrounded && !isJumping)
        {
            Move();
            if(currentSpeed <= 0)
            {
                isWalking = false;
            }
        }
        if (aimControl.action.IsPressed())
        {
            mainCamera.SetActive(false);
            aimCamera.SetActive(true);
            aimCursor.SetActive(true);
        }
        else
        {
            mainCamera.SetActive(true);
            aimCamera.SetActive(false);
            aimCursor.SetActive(false);
        }

        Jump();
        CheckGrounded();
        ApplySpeed();
        DebugFunctions();
    }

    private void FixedUpdate()
    {
        Rotate();
    }

    #region Debug
    private void DebugFunctions()
    {
        if (debugToggle == debugMode.on)
        {
            //Debug.Log(faceDir);
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        //if debug mode is on display a sphere which represents the radius of the groundcheck spherecast
        if (debugToggle == debugMode.on)
        {
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }
    }
    #endregion

    #region Ground Check
    private void CheckGrounded()
    {
        //send a spherecast downwards and check for ground, if theres ground we are grounded
        RaycastHit hit;
        if (Physics.SphereCast(groundCheck.position, groundCheckRadius, Vector3.down, out hit, groundCheckDist, groundLayer))
        {
            isGrounded = true;
            isJumping = false;
            Debug.Log("isGrounded" + isGrounded);

        }
        else
        {
            isGrounded = false;
            Debug.Log("isGrounded" + isGrounded);
            playerVelocity.y += gravityValue * Time.deltaTime;

        }
    }

    #endregion

    #region Player Movement
    void Move()
    {
        //access new input system, add xbox input 
        movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
       
        move = player1Cam.forward * move.z + player1Cam.right * move.x;
        move.y = 0;

        //moveN is the normalized version of move
        //we want to keep move not normalized because it allows for different speeds when pushing the stick further
        //but we need the normalized version to caluclate our facing direction
        Vector3 moveN = move.normalized;
        moveN = moveN.normalized;
     
        if (moveN.x != 0 || moveN.z != 0)
        {
            faceDir = moveN;
        }
        
        //if we are reading the run button, start running
        if (runControl.action.ReadValue<float>() == 1)
        {
            running = true;
        }
        else
        {
            running = false;
        }

        //calculate the current move speed based on if running or not
        if (move.x != 0 && move.z != 0)
        {
            if (running)
            {
                currentSpeed += ((playerRunSpeed - 0) / timeToRunSpeed) * Time.deltaTime;

                //if we are running from idle give us a speed boost to at least half of walking speed to feel more snappy
                if (currentSpeed < playerWalkSpeed / 2)
                {
                    currentSpeed = playerWalkSpeed;
                }
            }
            else if (currentSpeed < playerWalkSpeed)
            {
                currentSpeed += ((playerWalkSpeed - 0) / timeToWalkSpeed) * Time.deltaTime;
            }
        }
        else if (currentSpeed > 0) 
        {
            //bring down speed back to 0 with a little delay to give some weight to the character
            currentSpeed += ((0 - playerWalkSpeed) / timeToZero) * Time.deltaTime; 
        }

        //clamp values to maximum speed values
        if (running)
        {
            currentSpeed = Mathf.Clamp(currentSpeed, 0, playerRunSpeed);
        }
        else
        {
            //if our current speed is more than our walk speed and we let go of running
            //slow down a little gradually back to walk for weight
            if (currentSpeed > playerWalkSpeed + 5)
            {
                currentSpeed += ((0 - playerWalkSpeed) / 0.6f) * Time.deltaTime;
                
            }
            else
            {
                currentSpeed = Mathf.Clamp(currentSpeed, 0, playerWalkSpeed);
            }
           
        }

        //set the player velocity vector which is added to the controller to make it move
        playerVelocity = new Vector3(faceDir.x * currentSpeed, jumpSpeed , faceDir.z * currentSpeed);
        
        

        if (move != Vector3.zero)
        {
            transform.forward = move;
        }
        isWalking = true;


    }

    private void ApplySpeed()
    {
        //apply our actual speed in one place to avoid trying to override from multiple parts of the script
        controller.Move(playerVelocity * Time.deltaTime);
    }


    #endregion

    #region Jump Function
    void Jump()
    {

        //input is pressed and player is grounded, the jump can be triggered
        if (jumpControl.action.triggered && isGrounded && !isWalking)
        {
            //apply the initial jump force
            jumpSpeed = jumpForce;
            isJumping = true;
        }

        if (isJumping)
        {

        }
        //apply gravity
        if(jumpSpeed > -20)
        {
            jumpSpeed += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            jumpSpeed = -20;
        }




    }

    #endregion

    #region Player facing direction
    void Rotate()
    {
        if(movement!= Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + player1Cam.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }
    #endregion
}
