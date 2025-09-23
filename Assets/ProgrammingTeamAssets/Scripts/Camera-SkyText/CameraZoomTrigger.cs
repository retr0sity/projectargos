using UnityEngine;
using Cinemachine;
using System.Collections;

/// <summary>
/// Trigger-based camera zoom and position adjustment using Cinemachine.
/// Attach to trigger collider zone. When player enters, camera adjusts.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CameraZoomTrigger : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera targetCamera;
    
    [Header("Camera Settings")]
    [SerializeField] private float targetZoom = 8f; // Orthographic size
    [SerializeField] private Vector3 cameraOffset = Vector3.zero; // Position offset
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private bool returnOnExit = true;
    
    [Header("Trigger Settings")]
    [SerializeField] private bool oneTimeUse = false;
    
    private float originalZoom;
    private Vector3 originalOffset;
    private bool hasTriggered = false;
    private Coroutine currentTransition;
    
    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
    
    void Start()
    {
        if (targetCamera == null)
        {
            // Try to find main virtual camera
            targetCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }
        
        if (targetCamera != null)
        {
            originalZoom = targetCamera.m_Lens.OrthographicSize;
            
            // Store original follow offset if using Framing Transposer
            var transposer = targetCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (transposer != null)
            {
                originalOffset = transposer.m_TrackedObjectOffset;
            }
        }
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
        if (currentTransition != null)
            StopCoroutine(currentTransition);
            
        currentTransition = StartCoroutine(TransitionCamera(targetZoom, cameraOffset));
    }
    
    public void ResetCameraSettings()
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);
            
        currentTransition = StartCoroutine(TransitionCamera(originalZoom, originalOffset));
    }
    
    IEnumerator TransitionCamera(float toZoom, Vector3 toOffset)
    {
        if (targetCamera == null) yield break;
        
        float startZoom = targetCamera.m_Lens.OrthographicSize;
        Vector3 startOffset = Vector3.zero;
        
        var transposer = targetCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            startOffset = transposer.m_TrackedObjectOffset;
        }
        
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            
            // Smooth easing
            t = Mathf.SmoothStep(0, 1, t);
            
            // Apply zoom
            targetCamera.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, toZoom, t);
            
            // Apply offset
            if (transposer != null)
            {
                transposer.m_TrackedObjectOffset = Vector3.Lerp(startOffset, toOffset, t);
            }
            
            yield return null;
        }
        
        // Ensure final values
        targetCamera.m_Lens.OrthographicSize = toZoom;
        if (transposer != null)
        {
            transposer.m_TrackedObjectOffset = toOffset;
        }
        
        currentTransition = null;
    }
    
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}