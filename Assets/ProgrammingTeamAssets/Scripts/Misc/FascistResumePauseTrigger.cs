using UnityEngine;

public class FascistResumePauseTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Find all paused enemies in the scene
            Fascist[] enemies = FindObjectsOfType<Fascist>();
            foreach (Fascist enemy in enemies)
            {
                enemy.ResumePause();
            }
        }
    }
}
