using UnityEngine;

public class EnemyStopTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyChase enemy = other.GetComponent<EnemyChase>();
        if (enemy != null)
        {
            enemy.TriggerStop();
        }
    }
}