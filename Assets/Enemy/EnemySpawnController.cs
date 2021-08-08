using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{

    [SerializeField] private Transform[] spawners;
    private float currentSpawnTime = 100f;
    [SerializeField] private float spawnRate = 100f;
    [SerializeField] GameObject enemy;
    [SerializeField] private int maxAliveCount = 25;
    private int currentEnemyCount = 0;
  
    // Update is called once per frame
    void Update()
    {
        IncrementSpawnTime();
    }

    private void IncrementSpawnTime()
    {
        currentSpawnTime += Time.deltaTime;
        if (currentSpawnTime >= spawnRate && currentEnemyCount < maxAliveCount)
        {
            currentSpawnTime = 0f;
            currentEnemyCount += 1;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        int spawnerIndex = Random.Range(0, spawners.Length);
        Instantiate(enemy);
        enemy.transform.position = spawners[spawnerIndex].position;
    }
}
