using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawning : MonoBehaviour
{
    public ProjectileBehavior ProjectilePrefab; // The projectile prefab to spawn
    public Transform SpawnPoint; // The location where the projectile will be spawned

    // Start is called before the first frame update
    private void Start()
    {
        // Begin the coroutine to spawn projectiles
        StartCoroutine(SpawnProjectileRoutine());
    }

    // Coroutine that handles the spawning of projectiles at regular intervals
    private IEnumerator SpawnProjectileRoutine()
    {
        // Infinite loop to continuously spawn projectiles
        while (true)
        {
            // Calculate the wait time between spawns based on the player's score
            float waitTime = 3f - (ScoreManager.instance.score / 5) * 0.5f;

            // Ensure the wait time doesn't go below 0.5 seconds
            waitTime = Mathf.Max(waitTime, 0.5f);

            // Wait for the calculated time before spawning the next projectile
            yield return new WaitForSeconds(waitTime);

            // Spawn a new projectile
            SpawnProjectile();
        }
    }

    // Spawns a projectile at the specified spawn point
    private void SpawnProjectile()
    {
        // Check if the projectile prefab and spawn point are set
        if (ProjectilePrefab != null && SpawnPoint != null)
        {
            // Instantiate the projectile at the spawn point with the current rotation
            Instantiate(ProjectilePrefab, SpawnPoint.position, transform.rotation);
        }
    }
}
