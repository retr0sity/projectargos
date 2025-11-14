using UnityEngine;

public class EnemyJumpTrigger : MonoBehaviour
{

    // EnemyJumpTrigger.cs
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{name} triggered by {other.name}");
        Fascist enemy = other.GetComponentInParent<Fascist>();
        if (enemy != null)
        {
            enemy.TriggerJump();
        }
    }


}