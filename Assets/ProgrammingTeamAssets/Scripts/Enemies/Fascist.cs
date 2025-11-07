using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Fascist : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool facingRight = true; // true = moves to +X, false = -X

    [Header("Jumping")]
    public float jumpForce = 6f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool shouldJump = false;

    // === Queue / slot system ===
    private bool hasSlot = false;           // assigned slot to move to
    private Vector3 targetSlot;
    private bool isPausing = false;         // true while actually paused
    private float pauseDuration = 0f;       // for pause slots

    private bool isStopping = false;        // true if this slot is a permanent stop
    private bool isStopped = false;         // frozen permanently

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true; // keeps upright
    }

    void FixedUpdate()
    {
        // 1️⃣ Permanent stop
        if (isStopped)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        // 2️⃣ Currently pausing at slot
        if (isPausing)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        // 3️⃣ Moving toward a slot (pause or stop)
        if (hasSlot)
        {
            float dist = Mathf.Abs(transform.position.x - targetSlot.x);
            if (dist > 0.05f)
            {
                // Walk left toward slot
                rb.linearVelocity = new Vector2(-Mathf.Abs(moveSpeed), rb.linearVelocity.y);
                animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            }
            else
            {
                // Arrived at slot
                rb.linearVelocity = Vector2.zero;
                animator.SetFloat("Speed", 0f);
                hasSlot = false;

                if (isStopping)
                {
                    // Lock permanently
                    isStopped = true;
                    isStopping = false;
                }
                else
                {
                    // Pause until trigger resumes
                    HandlePauseAtSlot();
                }
            }
            return;
        }

        // 4️⃣ Normal movement
        float dir = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        // Jump logic
        if (shouldJump && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            shouldJump = false;
        }
    }

    // === Trigger methods ===

    public void TriggerJump()
    {
        shouldJump = true;
    }

    // Pause slot: enemy walks to slot, then pauses until resumed
    public void TriggerPause(Vector3 stopPosition)
    {
        targetSlot = stopPosition;
        hasSlot = true;
        isStopping = false;
        isPausing = false;
    }

    // Instead of a coroutine, we directly mark as paused when reaching the slot
    private void HandlePauseAtSlot()
    {
        isPausing = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetFloat("Speed", 0f);
    }
    
    public void ResumePause()
    {
        if (isPausing)
        {
            isPausing = false;
        }
    }


    // Stop slot: enemy walks to slot, then freezes permanently
    public void TriggerStop(Vector3 stopPosition)
    {
        targetSlot = stopPosition;
        hasSlot = true;
        isStopping = true;
    }

    // Optional manual resume (for stopped enemies)
    public void Resume()
    {
        isStopped = false;
    }
}