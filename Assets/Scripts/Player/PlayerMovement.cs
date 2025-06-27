using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller; // Reference to the character controller script
    public Animator animator; // Reference to the animator component

    public float runSpeed = 40f; // Player movement speed

    float horizontalMove = 0f; // Stores horizontal movement input
    bool jump = false; // Tracks if the player is jumping

    // Update is called once per frame
    void Update()
    {
        // Get horizontal movement input and multiply by run speed
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        // Update animator speed parameter based on movement input
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        // Only allow jumping if the current scene is not the "death" scene
        if (SceneManager.GetActiveScene().name != "death")
        {
            if (Input.GetButtonDown("Jump")) // Check if jump button is pressed
            {
                jump = true; // Set jump flag to true
                animator.SetBool("IsJumping", true); // Trigger jumping animation
            }
        }

        // Disable movement in arduino scene
        if (SceneManager.GetActiveScene().buildIndex == 7)
        {
            horizontalMove = 0f; // Stop horizontal movement
        }
    }

    // Called when the player lands on the ground
    public void OnLanding()
    {
        animator.SetBool("IsJumping", false); // Reset jumping animation
    }

    void FixedUpdate()
    {
        // Move the character based on input
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false; // Reset jump flag after movement
    }
}
