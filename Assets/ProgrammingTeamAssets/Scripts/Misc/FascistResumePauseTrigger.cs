using UnityEngine;

public class FascistResumePauseTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            // Resume all paused enemies
            Fascist[] enemies = FindObjectsOfType<Fascist>();
            foreach (Fascist enemy in enemies)
            {
                enemy.ResumePause();
            }
        }
    }
}