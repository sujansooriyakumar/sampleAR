using UnityEngine;
using UnityEngine.InputSystem;

public class BallSpawner : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions;
    public string pressActionName = "Throw Press";
    public string positionActionName = "Throw Position";

    [Header("Spawning")]
    public Rigidbody ballPrefab;
    public float spawnDistance;

    [Header("Launch")]
    public float maxLaunchSpeed = 20f;
    [Range(0.05f, 1f)]
    public float fullDragLength = 0.5f;
    [Range(0f, 0.2f)]
    public float minDragLength = 0.02f;

    private InputAction pressAction;
    private InputAction positionAction;
    private Camera cam;

    private Rigidbody heldBall;
    private bool needsStartPosition;
    private Vector2 dragStartPosition;
    private Vector2 dragCurrentPosition;

    private void Awake()
    {
        pressAction = inputActions.FindAction(pressActionName, throwIfNotFound: true);
        positionAction = inputActions.FindAction(positionActionName, throwIfNotFound: true);
        cam = Camera.main;
    }

    private void OnEnable()
    {
        pressAction.Enable();
        positionAction.Enable();

        pressAction.performed += OnPress;
        pressAction.canceled += OnRelease;

    }

    private void OnDisable()
    {
        pressAction.Disable();
        positionAction.Disable();

        pressAction.performed -= OnPress;
        pressAction.canceled -= OnRelease; 
    }

    private void Update()
    {
        if (heldBall == null) return;

        Vector2 pos = positionAction.ReadValue<Vector2>();
        if (needsStartPosition)
        {
            dragStartPosition = pos;
            needsStartPosition = false;
        }

        dragCurrentPosition = pos;
        heldBall.MovePosition(GetSpawnPosition());
    }

    private void OnPress(InputAction.CallbackContext c)
    {
        if (heldBall != null) return;
        heldBall = Instantiate(ballPrefab, GetSpawnPosition(), cam.transform.rotation);
        heldBall.isKinematic = true;
        needsStartPosition = true;
    }

    private void OnRelease(InputAction.CallbackContext c)
    {
        if (heldBall == null) return;
        if (needsStartPosition)
        {
            dragCurrentPosition = dragStartPosition;
        }

        float dragLength = (dragCurrentPosition - dragStartPosition).magnitude / Screen.height;
        heldBall.isKinematic = false;

        if(dragLength >= minDragLength)
        {
            float power = Mathf.Clamp01(dragLength / fullDragLength);
            heldBall.AddForce(cam.transform.forward * (power * maxLaunchSpeed), ForceMode.VelocityChange);
        }
        heldBall = null;
    }

    private Vector3 GetSpawnPosition()
    {
        return cam.transform.position + cam.transform.forward * spawnDistance;
    }
}
