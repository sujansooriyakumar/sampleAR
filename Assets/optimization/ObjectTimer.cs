using System;
using System.Collections;
using UnityEngine;

public class ObjectTimer : MonoBehaviour
{
    public event Action<GameObject> OnDespawn;
    public float despawnTimer = 3.0f;

    private void Start()
    {
        StartCoroutine(DespawnTimer(despawnTimer));
    }
    private IEnumerator DespawnTimer(float time)
    {
        yield return new WaitForSeconds(time);
        OnDespawn?.Invoke(this.gameObject);

    }
}
