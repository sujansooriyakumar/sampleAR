using NUnit.Framework;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARShootController : MonoBehaviour
{
  public struct RayShotResult
    {
        public bool didHit;
        public Vector3 origin;
        public Vector3 endPoint;
        public float distance;
        public GameObject hitObject;
        public bool hitARTrackable;
    }

    [SerializeField] private Camera arCamera;
    [SerializeField] private ARRaycastManager arRaycastManager;

    [SerializeField]
    private InputActionProperty pressAction = new InputActionProperty(
        new InputAction("Press", InputActionType.Button, "<Touchscreen>/primaryTouch/press"));

    [Header("Raycast settings")]
    [SerializeField] private float rayDistance = 5f;
    [SerializeField] private LayerMask hitLayers = 0;
    [SerializeField] private bool otherLayersBlockRay = true;
    [SerializeField] private TrackableType arTrackableTypes = TrackableType.AllTypes;

    [Header("Debug")]
    [SerializeField] LineRenderer debugLine;
    [SerializeField] private float debugLineDuration;
    [SerializeField] private float debugLineHideTime;

    [Header("Events")]
    public UnityEvent<Vector3> onHit;
    public UnityEvent<Vector3> onMiss;
    public UnityEvent onShoot;

    private static readonly List<ARRaycastHit> ARHits = new List<ARRaycastHit>();

    private void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
        if (debugLine != null) debugLine.enabled = false;

#if UNITY_EDITOR
        pressAction.action.AddBinding("<Mouse>/leftButton");
#endif
    }

    private void OnEnable()
    {
        pressAction.action?.Enable();
    }

    private void OnDisable()
    {
        if(pressAction.reference != null) pressAction.action?.Disable();

    }

    private void Update()
    {
        if(pressAction.action != null && pressAction.action.WasPressedThisFrame())
        {
            ShootRay(rayDistance);
            onShoot?.Invoke();
        }

        if(debugLine != null && debugLine.enabled && Time.time >= debugLineHideTime)
        {
            debugLine.enabled = false;
        }
    }

    public RayShotResult ShootRay(float distance)
    {
        // get center of screen

        Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RayShotResult result = new RayShotResult
        {
            didHit = false,
            origin = ray.origin,
            endPoint = ray.GetPoint(distance),
            distance = distance
        };

        // check physics objects
        int castMask = otherLayersBlockRay ? Physics.AllLayers : hitLayers.value;
        if(Physics.Raycast(ray, out RaycastHit physicsHit, distance, castMask, QueryTriggerInteraction.Ignore))
        {
            GameObject hitGO = physicsHit.collider.gameObject;

            if (IsOnHitLayer(hitGO))
            {
                result.didHit = true;
                result.endPoint = physicsHit.point;
                result.distance = physicsHit.distance;
                result.hitObject = hitGO;
                Debug.Log(hitGO.name);
                if(hitGO.TryGetComponent<EnemyAI>(out EnemyAI enemyAI))
                {
                    enemyAI.TakeDamage(1.0f);
                }
            }
            else
            {
                distance = physicsHit.distance;
                result.endPoint = physicsHit.point;
                result.distance = physicsHit.distance;
            }
        }

        // check ar trackables

        if(arRaycastManager != null && arRaycastManager.Raycast(ray, ARHits, arTrackableTypes))
        {
            foreach(ARRaycastHit arHit in ARHits)
            {
                if(arHit.trackable == null || !IsOnHitLayer(arHit.trackable.gameObject))
                {
                    continue;
                }

                float arDistance = Vector3.Distance(ray.origin, arHit.pose.position);

                if(arDistance <= distance && (!result.didHit || arDistance < result.distance))
                {
                    result.didHit = true;
                    result.hitARTrackable = true;
                    result.endPoint = arHit.pose.position;
                    result.distance = arDistance;
                    result.hitObject = arHit.trackable.gameObject;
                }
                break;
            }
        }

        if (result.didHit) onHit?.Invoke(result.endPoint);
        else onMiss?.Invoke(result.endPoint);

        DrawDebug(result);
        return result;
    }

    private void DrawDebug(RayShotResult result)
    {
        Debug.DrawLine(result.origin, result.endPoint, result.didHit ? Color.green : Color.red, debugLineDuration);

        if (debugLine == null) return;
        debugLine.positionCount = 2;
        Transform cam = arCamera.transform;
        debugLine.SetPosition(0, result.origin + cam.forward * 0.05f - cam.up * 0.02f);
        debugLine.SetPosition(1, result.endPoint);
        debugLine.startColor = debugLine.endColor = result.didHit ? Color.green : Color.red;
        debugLine.enabled = true;
        debugLineHideTime = Time.time + debugLineDuration;
    }

    private bool IsOnHitLayer(GameObject go) => (hitLayers.value & (1 << go.layer)) != 0;
}
