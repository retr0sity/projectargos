using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Animator doorAnimator; // Assign the Animator in the Inspector
    public string booleanName;    // Specify the name of the boolean parameter in the Animator

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the Player
        if (collision.CompareTag("Player"))
        {
            // Set the boolean to true in the Animator
            if (doorAnimator != null && !string.IsNullOrEmpty(booleanName))
            {
                doorAnimator.SetBool(booleanName, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the colliding object is the Player
        if (collision.CompareTag("Player"))
        {
            // Set the boolean to false in the Animator
            if (doorAnimator != null && !string.IsNullOrEmpty(booleanName))
            {
                doorAnimator.SetBool(booleanName, false);
            }
        }
    }
}
