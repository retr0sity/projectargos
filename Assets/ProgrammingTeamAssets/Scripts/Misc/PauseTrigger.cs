using UnityEngine;

public class EnemyPauseTrigger : MonoBehaviour
{
    public float pauseDuration = 2f; // set in Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{name} triggered by {other.name}");
        Fascist enemy = other.GetComponentInParent<Fascist>(); // 👈 works even if collider is a child
        if (enemy != null)
        {
            enemy.TriggerPause(pauseDuration);
        }
    }
}
