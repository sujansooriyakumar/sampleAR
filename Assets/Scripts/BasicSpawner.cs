using UnityEngine;

public class BasicSpawner : MonoBehaviour
{
    public GameObject prefab;

    public void Spawn(RaycastHit hit)
    {
        GameObject obj = Instantiate(prefab, hit.point, Quaternion.identity);
        Collider objCol = obj.GetComponent<Collider>();

        Vector3 n = hit.normal;
        Vector3 ext = objCol.bounds.extents;

        float support = Mathf.Abs(n.x) * ext.x + Mathf.Abs(n.y) * ext.y + Mathf.Abs(n.z) * ext.z;

        Vector3 pivot = objCol.bounds.center - obj.transform.position;
        obj.transform.position = hit.point + n * (support + 0.001f) - pivot;
    }
}
