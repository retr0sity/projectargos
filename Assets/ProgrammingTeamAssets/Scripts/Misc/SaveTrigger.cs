using UnityEngine;

public class SaveTrigger : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.SavePlayer();
                Debug.Log("Game saved!");
            }
        }
    }
}