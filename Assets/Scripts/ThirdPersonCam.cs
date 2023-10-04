using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        //rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        //Vector3 viewDir = player.position - new Vector3(transform.position.x, transform.position.y, player.position.z);
        Debug.Log(viewDir.normalized);
        orientation.forward = viewDir.normalized;

        //rotate player object
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        float horizontalInput = movement.x;
        float verticalInput = movement.y;
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(inputDir != Vector3.zero)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

    }
}
