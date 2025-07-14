using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Core.Managers;

[RequireComponent(typeof(Rigidbody2D))]
public class ArduinoBehavior : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("How strong the jump impulse is")]
    [SerializeField] private float jumpForce = 12f;
    [Tooltip("Ground checking layer mask")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Empty GameObject at feet for ground check")]
    [SerializeField] private Transform groundCheck;
    [Tooltip("Radius to check for ground")]
    [SerializeField] private float groundCheckRadius = 0.1f;

    [Header("Death & UI")]
    [SerializeField] private Animator playerAnimator;      // Player Animator
    [SerializeField] private TMP_Text textMeshPro;         // UI text for death messages
    [SerializeField] private List<string> texts;           // Messages to type on death
    [SerializeField] private float typingSpeed = 0.1f;     // Letter-per-second speed

    private Rigidbody2D _rb;
    private AudioManager _audioManager;
    private bool _isGrounded;
    private bool _jumpRequested;
    private bool _isDead;
    private int _currentTextIndex;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioManager = FindObjectOfType<AudioManager>();
        if (_audioManager == null)
            Debug.LogWarning("No AudioManager found in scene.");
    }

    private void OnEnable()
    {
        InputManager.Instance.JumpEvent += OnJumpRequested;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.JumpEvent -= OnJumpRequested;
    }

    private void Update()
    {
        // Ground check
        _isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);
        playerAnimator.SetBool("IsJumping", !_isGrounded);
    }

    private void FixedUpdate()
    {
        if (_jumpRequested && _isGrounded && 
            SceneManager.GetActiveScene().name != "death")
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _jumpRequested = false;
        }
    }

    private void OnJumpRequested()
    {
        _jumpRequested = true;
    }

    /// <summary>
    /// Called by other scripts / damage sources to kill the player.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (_isDead) return;
        _isDead = true;

        // Stop background & play death SFX
        _audioManager?.Stop("arduino");
        _audioManager?.Play("death_sfx");
        _audioManager?.Play("arduino_death");

        // Freeze animations & game
        playerAnimator.enabled = false;
        Time.timeScale = 0f;

        StartCoroutine(ArduinoDeathSequence());
    }

    private IEnumerator ArduinoDeathSequence()
    {
        // Type out each message
        while (_currentTextIndex < texts.Count)
        {
            textMeshPro.text = "";
            foreach (char c in texts[_currentTextIndex])
            {
                textMeshPro.text += c;
                yield return new WaitForSecondsRealtime(typingSpeed);
            }
            _currentTextIndex++;
            yield return new WaitForSecondsRealtime(1f);
        }

        // Small pause, then reload
        yield return new WaitForSecondsRealtime(4f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("08_cave_afterminigame");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
