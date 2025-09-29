using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Fascist : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool facingRight = true; // true = moves to +X, false = -X

    [Header("Jumping")]
    public float jumpForce = 6f;

    private Rigidbody2D rb;
    private bool shouldJump = false;
    private bool isPaused = false;
    private bool isStopped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // keeps upright
    }

    void FixedUpdate()
    {
        // High-priority checks first.
        if (isStopped)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isPaused)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Normal movement only if not stopped or paused.
        float dir = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);

        // Jump logic (already uses physics).
        if (shouldJump && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            shouldJump = false;
        }
    }

    // === Trigger methods called from trigger scripts ===

    public void TriggerJump()
    {
        shouldJump = true;
    }

    public void TriggerPause(float duration)
    {
        if (!isPaused) StartCoroutine(PauseCoroutine(duration));
    }

    private IEnumerator PauseCoroutine(float duration)
    {
        isPaused = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        isPaused = false;
    }

    public void TriggerStop()
    {
        isStopped = true;
        rb.linearVelocity = Vector2.zero;
    }

    // Optional: resume manually
    public void Resume()
    {
        isStopped = false;
    }
}
