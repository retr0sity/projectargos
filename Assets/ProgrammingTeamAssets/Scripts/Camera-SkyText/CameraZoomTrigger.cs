using UnityEngine;
using Cinemachine;
using System.Collections;

/// <summary>
/// Trigger-based camera zoom and position adjustment using Cinemachine.
/// Supports both Orthographic and Perspective cameras.
/// Attach to trigger collider zone. When player enters, camera adjusts.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CameraZoomTrigger : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera targetCamera;
    
    [Header("Camera Settings")]
    [Tooltip("For Orthographic: OrthographicSize. For Perspective: Field of View.")]
    [SerializeField] private float targetZoom = 40f;
    [SerializeField] private Vector3 cameraOffset = Vector3.zero;
    [Tooltip("Change camera distance from player. Set to 0 to only change FOV without moving camera.")]
    [SerializeField] private float targetCameraDistance = 0f;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private bool returnOnExit = true;
    
    [Header("Trigger Settings")]
    [SerializeField] private bool oneTimeUse = false;
    
    private float originalZoom;
    private Vector3 originalOffset;
    private float originalCameraDistance;
    private bool hasTriggered = false;
    private Coroutine currentTransition;
    private bool isPerspective;
    
    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
    
    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }
        
        if (targetCamera != null)
        {
            isPerspective = !targetCamera.m_Lens.Orthographic;
            
            if (isPerspective)
            {
                originalZoom = targetCamera.m_Lens.FieldOfView;
                Debug.Log($"Perspective camera detected. Original FOV: {originalZoom}");
            }
            else
            {
                originalZoom = targetCamera.m_Lens.OrthographicSize;
                Debug.Log($"Orthographic camera detected. Original Size: {originalZoom}");
            }
            
            var transposer = targetCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (transposer != null)
            {
                originalOffset = transposer.m_TrackedObjectOffset;
                originalCameraDistance = transposer.m_CameraDistance;
            }
        }
        else
        {
            Debug.LogError("CameraZoomTrigger: No CinemachineVirtualCamera found!");
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
        
        float distanceToUse = targetCameraDistance > 0 ? targetCameraDistance : originalCameraDistance;
        currentTransition = StartCoroutine(TransitionCamera(targetZoom, cameraOffset, distanceToUse));
    }
    
    public void ResetCameraSettings()
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);
            
        currentTransition = StartCoroutine(TransitionCamera(originalZoom, originalOffset, originalCameraDistance));
    }
    
    IEnumerator TransitionCamera(float toZoom, Vector3 toOffset, float toDistance)
    {
        if (targetCamera == null) yield break;
        
        float startZoom = isPerspective ? targetCamera.m_Lens.FieldOfView : targetCamera.m_Lens.OrthographicSize;
        Vector3 startOffset = Vector3.zero;
        float startDistance = originalCameraDistance;
        
        var transposer = targetCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            startOffset = transposer.m_TrackedObjectOffset;
            startDistance = transposer.m_CameraDistance;
        }
        
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / transitionDuration);
            
            float currentZoom = Mathf.Lerp(startZoom, toZoom, t);
            if (isPerspective)
                targetCamera.m_Lens.FieldOfView = currentZoom;
            else
                targetCamera.m_Lens.OrthographicSize = currentZoom;
            
            if (transposer != null)
            {
                transposer.m_TrackedObjectOffset = Vector3.Lerp(startOffset, toOffset, t);
                
                if (targetCameraDistance > 0)
                {
                    transposer.m_CameraDistance = Mathf.Lerp(startDistance, toDistance, t);
                }
            }
            
            yield return null;
        }
        
        if (isPerspective)
            targetCamera.m_Lens.FieldOfView = toZoom;
        else
            targetCamera.m_Lens.OrthographicSize = toZoom;
        
        if (transposer != null)
        {
            transposer.m_TrackedObjectOffset = toOffset;
            
            if (targetCameraDistance > 0)
            {
                transposer.m_CameraDistance = toDistance;
            }
        }
        
        currentTransition = null;
    }
    
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}