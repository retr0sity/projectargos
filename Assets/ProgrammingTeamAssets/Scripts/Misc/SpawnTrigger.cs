using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public GameObject objectToSpawn; // The prefab you want to spawn
    public Transform spawnPoint;     // Where you want it to spawn

    private bool hasSpawned = false; // Ensure it only spawns once

    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug log to confirm trigger
        Debug.Log("Triggered by: " + other.name);

        // Check if the player triggered the collider
        if (!hasSpawned && other.CompareTag("Player"))
        {
            Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            hasSpawned = true;
        }
    }
}
