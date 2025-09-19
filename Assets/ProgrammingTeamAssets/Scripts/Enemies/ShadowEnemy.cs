using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    public Transform player;        
    public float moveSpeed = 5f;    
    public float jumpForce = 8f;    

    private Rigidbody2D rb;
    private bool shouldJump = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // keep upright
    }

    void Update()
    {
        if (player == null) return;

        // Move toward player on X-axis only
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // Jump if triggered
        if (shouldJump && Mathf.Abs(rb.linearVelocity.y) < 0.01f) // only if grounded
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            shouldJump = false;
        }
    }

    // Called from jump triggers
    public void TriggerJump()
    {
        shouldJump = true;
    }
}