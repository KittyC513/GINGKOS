using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference movementControl;
    [SerializeField]
    private InputActionReference jumpControl;
    [SerializeField]
    private InputActionReference runControl;
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
    private Vector3 moveDir;

    //local speed value
    [SerializeField]
    private float currentSpeed;

    private bool running = false;
   
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform player1Cam;
    Vector2 movement;

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        runControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        runControl.action.Disable();
    }


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        player1Cam = Camera.main.transform;
        moveDir = Vector3.zero;
    }

    void Update()
    {
        Move();
        Jump();
        GroundDetect();
    }

    private void FixedUpdate()
    {
        
        Rotate();
    }

    #region Ground Check
    void GroundDetect()
    {
        //ground check, if player collides with ground layer, groundedPlayer equels true
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
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
        move.y = 0f;

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
            if (currentSpeed >= playerRunSpeed)
            {
                currentSpeed += ((0 - playerWalkSpeed) / 0.6f) * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, playerWalkSpeed, playerRunSpeed);
            }
            else
            {
                currentSpeed = Mathf.Clamp(currentSpeed, 0, playerWalkSpeed);
            }
           
        }


        controller.Move(transform.forward * Time.deltaTime * currentSpeed);


    }


    #endregion

    #region Jump Function
    void Jump()
    {
        //input is pressed and player is grounded, the jump can be triggered
        if (jumpControl.action.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
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
