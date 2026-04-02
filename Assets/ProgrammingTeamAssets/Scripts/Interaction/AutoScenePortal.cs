using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Automatic portal that transports player to another scene when entered.
/// No interaction required — player walks into it = instant scene load.
/// Saves player's current position so they can return later via AutoReturnTrigger.
/// Requires "Player" tag on player and GameStateManager in scene.
/// Each portal tracks its own usage independently.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AutoScenePortal : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private string targetSceneName = "";
    [SerializeField] private bool oneTimeUse = true;
    [SerializeField] private string portalID = "";

    [Header("Return Position")]
    [Tooltip("Assign a Transform to specify an exact return spot. If empty, the portal's own position is used.")]
    [SerializeField] private Transform customReturnPoint;

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
            Debug.LogError($"{gameObject.name} needs a Collider2D component!");

        if (string.IsNullOrEmpty(portalID))
            portalID = $"{SceneManager.GetActiveScene().name}_{gameObject.name}";
    }

    void Start()
    {
        if (oneTimeUse && GameStateManager.Instance != null && GameStateManager.Instance.HasPortalBeenUsed(portalID))
            gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isLoading) return;
        if (oneTimeUse && hasBeenUsed) return;
        if (oneTimeUse && GameStateManager.Instance != null && GameStateManager.Instance.HasPortalBeenUsed(portalID)) return;

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager not found in scene!");
            return;
        }

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError($"No target scene set for portal {gameObject.name}");
            return;
        }

        StartCoroutine(PortalFadeOut());
    }

    private IEnumerator PortalFadeOut()
    {
        isLoading = true;
        hasBeenUsed = true;

        if (oneTimeUse)
            GameStateManager.Instance.MarkPortalAsUsed(portalID);

        Vector3 returnPos = customReturnPoint != null ? customReturnPoint.position : transform.position;
        GameStateManager.Instance.SetReturnPoint(SceneManager.GetActiveScene().name, returnPos);

        Debug.Log($"[AutoPortal] Fading to {targetSceneName}, return point saved at {returnPos}");

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

        SceneManager.LoadScene(targetSceneName);
    }

    public void ResetPortal()
    {
        isLoading = false;
        hasBeenUsed = false;
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.ResetPortal(portalID);
        gameObject.SetActive(true);
    }
}