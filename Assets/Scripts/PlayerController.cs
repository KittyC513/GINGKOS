using System.Collections;
using System.Collections.Generic;
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
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 4;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform player1Cam;
    Vector2 movement;

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
    }


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        player1Cam = Camera.main.transform;
    }

    void Update()
    {
        Move();
        Jump();
    }

    private void FixedUpdate()
    {
        GroundDetect();
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

        controller.Move(move * Time.deltaTime * playerSpeed);

        //move function
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
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
