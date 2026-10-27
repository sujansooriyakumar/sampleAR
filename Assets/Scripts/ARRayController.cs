using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ARRayController : MonoBehaviour
{
    public InputActionAsset arActions;
    public string pressActionName = "Screen Press";
    public string positionActionName = "Screen Position";
    public UnityEvent<RaycastHit> OnPlaneHit;
    private InputAction screenPressAction;
    private InputAction screenPosAction;
    public Vector3 screenPos;
    public LayerMask targetLayer;
    private Camera cam;

    public static ARRayController Instance;
    private void Awake()
    {
        Instance = this;
        screenPressAction = arActions.FindAction(pressActionName, throwIfNotFound: true);
        screenPosAction = arActions.FindAction(positionActionName, throwIfNotFound: true);
        cam = Camera.main;


    }

    private void OnEnable()
    {
        screenPressAction.Enable();
        screenPosAction.Enable();
        screenPressAction.performed += SpawnPress_Performed;
    }

    private void OnDisable()
    {
        screenPressAction.Disable();
        screenPosAction.Disable();
        screenPressAction.performed -= SpawnPress_Performed;
    }

    private void SpawnPress_Performed(InputAction.CallbackContext c)
    {
        //Vector3 origin = cam.ScreenToWorldPoint(screenPos);
        bool hit = Physics.Raycast(cam.ScreenPointToRay(screenPos), out RaycastHit hitInfo, Mathf.Infinity, targetLayer);


        if (hit)
        {

            OnPlaneHit?.Invoke(hitInfo);
        }
    }

    private void Update()
    {
        screenPos = screenPosAction.ReadValue<Vector2>();
    }


}
