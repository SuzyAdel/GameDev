using UnityEngine;

public class FallingRocksManager : MonoBehaviour
{
    public GameObject[] rockPrefabs; // Array of rock prefabs
    public float minSpawnTime = 0.1f;
    public float maxSpawnTime = 0.5f;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = Camera.main.transform; // Assuming player is the main camera for reference
        InvokeRepeating("SpawnRock", 5f, Random.Range(minSpawnTime, maxSpawnTime));
    }

    void SpawnRock()
    {
        // Choose a random rock prefab
        GameObject rockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
        // Spawn rock at random X/Z in front of the player
        float spawnX = Random.Range(-10f, 10f);
        float spawnZ = Random.Range(30f, 40f);
        Vector3 spawnPosition = playerTransform.position + new Vector3(spawnX, 40f, spawnZ);

        GameObject rock = Instantiate(rockPrefab, spawnPosition, Quaternion.identity);

        // Apply random torque to the rock for rotation effect
        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 10f);
        }

        // You can implement further behavior for the rocks, like falling and colliding with the player
    }
}
