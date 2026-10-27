using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemySO ghost;
    public Transform player;
    public float spawnDelay;
    public int maxEnemyCount;
    public List<GameObject> spawnedEnemies;
    public GameObject deadFX;
    private void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }
    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (spawnedEnemies.Count < maxEnemyCount)
            {
                SpawnEnemy(ghost);
            }
           
            yield return new WaitForSeconds(spawnDelay);
            yield return null;
        }
    }

    private void SpawnEnemy(EnemySO config_)
    {
        GameObject tmp = Instantiate(config_.prefab, transform.position, config_.prefab.transform.rotation);
        spawnedEnemies.Add(tmp);
        EnemyAI enemyAI = tmp.GetComponent<EnemyAI>();
        enemyAI.movementSpeed = config_.movementSpeed;
        enemyAI.rotationSpeed = config_.rotationSpeed;
        enemyAI.hp = config_.hp;
        enemyAI.strength = config_.strength;
        enemyAI.player = player;
        enemyAI.OnKilled += RemoveEnemyFromList;
    }

    private void RemoveEnemyFromList(GameObject enemy)
    {
        Instantiate(deadFX, enemy.transform.position, Quaternion.identity);
        spawnedEnemies.Remove(enemy);
    }
}
