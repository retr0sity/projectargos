using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraTriggerController : MonoBehaviour
{
   [Header("Cinemachine Reference")]
    public CinemachineVirtualCamera playerVCam;

    [Header("Spot Position")]
    public Transform spotTransform; // Drag empty GO here

    [Header("Timing & Speed")]
    [Range(0.1f, 10f)] public float stayDuration = 3f;
    [Range(0.5f, 5f)] public float moveDuration = 1.5f; // Pan speed (lower = faster)
    [Range(0f, 10f)] public float triggerCooldown = 2f;

    [Header("Advanced")]
    public bool oneTimeOnly = false; // Disable after first trigger?
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // For smoother lerp (edit in Inspector)

    private bool canTrigger = true;
    private Coroutine routine;
    private Transform originalFollow;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!canTrigger || !other.CompareTag("Player")) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        canTrigger = false;
        originalFollow = playerVCam.Follow; // Backup

        // Step 1: Disable follow, lerp to spot
        playerVCam.Follow = null;
        Vector3 startPos = playerVCam.transform.position;
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(elapsed / moveDuration); // Eased lerp
            playerVCam.transform.position = Vector3.Lerp(startPos, spotTransform.position, t);
            yield return null;
        }
        playerVCam.transform.position = spotTransform.position; // Snap end

        // Step 2: Stay at spot
        yield return new WaitForSeconds(stayDuration);

        // Step 3: Lerp back to player's CURRENT position
        startPos = playerVCam.transform.position;
        Vector3 targetPos = originalFollow.position;
        targetPos.z = startPos.z; // Keep Z
        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(elapsed / moveDuration);
            // Update target mid-lerp (if player moves)
            targetPos = originalFollow.position;
            targetPos.z = startPos.z;
            playerVCam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Re-enable follow (damping smooths any snap)
        playerVCam.Follow = originalFollow;

        // Cooldown/Reset
        yield return new WaitForSeconds(triggerCooldown);
        if (!oneTimeOnly) canTrigger = true;
        routine = null;
    }
}