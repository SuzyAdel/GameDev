using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardSpawner : MonoBehaviour
{
    // 25% probability of spawning a shard every 5 seconds
    public GameObject[] shardPrefabs; // Array to store shard prefabs
    public float spawnInterval = 5f; // Time interval between spawns
    public float spawnRangeX = 10f; // Range for random X-axis position
    public float spawnRangeZ = 10f; // Range for random Z-axis position
    public float spawnHeight = 40f; // Fixed Y-axis height for shard spawn

    private void Start()
    {
        // Start spawning shards at regular intervals
        InvokeRepeating("TrySpawnShard", 0f, spawnInterval);
    }

    // Function that tries to spawn a shard based on a 25% probability
    void TrySpawnShard()
    {
        // 25% chance to spawn a shard
        if (Random.value <= 0.25f)
        {
            SpawnShard();
        }
        Debug.Log("Random Value: " + Random.value);

    }

    // Function to spawn a shard at a random position within specified range
    void SpawnShard()
    {
        // Random position for the shard
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), spawnHeight, Random.Range(30f, spawnRangeZ));

        // Randomly select a shard prefab
        GameObject shardPrefab = shardPrefabs[Random.Range(0, shardPrefabs.Length)];

        // Instantiate the shard at the random position
        Instantiate(shardPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Spawning shard...");
        Debug.Log("Spawning shard at position: " + shardPrefab.transform.position);

    }
}
