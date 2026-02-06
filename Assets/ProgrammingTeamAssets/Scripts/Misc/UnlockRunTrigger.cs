using UnityEngine;

public class UnlockRunTrigger : MonoBehaviour
{
    // Example: in a trigger zone script
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (collision.TryGetComponent(out RigPlayerController controller))
        {
            controller.UnlockRun();
        }
    }
}
