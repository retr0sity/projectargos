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

    

    public void Init(Vector2 pushForce)
    {
        _pushForce = pushForce;
        // Move left across the screen
        _rb.linearVelocity = Vector2.left * moveSpeed;
    }

    void Awake()
{
    _rb = GetComponent<Rigidbody2D>();
    _rb.gravityScale = 0f;
    _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    Collider2D col = GetComponent<Collider2D>();
    if (col != null) col.isTrigger = true;
    Destroy(gameObject, lifetime);
}

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.AddForce(_pushForce, ForceMode2D.Force);

        if (FeatherGameManager.Instance != null)
            FeatherGameManager.Instance.NotifyFeatherHit();
    }

    void Update()
    {
        if (FeatherGameManager.Instance == null) return;

        // Continuously push any player in range
        Collider2D hit = Physics2D.OverlapBox(transform.position, GetComponent<Collider2D>().bounds.size, 0f);
        if (hit != null && hit.CompareTag("Player"))
        {
            Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.AddForce(_pushForce * Time.deltaTime * 50f, ForceMode2D.Force);
                FeatherGameManager.Instance.NotifyFeatherHit();
            }
        }

        // Destroy when far behind player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && transform.position.x < player.transform.position.x - 20f)
            Destroy(gameObject);
    }
    
    void OnDestroy()
    {
        Debug.Log($"[ObstacleFeather] Destroyed at position {transform.position}, time alive: {Time.time}");
    }
}