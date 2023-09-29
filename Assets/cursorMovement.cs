using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
namespace JL.Player.UI
{
    public class cursorMovement : MonoBehaviour
    {
        [SerializeField] RectTransform _cursorParent;
        [SerializeField] Image _normalCursor;
        [SerializeField] Image _hoverCursor;
        [SerializeField] EventSystem _eventSystem;
        /*[ShowInInspector, ReadOnly]*/
        bool _hovering = false;
        /*[ShowInInspector, ReadOnly]*/
        bool _visible = false;
        /*[ShowInInspector, ReadOnly]*/
        bool _isGamepad = false;
        /*[ShowInInspector, ReadOnly]*/
        bool _isCursorFree = false;
        Vector2 _halfScreen;
        /*[ShowInInspector, ReadOnly]*/
        Vector2 _cursorPosition;
        PlayerInput _controls;
        ButtonControl _uiClickButton;
        private void Start()
        {
            _halfScreen = new Vector2(Screen.width, Screen.height) / 2;
            _cursorPosition = _halfScreen;
            _controls = new PlayerInput();
            //_controls.Enable();
        }
        void Update()
        {
            //_isGamepad = InputMaster.currentInputDevice ==
                //InputMaster.InputDeviceType.Gamepad;
            //_isCursorFree = InputMaster.IsBlocked(CharCtrlType.MouseLook);
            if (!_isGamepad || !_isCursorFree)
            {
                SetVisible(false);
                if (Mouse.current != null && !_isGamepad)
                {
                    _cursorPosition = Mouse.current.position.ReadValue();
                }
                else
                {
                    _cursorPosition = _halfScreen;
                }
                return;
            }
            _hovering = _eventSystem.IsPointerOverGameObject();
            SetVisible(true);
            //Vector2 delta = _controls.Player.Look.ReadValue<Vector2>();
            //_cursorPosition += delta;
            _cursorPosition.x = Mathf.Clamp(_cursorPosition.x, 0, Screen.width);
            _cursorPosition.y = Mathf.Clamp(_cursorPosition.y, 0, Screen.height);
            //Mouse.current.WarpCursorPosition(_cursorPosition);
            InputState.Change(Mouse.current.position, _cursorPosition);
            _cursorParent.position = _cursorPosition;
            /*if (_controls.UI.Click.activeControl != null)
            {
                _uiClickButton =
                    _controls.UI.Click.activeControl as ButtonControl;
            }
            if (_uiClickButton != null)
            {
                bool isPressed = _uiClickButton.IsPressed();
                Mouse.current.CopyState<MouseState>(out var mouseState);
                mouseState = mouseState.WithButton(MouseButton.Left, isPressed);
                InputState.Change(Mouse.current, mouseState);
            }
            */
        }
        void SetVisible(bool isVisible)
        {
            _visible = isVisible;
            _cursorParent.gameObject.SetActive(isVisible);
            if (!isVisible)
            {
                _normalCursor.gameObject.SetActive(false);
                _hoverCursor.gameObject.SetActive(false);
            }
            else
            {
                _normalCursor.gameObject.SetActive(!_hovering);
                _hoverCursor.gameObject.SetActive(_hovering);
            }
        }
    }
}