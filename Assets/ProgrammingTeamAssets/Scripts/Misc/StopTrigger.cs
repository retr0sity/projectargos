using UnityEngine;
using System.Collections.Generic;

public class EnemyStopTrigger : MonoBehaviour
{
    public float spacing = 1.5f;

    private List<Fascist> queue = new List<Fascist>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Fascist enemy = other.GetComponentInParent<Fascist>();
        if (enemy != null && !queue.Contains(enemy))
        {
            queue.Add(enemy);

            // Index determines how far back this enemy should stop
            int index = queue.Count - 1;

            Vector3 stopPos = transform.position;
            stopPos.x += index * spacing; // 👈 shift right for later enemies

            enemy.TriggerStop(stopPos);
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
