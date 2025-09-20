using UnityEngine;

public class EnemyPauseTrigger : MonoBehaviour
{
    public float pauseDuration = 2f; // set in Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyChase enemy = other.GetComponent<EnemyChase>();
        if (enemy != null)
        {
            enemy.TriggerPause(pauseDuration);
        }
    }
}