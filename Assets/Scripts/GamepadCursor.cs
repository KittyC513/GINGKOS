using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.UIElements;
using MouseButton = UnityEngine.InputSystem.LowLevel.MouseButton;

public class GamepadCursor : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private RectTransform cursorTransform;
    [SerializeField]
    private float cursorSpeed = 1000f;
    [SerializeField]
    private RectTransform canvasRectTransform;
    [SerializeField]
    private Canvas canvas;

    private Mouse virtualMouse;

    private bool previousMouseState;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        mainCamera = Camera.main;
        if(virtualMouse == null)
        {
            virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
        }
        else if(!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse); 
        }

        //pair the device to the user to use the PlayerInput component with the event system & the Virtual Mouse
        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if(cursorTransform != null)
        {
            //access to the gamepad input
            Vector2 position = cursorTransform.anchoredPosition; // change the cursor position regarding its pivot
            InputState.Change(virtualMouse.position, position); 
        }
        //update to the new position
        InputSystem.onAfterUpdate += UpdateMotion;
    }

    private void onDistable()
    {
        InputSystem.onAfterUpdate -= UpdateMotion;
    }

    private void UpdateMotion()
    {
        //it requires virtual mouse and gamepad to run the function
        if(virtualMouse == null || Gamepad.current == null)
        {
            return;
        }
        //read value from Gamepad
        Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
        deltaValue *= cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        //set the boundary
        newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
        newPosition.x = Mathf.Clamp(newPosition.y, 0, Screen.height);

        //move the position
        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        bool yButtonIsPressed = Gamepad.current.yButton.IsPressed();
        if(previousMouseState != yButtonIsPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, Gamepad.current.yButton.IsPressed());
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = yButtonIsPressed;
        }

        AnchorCursor(newPosition);

    }

    private void AnchorCursor(Vector2 position)
    {
        //Change the cursor with the screen sizes
        
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    
    }
}
