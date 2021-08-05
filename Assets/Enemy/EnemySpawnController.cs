using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{

    [SerializeField] private Transform[] spawners;
    private float currentSpawnTime = 0f;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        IncrementSpawnTime();
    }

    private void IncrementSpawnTime()
    {
        currentSpawnTime += Time.deltaTime;
        if (currentSpawnTime >= spawnRate)
        {
            currentSpawnTime = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        int spawnerIndex = Random.Range(0, spawners.Length - 1);
        Instantiate(enemy);
        enemy.transform.position = spawners[spawnerIndex].position;
    }
}
