using UnityEngine;

public class ConsoleWakeUp : MonoBehaviour
{
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("WakeUp");
        }
    }
}