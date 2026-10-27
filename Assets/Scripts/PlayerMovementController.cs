using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerMovementController : MonoBehaviour
{
    private PlayerInputController playerInputController;
    public Vector3 moveDirection;
    public event Action<Vector3> OnWalk;
    private void Awake()
    {
        playerInputController = GetComponent<PlayerInputController>();
        playerInputController.OnMovePressed += UpdateMoveDirection;
    }

    private void UpdateMoveDirection(Vector3 moveVector_)
    {
        moveDirection = moveVector_;

        OnWalk?.Invoke(moveVector_);
    }
}
