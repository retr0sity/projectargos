using UnityEngine;

/// <summary>
/// An obstacle feather that flies in from the right and pushes the player on contact.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ObstacleFeather : MonoBehaviour
{
    private Vector2 _pushForce;
    private Rigidbody2D _rb;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float lifetime  = 10f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
        Destroy(gameObject, lifetime);
    }

    public void Init(Vector2 pushForce)
    {
        _pushForce = pushForce;
        // Move left across the screen
        _rb.linearVelocity = Vector2.left * moveSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.AddForce(_pushForce, ForceMode2D.Impulse);

        Destroy(gameObject);
    }

    void Update()
    {
        // Destroy if too far left (off screen)
        if (transform.position.x < -20f)
            Destroy(gameObject);
    }
}