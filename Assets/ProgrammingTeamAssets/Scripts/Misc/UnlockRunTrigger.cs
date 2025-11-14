using UnityEngine;

public class UnlockRunTrigger : MonoBehaviour
{
    // Example: in a trigger zone script
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<RigPlayerController>().UnlockRun();
        }
    }
}
