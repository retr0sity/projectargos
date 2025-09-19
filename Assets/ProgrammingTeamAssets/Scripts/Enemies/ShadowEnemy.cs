using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class RigidbodyShadowEnemy : MonoBehaviour
{
    public Transform player;
    public float followDelay = 1.0f;      // seconds behind the player
    public float recordInterval = 0.02f;  // how often to record positions
    public float moveSpeed = 10f;         // how fast enemy catches up

    private Queue<Vector2> playerPositions = new Queue<Vector2>();
    private float timer;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Record player positions
        timer += Time.deltaTime;
        if (timer >= recordInterval)
        {
            playerPositions.Enqueue(player.position);
            timer = 0f;
        }
    }

    void FixedUpdate()
    {
        // Only move if enough history has built up
        if (playerPositions.Count * recordInterval > followDelay)
        {
            Vector2 targetPos = playerPositions.Dequeue();

            // Use Rigidbody movement (respects gravity, collisions, etc.)
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }
}