using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Health : MonoBehaviour
{
    public int maxHealth = 1;
    public int currentHealth;
    public Animator playerAnimator;
    private SpriteRenderer spriteRenderer;
    public Animator transition;
    private AudioSource mainCameraAudioSource;

    void Start()
    {
        currentHealth = maxHealth; // initialize health
        spriteRenderer = GetComponent<SpriteRenderer>(); // get sprite renderer
        mainCameraAudioSource = Camera.main.GetComponent<AudioSource>(); // get main camera audio source
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            if (mainCameraAudioSource != null)
            {
                mainCameraAudioSource.Stop(); // stop background music
            }
            FindObjectOfType<AudioManager>().Play("death_sfx"); // play death sound effect
            playerAnimator.enabled = false; // kokkaloma paixth
            Time.timeScale = 0; // kokkaloma pistas
            transition.updateMode = AnimatorUpdateMode.UnscaledTime; // allows transition to run even when game is frozen
            StartCoroutine(DeathEffectCoroutine());
        }
    }

    private IEnumerator DeathEffectCoroutine()
    {
        int steps = 3; // 3 seconds to lose human rights
        float stepDuration = 1.0f; // obvious

        for (int i = 0; i < steps; i++)
        {
            if (i != 0)
            {
                FindObjectOfType<AudioManager>().Play("dying_sfx"); // play the death first, dying hits later mothafakasssss
            }
            float grayAmount = (float)(i + 1) / steps; // maurisma
            SetSceneGrayscale(grayAmount); // gradually apply grayscale effect
            yield return new WaitForSecondsRealtime(stepDuration); // wait without being affected by time scale
        }

        FindObjectOfType<AudioManager>().Play("dying_sfx"); // final death sfx before transition
        transition.SetTrigger("Start"); // trigger transition animation
        yield return new WaitForSecondsRealtime(1); // dramatic efe

        SceneManager.LoadScene("02_death"); // psofos
        playerAnimator.enabled = true; // restore player animation state
        Time.timeScale = 1; // restore normal game speed
        transition.updateMode = AnimatorUpdateMode.Normal; // reset transition mode
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
}


