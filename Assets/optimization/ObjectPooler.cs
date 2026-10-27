using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public int maxSpawns;
    public List<GameObject> pooledObjects;
    public List<GameObject> spawnedObjects;
    public GameObject prefab;
    public Transform spawn;

    private void Awake()
    {
        pooledObjects = new List<GameObject>();
        for(int i = 0; i < maxSpawns; i++)
        {
            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            pooledObjects.Add(obj);
            if (obj.TryGetComponent<ObjectTimer>(out ObjectTimer timer))
            {
                timer.OnDespawn += Despawn;
            }

                obj.SetActive(false);
        }
        StartCoroutine(SpawnTimer());
    }
   
    private IEnumerator SpawnTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Vector3 spawnPos = spawn.position;
            spawnPos += new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
            Spawn(spawnPos);
            yield return null;
        }
    }
    public void Spawn(Vector3 spawnPosition)
    {
      /*  if(pooledObjects.Count > 0)
        {
            GameObject spawned = pooledObjects[0];
            spawned.transform.position = spawnPosition;
            spawned.SetActive(true);
            pooledObjects.Remove(spawned);
            spawnedObjects.Add(spawned);

        }*/
      foreach(GameObject obj in pooledObjects)
        {
            if (obj.activeInHierarchy) continue;
            obj.SetActive(true);
            obj.transform.position = spawnPosition;
            break;
        }
    }

    public void Despawn(GameObject obj)
    {
        /*    if (spawnedObjects.Contains(obj))
            {
                obj.SetActive(false);
                pooledObjects.Add(obj);
                spawnedObjects.Remove(obj);
            }*/
        obj.SetActive(false);
    }
}
