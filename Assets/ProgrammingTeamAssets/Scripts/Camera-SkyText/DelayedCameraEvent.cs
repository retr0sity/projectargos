using UnityEngine;
using TMPro;
using System.Collections;

public class DelayedCameraEvent : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraZoomTrigger cameraZoomTrigger;
    [SerializeField] private float cameraDelay = 0.5f;
    
    [Header("Sky Text (Optional)")]
    [SerializeField] private TextMeshPro skyText;   // <--- direct TMP reference
    [SerializeField] private string textMessage;
    [SerializeField] private float textDelay = 1.5f;
    
    public void TriggerSequence()
    {
        StartCoroutine(CameraTextSequence());
    }

    public void TriggerSequence(string customMessage)
    {
        textMessage = customMessage;
        StartCoroutine(CameraTextSequence());
    }

    private IEnumerator CameraTextSequence()
    {
        yield return new WaitForSeconds(cameraDelay);

        if (cameraZoomTrigger != null)
            cameraZoomTrigger.ApplyCameraSettings();

        yield return new WaitForSeconds(textDelay);

        if (skyText != null && !string.IsNullOrEmpty(textMessage))
            skyText.text = textMessage;   // directly set TMP text
    }
}