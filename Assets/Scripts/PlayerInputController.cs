using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerInputController : MonoBehaviour
{
    public InputAction moveInput;
    public event Action<Vector3> OnMovePressed;

    private void Awake()
    {
        moveInput.Enable();
        moveInput.performed += MovePressed;
        moveInput.canceled += MovePressed;

    }

    private void MovePressed(InputAction.CallbackContext context)
    {
        Vector3 moveVector = context.ReadValue<Vector2>();
        OnMovePressed?.Invoke(moveVector);
    }
}
