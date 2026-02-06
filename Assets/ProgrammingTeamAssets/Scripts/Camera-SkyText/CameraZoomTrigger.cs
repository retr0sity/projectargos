using UnityEngine;
using Cinemachine;
using System.Collections;

/// <summary>
/// Trigger-based orthographic camera zoom and framing adjustment using Cinemachine.
/// Can keep following the player, or detach and move to a world/player-relative focus point.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CameraZoomTrigger : MonoBehaviour
{
    private enum FocusMode
    {
        FollowOffset = 0,
        DetachToWorldTarget = 1,
        DetachRelativeToPlayer = 2
    }

    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera targetCamera;
    
    [Header("Zoom + Framing")]
    [SerializeField] private float targetZoom = 40f;
    [SerializeField] private Vector3 cameraOffset = Vector3.zero;
    [Tooltip("Legacy support field. Used only if Framing Transposer camera distance is configured.")]
    [SerializeField] private float targetCameraDistance = 0f;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private bool returnOnExit = true;
    
    [Header("Detach Focus")]
    [SerializeField] private FocusMode focusMode = FocusMode.FollowOffset;
    [SerializeField] private Transform worldFocusTarget;
    [SerializeField] private Vector3 playerRelativeFocusOffset = new Vector3(0f, 6f, 0f);
    [SerializeField] private bool trackPlayerWhileDetaching = true;
    [SerializeField] private bool overrideDetachedZ = true;
    [SerializeField] private float detachedZ = -10f;

    [Header("Optional Auto Return")]
    [SerializeField] private bool autoReturnAfterDelay = false;
    [SerializeField] private float holdDuration = 2f;

    [Header("Trigger Settings")]
    [SerializeField] private bool oneTimeUse = false;
    
    private float originalZoom;
    private Vector3 originalOffset;
    private float originalCameraDistance;
    private Vector3 originalVirtualCamPosition;
    private Transform originalFollow;
    private Transform originalLookAt;
    private Transform cachedPlayer;

    private bool hasTriggered = false;
    private bool isFocused = false;
    private bool initialized = false;
    private Coroutine currentTransition;
    private Coroutine autoReturnRoutine;
    private CinemachineFramingTransposer framingTransposer;
    
    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
    
    void Start()
    {
        EnsureInitialized();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneTimeUse && hasTriggered) return;
        
        hasTriggered = true;
        ApplyCameraSettings();
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!returnOnExit) return;
        
        ResetCameraSettings();
    }
    
    public void ApplyCameraSettings()
    {
        EnsureInitialized();
        if (!initialized || targetCamera == null) return;

        if (currentTransition != null)
            StopCoroutine(currentTransition);

        if (autoReturnRoutine != null)
            StopCoroutine(autoReturnRoutine);

        if (!isFocused)
            CaptureCurrentStateAsOriginal();

        isFocused = true;
        currentTransition = StartCoroutine(TransitionToTarget());

        if (autoReturnAfterDelay && !returnOnExit)
            autoReturnRoutine = StartCoroutine(AutoReturnRoutine());
    }
    
    public void ResetCameraSettings()
    {
        EnsureInitialized();
        if (!initialized || targetCamera == null || !isFocused) return;

        if (currentTransition != null)
            StopCoroutine(currentTransition);

        if (autoReturnRoutine != null)
        {
            StopCoroutine(autoReturnRoutine);
            autoReturnRoutine = null;
        }

        currentTransition = StartCoroutine(TransitionToOriginal());
    }

    private IEnumerator AutoReturnRoutine()
    {
        yield return new WaitForSeconds(holdDuration);
        autoReturnRoutine = null;
        ResetCameraSettings();
    }

    private IEnumerator TransitionToTarget()
    {
        if (targetCamera == null) yield break;

        float duration = Mathf.Max(0.01f, transitionDuration);
        float startZoom = targetCamera.m_Lens.OrthographicSize;
        float toDistance = targetCameraDistance > 0f ? targetCameraDistance : originalCameraDistance;
        Vector3 startOffset = framingTransposer != null ? framingTransposer.m_TrackedObjectOffset : Vector3.zero;
        float startDistance = framingTransposer != null ? framingTransposer.m_CameraDistance : 0f;
        Vector3 startPosition = targetCamera.transform.position;
        Vector3 detachedTargetPosition = ResolveDetachedTargetPosition(startPosition);
        bool detach = focusMode != FocusMode.FollowOffset;

        if (detach)
        {
            targetCamera.Follow = null;
            targetCamera.LookAt = null;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            targetCamera.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, targetZoom, t);

            if (framingTransposer != null)
            {
                framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(startOffset, cameraOffset, t);

                if (targetCameraDistance > 0)
                {
                    framingTransposer.m_CameraDistance = Mathf.Lerp(startDistance, toDistance, t);
                }
            }

            if (detach)
            {
                if (focusMode == FocusMode.DetachRelativeToPlayer && trackPlayerWhileDetaching)
                {
                    detachedTargetPosition = ResolveDetachedTargetPosition(startPosition);
                }

                targetCamera.transform.position = Vector3.Lerp(startPosition, detachedTargetPosition, t);
            }

            yield return null;
        }

        targetCamera.m_Lens.OrthographicSize = targetZoom;
        if (framingTransposer != null)
        {
            framingTransposer.m_TrackedObjectOffset = cameraOffset;

            if (targetCameraDistance > 0)
            {
                framingTransposer.m_CameraDistance = toDistance;
            }
        }

        if (detach)
        {
            targetCamera.transform.position = detachedTargetPosition;
        }

        currentTransition = null;
    }

    private IEnumerator TransitionToOriginal()
    {
        if (targetCamera == null) yield break;

        float duration = Mathf.Max(0.01f, transitionDuration);
        float startZoom = targetCamera.m_Lens.OrthographicSize;
        Vector3 startOffset = framingTransposer != null ? framingTransposer.m_TrackedObjectOffset : Vector3.zero;
        float startDistance = framingTransposer != null ? framingTransposer.m_CameraDistance : 0f;
        Vector3 startPosition = targetCamera.transform.position;
        bool detach = focusMode != FocusMode.FollowOffset;

        if (detach)
        {
            targetCamera.Follow = null;
            targetCamera.LookAt = null;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            targetCamera.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, originalZoom, t);

            if (framingTransposer != null)
            {
                framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(startOffset, originalOffset, t);
                framingTransposer.m_CameraDistance = Mathf.Lerp(startDistance, originalCameraDistance, t);
            }

            if (detach)
            {
                targetCamera.transform.position = Vector3.Lerp(startPosition, originalVirtualCamPosition, t);
            }

            yield return null;
        }

        targetCamera.m_Lens.OrthographicSize = originalZoom;
        if (framingTransposer != null)
        {
            framingTransposer.m_TrackedObjectOffset = originalOffset;
            framingTransposer.m_CameraDistance = originalCameraDistance;
        }

        if (detach)
        {
            targetCamera.transform.position = originalVirtualCamPosition;
        }

        targetCamera.Follow = originalFollow;
        targetCamera.LookAt = originalLookAt;

        isFocused = false;
        currentTransition = null;
    }

    private void EnsureInitialized()
    {
        if (targetCamera == null)
            targetCamera = ResolveDefaultVirtualCamera();

        if (cachedPlayer == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) cachedPlayer = player.transform;
        }

        if (targetCamera == null)
        {
            initialized = false;
            Debug.LogError("CameraZoomTrigger: No CinemachineVirtualCamera found!");
            return;
        }

        LensSettings lens = targetCamera.m_Lens;
        if (!lens.Orthographic)
        {
            lens.Orthographic = true;
            targetCamera.m_Lens = lens;
        }

        framingTransposer = targetCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        initialized = true;
    }

    private CinemachineVirtualCamera ResolveDefaultVirtualCamera()
    {
        CinemachineVirtualCamera[] cameras = FindObjectsOfType<CinemachineVirtualCamera>();
        if (cameras == null || cameras.Length == 0)
            return null;

        CinemachineVirtualCamera bestCamera = null;
        int bestPriority = int.MinValue;

        foreach (CinemachineVirtualCamera camera in cameras)
        {
            if (camera == null || !camera.isActiveAndEnabled)
                continue;

            if (bestCamera == null || camera.Priority > bestPriority)
            {
                bestCamera = camera;
                bestPriority = camera.Priority;
            }
        }

        return bestCamera != null ? bestCamera : cameras[0];
    }

    private void CaptureCurrentStateAsOriginal()
    {
        if (targetCamera == null) return;

        originalZoom = targetCamera.m_Lens.OrthographicSize;
        originalFollow = targetCamera.Follow;
        originalLookAt = targetCamera.LookAt;
        originalVirtualCamPosition = targetCamera.transform.position;

        if (framingTransposer != null)
        {
            originalOffset = framingTransposer.m_TrackedObjectOffset;
            originalCameraDistance = framingTransposer.m_CameraDistance;
        }
        else
        {
            originalOffset = Vector3.zero;
            originalCameraDistance = 0f;
        }
    }

    private Vector3 ResolveDetachedTargetPosition(Vector3 fallbackPosition)
    {
        Vector3 focusPosition = fallbackPosition;
        Transform playerRef = originalFollow != null ? originalFollow : cachedPlayer;

        if (focusMode == FocusMode.DetachToWorldTarget && worldFocusTarget != null)
        {
            focusPosition = worldFocusTarget.position;
        }
        else if (focusMode == FocusMode.DetachRelativeToPlayer && playerRef != null)
        {
            focusPosition = playerRef.position + playerRelativeFocusOffset;
        }

        if (overrideDetachedZ)
            focusPosition.z = detachedZ;

        return focusPosition;
    }

    public void SetWorldFocusTarget(Transform target)
    {
        worldFocusTarget = target;
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
