using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
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
        
    }

    // Update is called once per frame
    void Update()
    {
        //read the input from new input system 
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);

        //we want player move forward according to camera's direction
        move = player1Cam.forward * move.z + player1Cam.right * move.x;
        move.y = 0f;

        playerObj.forward = Vector3.Slerp(playerObj.forward, move.normalized, Time.deltaTime * rotationSpeed);

    }
}
