using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Place this in any scene that should show mood notifications.
/// MoodManager calls ShowIfPresent() — if this exists in the scene, it shows the text.
/// </summary>
public class MoodFloatingText : MonoBehaviour
{
    private static MoodFloatingText _sceneInstance;

    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private float fadeInTime  = 0.3f;
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float fadeOutTime = 0.5f;

    Coroutine _current;

    void OnEnable()  { _sceneInstance = this; }
    void OnDisable() { if (_sceneInstance == this) _sceneInstance = null; }

    /// <summary>
    /// Called by MoodManager. Does nothing if no MoodFloatingText exists in the scene.
    /// </summary>
    public static void ShowIfPresent(string message)
    {
        if (_sceneInstance != null)
            _sceneInstance.Show(message);
    }

    void Show(string message)
    {
        if (_current != null) StopCoroutine(_current);
        _current = StartCoroutine(DisplayRoutine(message));
    }

    IEnumerator DisplayRoutine(string message)
    {
        textComponent.text = message;

        // Fade in
        float t = 0;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0, 1, t / fadeInTime));
            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        // Fade out
        t = 0;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(1, 0, t / fadeOutTime));
            yield return null;
        }

        SetAlpha(0);
        _current = null;
    }

    void SetAlpha(float a)
    {
        Color c = textComponent.color;
        c.a = a;
        textComponent.color = c;
    }
}
