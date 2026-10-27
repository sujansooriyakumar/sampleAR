using System.Collections.Generic;
using UnityEngine;

public class ProfilerTest : MonoBehaviour
{
    public GameObject prefab;
    public Transform spawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 10; i++)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        Vector3 spawnPos = spawn.position;
        spawnPos += new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
