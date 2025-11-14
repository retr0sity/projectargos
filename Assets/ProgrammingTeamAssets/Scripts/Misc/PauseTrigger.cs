using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EnemyPauseTrigger : MonoBehaviour
{
    public float pauseDuration = 2f;
    public float spacing = 1.5f;
    private int enemyCount = 0;

    private List<Fascist> queue = new List<Fascist>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Fascist enemy = other.GetComponentInParent<Fascist>();
        if (enemy != null && !queue.Contains(enemy))
        {
            // Add to queue
            queue.Add(enemy);

            // First enemy should get the *leftmost* slot.
            // So we invert: the slot index is based on "how many are in queue"
            int index = queue.Count - 1;

            Vector3 stopPos = transform.position;
            stopPos.x += index * spacing; // 👈 shift to the RIGHT for later enemies

            enemy.TriggerPause(stopPos);

            enemyCount++;

            // Disable trigger if at max capacity
            if (enemyCount >= 2)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Fascist enemy = other.GetComponentInParent<Fascist>();
        if (enemy != null)
        {
            queue.Remove(enemy);
        }
    }
}

