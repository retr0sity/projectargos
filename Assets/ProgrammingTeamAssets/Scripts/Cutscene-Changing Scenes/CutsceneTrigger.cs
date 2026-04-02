using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using Core.Managers;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Cameras (Cinemachine Virtual Cameras)")]
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera cutsceneCamera;

    [Header("Timeline")]
    public PlayableDirector playableDirector;
    public float delayBeforeTimeline = 0f;

    [Header("Fade Transition (Animator)")]
    public Animator transition;
    public float fadeDuration = 1f;

    [Header("Player References (Optional but recommended)")]
    public Animator playerAnimator;
    public Rigidbody2D playerRigidbody;

    [Header("Teleport After Cutscene (Optional)")]
    public bool teleportPlayerAfterCutscene = false;
    public Transform teleportTarget;

    [Header("Audio (Optional)")]
    public AudioSource mainCameraAudioSource;
    [Range(0f, 1f)] public float cutsceneVolume = 0f;

    [Header("Options")]
    public bool playOnlyOnce = true;
    public bool disableTriggerAfterPlay = true;

    [Header("Cinemachine Priority")]
    public int playerPriority = 10;
    public int cutscenePriority = 20;

    private bool hasTriggered = false;
    private bool controlsLockedByCutscene = false;

    private float originalLinearDamping = 0f;
    private bool hasCachedDamping = false;

    private float originalVolume = 1f;
    private bool hasCachedVolume = false;

    private Coroutine cutsceneRoutine;

    private void OnEnable()
    {
        if (playableDirector != null)
            playableDirector.stopped += OnCutsceneFinished;
    }

    private void OnDisable()
    {
        if (playableDirector != null)
            playableDirector.stopped -= OnCutsceneFinished;

        // Safety cleanup
        RestorePlayerMovement();
        RestoreAudio();
        RestoreCameraPriority();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;
        if (playOnlyOnce && hasTriggered) return;

        hasTriggered = true;

        if (cutsceneRoutine != null)
            StopCoroutine(cutsceneRoutine);

        cutsceneRoutine = StartCoroutine(CutsceneSequence());

        if (disableTriggerAfterPlay)
            GetComponent<Collider2D>().enabled = false;
    }

    private IEnumerator CutsceneSequence()
    {
        // Optional: play sound effect
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager?.Play("dying2");

        DisablePlayerMovement();

        // Fade out before switching camera
        yield return StartCoroutine(FadeOut());

        SwitchToCutsceneCamera();
        LowerMainCameraAudio();

        // Fade back in so player sees cutscene
        yield return StartCoroutine(FadeIn());

        if (delayBeforeTimeline > 0f)
            yield return new WaitForSecondsRealtime(delayBeforeTimeline);

        if (playableDirector != null)
        {
            playableDirector.Play();
        }
        else
        {
            EndCutscene();
        }
    }

    private IEnumerator FadeOut()
    {
        if (transition == null) yield break;

        transition.updateMode = AnimatorUpdateMode.UnscaledTime;
        transition.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(fadeDuration);
        transition.updateMode = AnimatorUpdateMode.Normal;
    }

    private IEnumerator FadeIn()
    {
        if (transition == null) yield break;

        transition.updateMode = AnimatorUpdateMode.UnscaledTime;
        transition.SetTrigger("End");
        yield return new WaitForSecondsRealtime(fadeDuration);
        transition.updateMode = AnimatorUpdateMode.Normal;
    }

    private void SwitchToCutsceneCamera()
    {
        if (playerCamera != null)
            playerCamera.Priority = playerPriority;

        if (cutsceneCamera != null)
            cutsceneCamera.Priority = cutscenePriority;
    }

    private void RestoreCameraPriority()
    {
        if (playerCamera != null)
            playerCamera.Priority = cutscenePriority;

        if (cutsceneCamera != null)
            cutsceneCamera.Priority = playerPriority;
    }

    private void DisablePlayerMovement()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.SetControlLock(true);
            controlsLockedByCutscene = true;
        }

        BasePlayerController player = FindObjectOfType<BasePlayerController>();
        player?.ForceStop();

        if (playerRigidbody != null)
        {
            if (!hasCachedDamping)
            {
                originalLinearDamping = playerRigidbody.linearDamping;
                hasCachedDamping = true;
            }

            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.linearDamping = 5f;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsJumping", false);
        }
    }

    private void RestorePlayerMovement()
    {
        if (controlsLockedByCutscene && InputManager.Instance != null)
            InputManager.Instance.SetControlLock(false);

        controlsLockedByCutscene = false;

        if (playerRigidbody != null && hasCachedDamping)
            playerRigidbody.linearDamping = originalLinearDamping;
    }

    private void LowerMainCameraAudio()
    {
        if (mainCameraAudioSource == null) return;

        if (!hasCachedVolume)
        {
            originalVolume = mainCameraAudioSource.volume;
            hasCachedVolume = true;
        }

        mainCameraAudioSource.volume = cutsceneVolume;
    }

    private void RestoreAudio()
    {
        if (mainCameraAudioSource != null && hasCachedVolume)
            mainCameraAudioSource.volume = originalVolume;
    }

    private void OnCutsceneFinished(PlayableDirector director)
    {
        StartCoroutine(EndCutsceneRoutine());
    }

    private IEnumerator EndCutsceneRoutine()
    {
        // Fade out before returning control
        yield return StartCoroutine(FadeOut());

        if (teleportPlayerAfterCutscene && teleportTarget != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = teleportTarget.position;
        }

        EndCutscene();

        // Fade back in after returning to gameplay
        yield return StartCoroutine(FadeIn());
    }

    private void EndCutscene()
    {
        RestorePlayerMovement();
        RestoreAudio();
        RestoreCameraPriority();

        cutsceneRoutine = null;
    }
}