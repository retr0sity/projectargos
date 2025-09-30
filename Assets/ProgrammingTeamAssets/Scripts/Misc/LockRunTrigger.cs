using UnityEngine;

public class LockRunTrigger : MonoBehaviour
{
    // Example: in a trigger zone script
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<RigPlayerController>().LockRun();
        }
    }
}