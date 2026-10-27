using UnityEngine;

[RequireComponent(typeof(PlayerMovementController))]
public class PlayerAnimationController : MonoBehaviour
{
    private void Awake()
    {
        PlayerMovementController movementController = GetComponent<PlayerMovementController>();
        movementController.OnWalk += UpdateWalkAnimation;
    }
    private void UpdateWalkAnimation(Vector3 moveDirection_)
    {
        // update the walk cycle
    }
}
