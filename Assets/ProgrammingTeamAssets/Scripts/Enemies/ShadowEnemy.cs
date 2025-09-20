using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private bool shouldJump = false;
    private bool isPaused = false;
    private bool isStopped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // keep upright
    }

    void Update()
    {
        if (player == null) return;

        if (!isPaused && !isStopped)
        {
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
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // stop horizontal movement
        }
    }

    // Called from jump triggers
    public void TriggerJump()
    {
        shouldJump = true;
    }

    // Called from PauseTrigger
    public void TriggerPause(float duration)
    {
        if (!isPaused) StartCoroutine(PauseCoroutine(duration));
    }
    
    private IEnumerator PauseCoroutine(float duration)
    {
        isPaused = true;
        yield return new WaitForSeconds(duration);
        isPaused = false;
    }

    // Called from StopTrigger
    public void TriggerStop()
    {
        isStopped = true;
        rb.linearVelocity = Vector2.zero;
    }
}