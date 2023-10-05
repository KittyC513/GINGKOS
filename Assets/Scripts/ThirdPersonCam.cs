using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{

    [Header("Orientation References")]
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    public Transform player1Cam;
    [SerializeField]
    private float rotationSpeed;

    [Header("Movement References")]
    [SerializeField]
    private InputActionReference movementControl;
    [SerializeField]
    private InputActionReference jumpControl;
    [SerializeField]
    private InputActionReference runControl;
    [SerializeField]
    private InputActionReference aimControl;

    [Header("Movement")]
    Vector3 move;

    [Header("Ground Check")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private float groundDrag;
    [SerializeField]
    private float playerHeight;




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

    // Start is called before the first frame update
    void Start()
    {
        //set the cursor to invisible and be locked
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player1Cam = Camera.main.transform;

        //freeze the rigidbody's rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        
    }

    // Update is called once per frame
    void Update()
    {
        MovementInput();
        groundCheck();

        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

    }

    private void FixedUpdate()
    {
        MoveFunction(3f);
    }

    #region Player Movement
    void MovementInput()
    {
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        move = new Vector3(movement.x, 0, movement.y);
    }

    void MoveFunction(float moveSpeed)
    {
        //always move forward the way the camera's looking
        move = player1Cam.forward * move.z + player1Cam.right * move.x;
        move.y = 0f;

        //rotate player
        playerObj.forward = Vector3.Slerp(playerObj.forward, move, Time.deltaTime * rotationSpeed);

        //player movement 
        rb.AddForce(move.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    #endregion

    #region Ground Check
    void groundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

    }

    #endregion
}
