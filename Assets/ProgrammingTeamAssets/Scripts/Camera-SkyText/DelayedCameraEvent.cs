using UnityEngine;
using TMPro;
using System.Collections;

public class DelayedCameraEvent : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraZoomTrigger cameraZoomTrigger;
    [SerializeField] private float cameraDelay = 0.5f;
    [SerializeField] private bool resetCameraAfterText = false;
    [SerializeField] private float resetDelay = 1.5f;
    
    [Header("Sky Text (Optional)")]
    [SerializeField] private TextMeshPro skyText;   // <--- direct TMP reference
    [SerializeField] private string textMessage;
    [SerializeField] private float textDelay = 1.5f;
    private Coroutine sequenceRoutine;
    
    public void TriggerSequence()
    {
        StartSequence();
    }

    public void TriggerSequence(string customMessage)
    {
        textMessage = customMessage;
        StartSequence();
    }

    private void OnDisable()
    {
        if (sequenceRoutine != null)
        {
            StopCoroutine(sequenceRoutine);
            sequenceRoutine = null;
        }
    }

    private void StartSequence()
    {
        if (sequenceRoutine != null)
            StopCoroutine(sequenceRoutine);

        sequenceRoutine = StartCoroutine(CameraTextSequence());
    }

    private IEnumerator CameraTextSequence()
    {
        yield return new WaitForSeconds(cameraDelay);

        if (cameraZoomTrigger != null)
            cameraZoomTrigger.ApplyCameraSettings();

        yield return new WaitForSeconds(textDelay);

        if (skyText != null && !string.IsNullOrEmpty(textMessage))
            skyText.text = textMessage;   // directly set TMP text

        if (resetCameraAfterText && cameraZoomTrigger != null)
        {
            yield return new WaitForSeconds(resetDelay);
            cameraZoomTrigger.ResetCameraSettings();
        }

        sequenceRoutine = null;
    }
}
