using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScenePortal : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private string targetSceneName = "";
    [SerializeField] private bool oneTimeUse = true;
    [SerializeField] private string portalID = "";

    [Header("Return Position")]
    [Tooltip("Assign a Transform to specify an exact return spot. If empty, the portal's own position is used.")]
    [SerializeField] private Transform customReturnPoint;
    [Tooltip("Should using this portal save a return point? Disable for portals that are part of a linear sequence (e.g. elevator).")]
    [SerializeField] private bool saveReturnPoint = true; // <-- the new toggle

    [Header("Transition")]
    [SerializeField] private Animator transition;

    private bool isLoading = false;

    void Awake()
    {
        if (gameObject.tag == "Untagged")
            gameObject.tag = "Interactable";

        if (string.IsNullOrEmpty(portalID))
            portalID = $"{SceneManager.GetActiveScene().name}_{gameObject.name}";
    }

    void Start()
    {
        if (oneTimeUse && GameStateManager.Instance != null && GameStateManager.Instance.HasPortalBeenUsed(portalID))
            gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        Debug.Log($"[Portal] OnInteract called, target: {targetSceneName}");
        if (isLoading) return;
        if (oneTimeUse && GameStateManager.Instance != null && GameStateManager.Instance.HasPortalBeenUsed(portalID)) return;

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager not found in scene!");
            return;
        }

        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogError("Player not found! Cannot save return position.");
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

        if (oneTimeUse)
            GameStateManager.Instance.MarkPortalAsUsed(portalID);

        // Only overwrite the return point if this portal is supposed to set one
        if (saveReturnPoint)
        {
            Vector3 returnPos = customReturnPoint != null ? customReturnPoint.position : transform.position;
            GameStateManager.Instance.SetReturnPoint(SceneManager.GetActiveScene().name, returnPos);
            Debug.Log($"[Portal] Return point saved at {returnPos}");
        }
        else
        {
            Debug.Log($"[Portal] saveReturnPoint is off — existing return point preserved.");
        }

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
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.ResetPortal(portalID);
        gameObject.SetActive(true);
    }
}