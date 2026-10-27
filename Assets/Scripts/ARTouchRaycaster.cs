using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Shoots a ray from the touched screen position out to a maximum distance from the AR camera.
/// Uses the Input System package (new input system) only.
/// Checks both Unity physics colliders and (optionally) AR trackables such as detected planes.
/// </summary>
public class ARTouchRaycaster : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The AR camera. Defaults to Camera.main if left empty.")]
    [SerializeField] private Camera arCamera;

    [Tooltip("Optional. Assign to also hit AR planes/feature points.")]
    [SerializeField] private ARRaycastManager arRaycastManager;

    [Header("Input (New Input System)")]
    [Tooltip("Button action that triggers the ray. Use the inline default or reference an action from your Input Actions asset.")]
    [SerializeField]
    private InputActionProperty pressAction = new InputActionProperty(
        new InputAction("Press", InputActionType.Button, "<Touchscreen>/primaryTouch/press"));

    [Tooltip("Vector2 action giving the screen position of the press.")]
    [SerializeField]
    private InputActionProperty positionAction = new InputActionProperty(
        new InputAction("Position", InputActionType.Value, "<Touchscreen>/primaryTouch/position", expectedControlType: "Vector2"));

    [Tooltip("Adds mouse bindings to the inline actions so you can test in the Editor.")]
    [SerializeField] private bool addMouseBindingsForEditor = true;

    [Header("Ray Settings")]
    [Tooltip("How far (in meters) the ray travels from the camera.")]
    [SerializeField] private float rayDistance = 5f;

    [SerializeField] private LayerMask physicsLayers = ~0;
    [SerializeField] private TrackableType arTrackableTypes = TrackableType.PlaneWithinPolygon;
    [SerializeField] private bool ignoreTouchesOverUI = true;

    [Header("Debug")]
    [Tooltip("Optional LineRenderer used to visualize the ray.")]
    [SerializeField] private LineRenderer debugLine;
    [SerializeField] private float debugLineDuration = 0.5f;

    [Header("Events")]
    public UnityEvent<Vector3> onHit;   // fired with the hit point
    public UnityEvent<Vector3> onMiss;  // fired with the ray's end point at max distance

    private static readonly List<ARRaycastHit> s_ARHits = new List<ARRaycastHit>();
    private float debugLineHideTime;

    public struct RayShotResult
    {
        public bool didHit;
        public Vector3 origin;
        public Vector3 endPoint;      // hit point, or the point at max distance if nothing was hit
        public float distance;
        public GameObject hitObject;  // collider's GameObject or AR trackable's GameObject (may be null)
        public bool hitARTrackable;
    }

    private void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
        if (debugLine != null) debugLine.enabled = false;

#if UNITY_EDITOR
        // Only modify inline actions, never actions referenced from an asset
        if (addMouseBindingsForEditor)
        {
            if (pressAction.reference == null && pressAction.action != null)
                pressAction.action.AddBinding("<Mouse>/leftButton");
            if (positionAction.reference == null && positionAction.action != null)
                positionAction.action.AddBinding("<Mouse>/position");
        }
#endif
    }

    private void OnEnable()
    {
        pressAction.action?.Enable();
        positionAction.action?.Enable();
    }

    private void OnDisable()
    {
        // Only disable inline actions; referenced asset actions may be shared with other scripts
        if (pressAction.reference == null) pressAction.action?.Disable();
        if (positionAction.reference == null) positionAction.action?.Disable();
    }

    private void Update()
    {
        // Polled in Update (rather than in a performed callback) so the UI check below is reliable
        if (pressAction.action != null && pressAction.action.WasPressedThisFrame())
        {
            if (!(ignoreTouchesOverUI && IsPointerOverUI()))
            {
                Vector2 screenPos = positionAction.action.ReadValue<Vector2>();
                ShootRay(screenPos, rayDistance);
            }
        }

        if (debugLine != null && debugLine.enabled && Time.time >= debugLineHideTime)
            debugLine.enabled = false;
    }

    /// <summary>
    /// Shoots a ray from the given screen position out to 'distance' meters from the camera.
    /// </summary>
    public RayShotResult ShootRay(Vector2 screenPosition, float distance)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        var result = new RayShotResult
        {
            didHit = false,
            origin = ray.origin,
            endPoint = ray.GetPoint(distance),
            distance = distance
        };

        // 1. Physics colliders (virtual objects you've placed)
        if (Physics.Raycast(ray, out RaycastHit physicsHit, distance, physicsLayers, QueryTriggerInteraction.Ignore))
        {
            result.didHit = true;
            result.endPoint = physicsHit.point;
            result.distance = physicsHit.distance;
            result.hitObject = physicsHit.collider.gameObject;
            Debug.Log(result.hitObject.name);
        }

        // 2. AR trackables (planes, feature points), keep whichever hit is closer
        if (arRaycastManager != null && arRaycastManager.Raycast(ray, s_ARHits, arTrackableTypes))
        {
            // Hits are sorted by distance, closest first
            ARRaycastHit arHit = s_ARHits[0];
            float arDistance = Vector3.Distance(ray.origin, arHit.pose.position);

            if (arDistance <= distance && (!result.didHit || arDistance < result.distance))
            {
                result.didHit = true;
                result.hitARTrackable = true;
                result.endPoint = arHit.pose.position;
                result.distance = arDistance;
                result.hitObject = arHit.trackable != null ? arHit.trackable.gameObject : null;
            }
        }

        if (result.didHit) onHit?.Invoke(result.endPoint);
        else onMiss?.Invoke(result.endPoint);

        DrawDebug(result);
        return result;
    }

    /// <summary>Convenience overload using the Inspector's rayDistance.</summary>
    public RayShotResult ShootRay(Vector2 screenPosition) => ShootRay(screenPosition, rayDistance);

    private static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // With InputSystemUIInputModule, pass the touch ID for touches
        var touchscreen = Touchscreen.current;
        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
            return EventSystem.current.IsPointerOverGameObject(touchscreen.primaryTouch.touchId.ReadValue());

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void DrawDebug(RayShotResult result)
    {
        Debug.DrawLine(result.origin, result.endPoint, result.didHit ? Color.green : Color.red, debugLineDuration);

        if (debugLine == null) return;
        debugLine.positionCount = 2;
        // Start slightly in front of the camera so the line is visible on device
        debugLine.SetPosition(0, result.origin + arCamera.transform.forward * 0.05f);
        debugLine.SetPosition(1, result.endPoint);
        debugLine.startColor = debugLine.endColor = result.didHit ? Color.green : Color.red;
        debugLine.enabled = true;
        debugLineHideTime = Time.time + debugLineDuration;
    }
}