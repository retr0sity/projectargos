using UnityEngine;

public class EnemyJumpTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyChase enemy = other.GetComponent<EnemyChase>();
        if (enemy != null)
        {
            enemy.TriggerJump();
        }
    }
}