using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float Speed = 3f; // Speed at which the projectile moves

    // Update is called once per frame
    private void Update()
    {
        // Move the projectile to the left based on speed and time
        transform.position += -transform.right * Time.deltaTime * Speed;
    }

    // Called when the projectile collides with another object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is not the player
        if (collision.gameObject.tag != "Player")
        {
            ScoreManager.instance.AddScore(); // Increase the score
            Destroy(gameObject); // Destroy the projectile
        }
    }
}
