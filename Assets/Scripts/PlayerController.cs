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
    [Header("Movement Variables")]
    [SerializeField]
    private PlayerControls playerControls;
    private InputAction movementControl;
    private InputAction jumpControl;
    private InputAction runControl;

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
    private float jumpForce = 15;
    [SerializeField]
    private float jumpDeaccel = 12f;
    [SerializeField]
    private float minJumpForce = 6;
    [SerializeField]
    private float maxFall = -35;

    private bool isJumping = false;


    [Space, Header("Other")]
    [SerializeField]
    private Animator playerAnim;
    private bool isOnCircle = false;
    private GameObject activeCircle;

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
    public enum playerState { idle, walking, running, jumping, airborne, summoning }
    public playerState pState;

    [Space, Header("Grappling Variables")]
    public Transform tail;



    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        //setup the controls in a way that doesn't require a new serialization for each input we need
        //we now just grab the whole input asset and get the individual components 
        movementControl = playerControls.Player.Movement;
        movementControl.Enable();
        jumpControl = playerControls.Player.Jump;
        jumpControl.Enable();
        runControl = playerControls.Player.Run;
        runControl.Enable();

        
        
    }

    private void OnDisable()
    {
        movementControl.Disable();
        jumpControl.Disable();
        runControl.Disable();
    }

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        player1Cam = Camera.main.transform;
        faceDir = Vector3.zero;
        Application.targetFrameRate = 60;
    }

    void Update()
    {

        if (pState != playerState.summoning)
        {
            Move();
            Jump();
        }
      
        ReadInput();
        UpdateAnimation();
        CheckGrounded();
        ApplySpeed();
        DebugFunctions();
        UpdateStates();
    }

    private void FixedUpdate()
    {
        Rotate();
    }

    private void ReadInput()
    {
        //access new input system, add xbox input 
        movement = movementControl.ReadValue<Vector2>();
    }

    private void UpdateAnimation()
    {
        //set our speed value in our animator
        playerAnim.SetFloat("speed", currentSpeed);
        //set our summoning state in our animator
        //playerAnim.SetBool("summoning", pState == playerState.summoning);
    }

    #region State Machine
    private void UpdateStates()
    {
        //A player state machine
        //changes state based on what the player is doing
        //this can be used to do certain things only when the player is doing certain actions
        //keeps everything in one place rather than a ton of bools
        if (isOnCircle)
        {
            pState = playerState.summoning;
        }
        else if (currentSpeed <= 0 && isGrounded)
        {
            pState = playerState.idle;
        }
        else if (!isGrounded && isJumping)
        {
            pState = playerState.jumping;
        }
        else if (!isGrounded)
        {
            //if we were just grounded and now we're airborne and we aren't jumping set our speed to 0
            //so we dont fall like a brick
            if (pState != playerState.airborne && pState != playerState.jumping)
            {
                jumpSpeed = 0;
            }
            pState = playerState.airborne;
        }
        else if (currentSpeed > playerWalkSpeed)
        {
            pState = playerState.running;
        }
        else if (currentSpeed <= playerWalkSpeed)
        {
            pState = playerState.walking;
        }

        //a switch function to make things run based on our current state
        switch (pState)
        {
            case playerState.idle:
                break;
            case playerState.walking:
                break;
            case playerState.running:
                break;
            case playerState.jumping:
                break;
            case playerState.airborne:
                break;
            case playerState.summoning:
                //set our direction towards the summoning circle and slowly move the player to it
                if (activeCircle != null)
                {
                    faceDir = activeCircle.transform.position - transform.position;
                    faceDir = faceDir.normalized;
                    currentSpeed = playerWalkSpeed;

                    playerVelocity = new Vector3(faceDir.x * currentSpeed, 0, faceDir.z * currentSpeed);
                }
                break;
        }
    }

    #endregion

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
            
            //Debug.Log("isGrounded" + isGrounded);

        }
        else
        {
            isGrounded = false;
            //Debug.Log("isGrounded" + isGrounded);
            playerVelocity.y += gravityValue * Time.deltaTime;

        }
    }

    #endregion

    #region Player Movement
    void Move()
    {
       
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
        if (runControl.ReadValue<float>() == 1)
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
            //transform.forward = move;
        }
        

        if (isGrounded && !isJumping)
        {

            if (currentSpeed <= 0)
            {
                isWalking = false;
            }
            else
            {
                isWalking = true;
            }
        }

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
        if (jumpControl.ReadValue<float>() == 1 && isGrounded)
        {
            //apply the initial jump force
            jumpSpeed = jumpForce;
            isJumping = true;
        }

        //if we let go of the button while jumping cut off our jump speed by half
        if (isJumping && jumpControl.ReadValue<float>() == 0 && jumpSpeed <= minJumpForce)
        {
            Debug.Log(jumpSpeed);
            if (jumpSpeed > 0)
            {
                jumpSpeed = jumpSpeed / 2;
            }
            
            isJumping = false;
        }

        //apply gravity
        if(jumpSpeed > maxFall)
        {
            jumpSpeed += -jumpDeaccel * Time.deltaTime;
        }
        else
        {
            jumpSpeed = maxFall;
        }


        //if we have started to move downwards we are not longer jumping
        if (jumpSpeed <= 0) isJumping = false;




    }

    #endregion

    #region Player facing direction
    void Rotate()
    {
        if(movement!= Vector2.zero || pState == playerState.summoning)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + player1Cam.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }
    #endregion

    #region Public Functions

    public bool ReadActionButton()
    {
        if (runControl.ReadValue<float>() == 1) return true;
        else return false;
    }

    public void OnSummoningEnter(GameObject circle)
    {
        //player can't move unless they let go of running
        //player is now in the summoning animation
        //the summoning circle is active
        //move player towards
        isOnCircle = true;
        activeCircle = circle;
    }

    public void OnSummoningExit()
    {
        //player can now move and summoning circle is not active
        //player is no longer in the summoning animation
        isOnCircle = false;
        activeCircle = null;
    }

    #endregion
}
