using UnityEngine;

public class DestroyOnGround : MonoBehaviour
{
    private int groundLayer;

    private void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground"); // replace "Ground" with your layer name
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            Destroy(gameObject);
        }
    }
}
