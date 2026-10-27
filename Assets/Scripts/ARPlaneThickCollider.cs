using UnityEngine;
using UnityEngine.XR.ARFoundation;

// Add to your AR plane prefab alongside ARPlane.
// Gives the plane a solid box collider with real thickness, sized to the plane's bounds.
[RequireComponent(typeof(ARPlane), typeof(BoxCollider))]
public class ARPlaneThickCollider : MonoBehaviour
{
    public float thickness = 0.1f;  // meters; extends below the plane surface

    private ARPlane plane;
    private BoxCollider box;

    private void Awake()
    {
        plane = GetComponent<ARPlane>();
        box = GetComponent<BoxCollider>();
    }

    private void OnEnable() => plane.boundaryChanged += OnBoundaryChanged;
    private void OnDisable() => plane.boundaryChanged -= OnBoundaryChanged;

    private void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs args)
    {
        // Plane space: the surface is the local XZ plane, with +Y as the plane's normal
        Vector2 size = plane.size;
        Vector2 center = plane.centerInPlaneSpace;

        box.size = new Vector3(size.x, thickness, size.y);
        box.center = new Vector3(center.x, -thickness * 0.5f, center.y);
    }
}