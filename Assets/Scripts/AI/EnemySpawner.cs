using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject AIPrefab;
    public int maxEnemiesPerSpawner = 3;
    public float minSpawnInterval = 10f;
    public float maxSpawnInterval = 30f;
    private int currentEnemiesCount = 0;
    int seed;

    private void Start()
    {
        //Random - randomizer
        Random.InitState(GetInstanceID());
        // Start the spawning process
        Invoke("SpawnEnemy", 3f);
    }

    private void SpawnEnemy()
    {
        if (currentEnemiesCount < maxEnemiesPerSpawner)
        {
            //The correct way to use instantiate
            Instantiate(AIPrefab, transform.position, Quaternion.identity, transform);
            currentEnemiesCount++;
        }

        // Reschedule the next spawn
        Invoke("SpawnEnemy", Random.Range(minSpawnInterval, maxSpawnInterval));
    }

    public void DecreaseEnemiesCount()
    {
        currentEnemiesCount--;
    }
}
