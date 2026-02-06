using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Core.Managers;

public class Health : MonoBehaviour
{
    public int maxHealth = 1;
    public int currentHealth;
    public Animator playerAnimator;
    private SpriteRenderer spriteRenderer;
    public Animator transition;
    private AudioSource mainCameraAudioSource;
    private BasePlayerController playerController;
    private bool isDead;
    private const string DeathSceneName = "02_death";

    void Start()
    {
        currentHealth = maxHealth; // initialize health
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();

        if (transition == null)
            TryResolveTransitionAnimator();

        spriteRenderer = GetComponent<SpriteRenderer>(); // get sprite renderer
        mainCameraAudioSource = Camera.main != null ? Camera.main.GetComponent<AudioSource>() : null; // get main camera audio source
        playerController = GetComponent<BasePlayerController>(); // get player controller
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            isDead = true;

            // Lock controls
            if (InputManager.Instance != null)
                InputManager.Instance.SetControlLock(true);

            playerController?.ForceStop();

            if (mainCameraAudioSource != null)
            {
                mainCameraAudioSource.Stop(); // stop background music
            }
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            audioManager?.Play("death_sfx"); // play death sound effect

            if (playerAnimator != null)
                playerAnimator.enabled = false; // kokkaloma paixth

            Time.timeScale = 0; // kokkaloma pistas
            if (transition != null)
                transition.updateMode = AnimatorUpdateMode.UnscaledTime; // allows transition to run even when game is frozen

            StartCoroutine(DeathEffectCoroutine());
        }
    }

    private IEnumerator DeathEffectCoroutine()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        int steps = 3; // 3 seconds to lose human rights
        float stepDuration = 1.0f; // obvious

        for (int i = 0; i < steps; i++)
        {
            if (i != 0)
            {
                audioManager?.Play("dying_sfx"); // play the death first, dying hits later mothafakasssss
            }
            float grayAmount = (float)(i + 1) / steps; // maurisma
            SetSceneGrayscale(grayAmount); // gradually apply grayscale effect
            yield return new WaitForSecondsRealtime(stepDuration); // wait without being affected by time scale
        }

        audioManager?.Play("dying_sfx"); // final death sfx before transition
        if (transition != null)
        {
            transition.SetTrigger("Start"); // trigger transition animation
            yield return new WaitForSecondsRealtime(1); // dramatic efe
            transition.updateMode = AnimatorUpdateMode.Normal; // reset transition mode
        }

        if (playerAnimator != null)
            playerAnimator.enabled = true; // restore player animation state

        Time.timeScale = 1; // restore normal game speed
        if (InputManager.Instance != null)
            InputManager.Instance.SetControlLock(false);

        SceneManager.LoadScene(DeathSceneName); // psofos
    }

    private void SetSceneGrayscale(float amount)
    {
        // find all sprite renderers and tilemap renderers sto current scene
        SpriteRenderer[] spriteRenderers = FindObjectsOfType<SpriteRenderer>();
        TilemapRenderer[] tilemapRenderers = FindObjectsOfType<TilemapRenderer>();

        // update grayscale for fuck knows what
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            if (renderer.material.HasProperty("_GrayscaleAmount"))
            {
                renderer.material.SetFloat("_GrayscaleAmount", amount);
            }
        }

        // update grayscale for fucking tilemap renderers
        foreach (TilemapRenderer renderer in tilemapRenderers)
        {
            if (renderer.material.HasProperty("_GrayscaleAmount"))
            {
                renderer.material.SetFloat("_GrayscaleAmount", amount);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("DamagingObject"))
        {
            TakeDamage(1);  // Deal 1 damage when touching an enemy or damaging object
        }
    }

    private void TryResolveTransitionAnimator()
    {
        SceneLoader loader = FindObjectOfType<SceneLoader>();
        if (loader != null && loader.transition != null)
        {
            transition = loader.transition;
            return;
        }

        SceneLoader[] loaders = FindObjectsOfType<SceneLoader>();
        foreach (SceneLoader candidate in loaders)
        {
            if (candidate != null && candidate.transition != null)
            {
                transition = candidate.transition;
                return;
            }
        }
    }
}
