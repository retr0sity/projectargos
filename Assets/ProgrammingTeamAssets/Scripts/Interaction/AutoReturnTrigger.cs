using UnityEngine;
using System.Collections;

/// <summary>
/// Automatic trigger that returns player to saved position when entered.
/// Place this in the final scene of your mini-game. Player walks into it = instant return.
/// Requires GameStateManager in scene.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AutoReturnTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool oneTimeUse = true;

    [Header("Transition")]
    [SerializeField] private Animator transition;

    private bool hasBeenUsed = false;
    private bool isLoading = false;

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
        else
            Debug.LogError($"{gameObject.name} needs a Collider2D component to work as a trigger!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneTimeUse && hasBeenUsed) return;
        if (isLoading) return;

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager not found! Cannot return player.");
            return;
        }

        StartCoroutine(ReturnFadeOut());
    }

    private IEnumerator ReturnFadeOut()
    {
        isLoading = true;
        hasBeenUsed = true;

        if (transition != null)
        {
            transition.updateMode = AnimatorUpdateMode.UnscaledTime;
            transition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(1f);
            transition.updateMode = AnimatorUpdateMode.Normal;
        }
        else
        {
            yield return null;
        }

        GameStateManager.Instance.ReturnToSavedPosition();
    }

    public void CutsceneReturn()
    {
        StartCoroutine(ReturnFadeOut());
    }

    public void ResetTrigger()
    {
        isLoading = false;
        hasBeenUsed = false;
    }
}