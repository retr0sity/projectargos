using UnityEngine;

public class EnemyStopTrigger : MonoBehaviour
{

    // EnemyStopTrigger.cs
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{name} triggered by {other.name}");
        Fascist enemy = other.GetComponentInParent<Fascist>();
        if (enemy != null)
        {
            enemy.TriggerStop();
        }
    }


}