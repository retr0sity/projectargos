using UnityEngine;
using System.Collections;

public class FogToggleTrigger : MonoBehaviour
{
    [Header("Fog Objects to Toggle")]
    [Tooltip("Drag the sprite follower GameObject here")]
    public GameObject fogFollowerSprite;

    [Tooltip("Drag the other fog controller GameObject here")]
    public GameObject fogController;

    [Header("Linked Triggers (Optional)")]
    [Tooltip("Trigger that forces fog ON when entered")]
    public Collider2D forceFogOnTrigger;

    [Tooltip("Trigger that forces fog OFF when entered")]
    public Collider2D forceFogOffTrigger;

    [Header("Settings")]
    [Tooltip("What tag should trigger this? Usually 'Player'")]
    public string playerTag = "Player";

    [Tooltip("Cooldown time to prevent rapid toggling")]
    public float toggleCooldown = 0.5f;

    [Tooltip("How long should the fog fade in/out transition take?")]
    [Range(0.1f, 5f)]
    public float transitionDuration = 1.5f;

    private float lastToggleTime = -1f;
    private Coroutine transitionRoutine;

    private void Start()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager not found! Make sure it exists in the scene.");
            return;
        }

        SetFogStateImmediate(GameStateManager.Instance.fogActive);
    }

    private void Update()
    {
        // Detect if player enters any of the "force" triggers
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        Vector2 playerPos = player.transform.position;

        if (forceFogOnTrigger && forceFogOnTrigger.OverlapPoint(playerPos))
        {
            ForceSetFog(true);
        }
        else if (forceFogOffTrigger && forceFogOffTrigger.OverlapPoint(playerPos))
        {
            ForceSetFog(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (Time.time < lastToggleTime + toggleCooldown)
            return;

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager.Instance is null!");
            return;
        }

        lastToggleTime = Time.time;

        bool newState = !GameStateManager.Instance.fogActive;
        GameStateManager.Instance.fogActive = newState;
        SetFogStateSmooth(newState);
    }

    /// <summary>
    /// Forces a specific fog state (used by fallback triggers)
    /// </summary>
    public void ForceSetFog(bool isActive)
    {
        if (GameStateManager.Instance == null)
            return;

        if (GameStateManager.Instance.fogActive == isActive)
            return; // already in correct state

        GameStateManager.Instance.fogActive = isActive;
        SetFogStateSmooth(isActive);
    }

    private void SetFogStateSmooth(bool isActive)
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(FadeFog(isActive));
    }

    private IEnumerator FadeFog(bool isActive)
    {
        float targetAlpha = isActive ? 1f : 0f;
        float duration = Mathf.Max(0.01f, transitionDuration);

        SpriteRenderer[] fogSprites = GetFogRenderers();

        if (isActive)
        {
            if (fogFollowerSprite) fogFollowerSprite.SetActive(true);
            if (fogController) fogController.SetActive(true);
        }

        float startAlpha = fogSprites.Length > 0 ? fogSprites[0].color.a : (isActive ? 0f : 1f);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            foreach (var sr in fogSprites)
            {
                Color c = sr.color;
                sr.color = new Color(c.r, c.g, c.b, newAlpha);
            }

            yield return null;
        }

        foreach (var sr in fogSprites)
        {
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, targetAlpha);
        }

        if (!isActive)
        {
            if (fogFollowerSprite) fogFollowerSprite.SetActive(false);
            if (fogController) fogController.SetActive(false);
        }

        transitionRoutine = null;
    }

    private void SetFogStateImmediate(bool isActive)
    {
        if (fogFollowerSprite) fogFollowerSprite.SetActive(isActive);
        if (fogController) fogController.SetActive(isActive);

        SpriteRenderer[] fogSprites = GetFogRenderers();
        foreach (var sr in fogSprites)
        {
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, isActive ? 1f : 0f);
        }
    }

    private SpriteRenderer[] GetFogRenderers()
    {
        var list = new System.Collections.Generic.List<SpriteRenderer>();
        if (fogFollowerSprite) list.AddRange(fogFollowerSprite.GetComponentsInChildren<SpriteRenderer>(true));
        if (fogController) list.AddRange(fogController.GetComponentsInChildren<SpriteRenderer>(true));
        return list.ToArray();
    }

    private void OnDrawGizmosSelected()
    {
        bool currentFogState = GameStateManager.Instance != null ? GameStateManager.Instance.fogActive : true;
        Gizmos.color = currentFogState ? Color.green : Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;

        BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
        if (boxCol != null)
            Gizmos.DrawWireCube(Vector3.zero, boxCol.size);

        CircleCollider2D circleCol = GetComponent<CircleCollider2D>();
        if (circleCol != null)
            Gizmos.DrawWireSphere(Vector3.zero, circleCol.radius);
    }
}