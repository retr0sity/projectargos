using UnityEngine;

public class SpriteFollower : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector2 offset = Vector2.zero;
    
    void LateUpdate()
    {
        if (player == null) return;
        
        // Match player's X and Y with offset, keep own Z
        transform.position = new Vector3(
            player.position.x + offset.x, 
            player.position.y + offset.y, 
            transform.position.z
        );
    }
}
