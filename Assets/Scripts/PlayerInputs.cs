using System;

using UnityEngine;

using UnityEngine.InputSystem;

using Utilities;

public class PlayerInputs : Singleton<PlayerInputs> {
    private Inputs _action;
    public Inputs Action => _action;

    public bool QuickTime = false;
    public Vector2 MousePosition;

    public InputAction FireAction => _action.Player.Fire;
    public InputAction QuickTimeAction => _action.Player.QuickTime;
    public InputAction MousePositionAction => _action.Player.MousePosition;

    public Vector2 RelativeDirection(Vector2 position) {
        return (MousePosition - position).normalized;
    }

    public float MouseAngle(Vector2 position) {
        return Vector2.SignedAngle(Vector2.up, RelativeDirection(position));
    }

    public Quaternion MouseRotation(Vector2 position) {
        return Quaternion.AngleAxis(MouseAngle(position), Vector3.forward);
    }

    protected override void Awake() {
        base.Awake();
        _action = new Inputs();
        _action.Player.MousePosition.performed += UpdateMousePosition;
        _action.Player.QuickTime.started += StartQuickTime;
        _action.Player.QuickTime.canceled += EndQuickTime;
        _action.Enable();
    }

    private void StartQuickTime(InputAction.CallbackContext context) {
        QuickTime = true;
    }

    private void EndQuickTime(InputAction.CallbackContext context) {
        QuickTime = false;
    }

    private void UpdateMousePosition(InputAction.CallbackContext context) {
        MousePosition = Helpers.Instance.MainCamera.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }
}