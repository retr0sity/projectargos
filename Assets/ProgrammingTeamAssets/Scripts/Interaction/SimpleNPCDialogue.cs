using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class SimpleNPCDialogue : MonoBehaviour
{
    [Header("Activation Mode")]
    [SerializeField] private bool useTriggerMode = false;

    [Header("Dialogue")]
    [SerializeField] private string speakerName = "NPC";
    [TextArea(3, 5)]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private bool oneTimeOnly = true;

    [Header("Persistent ID")]
    [Tooltip("Unique ID for this dialogue. Auto-generated if left empty.")]
    [SerializeField] private string triggerID = "";

    [Header("Conversation (multi-speaker)")]
    [Tooltip("Enable to use per-line speakers. Overrides Dialogue lines above.")]
    [SerializeField] private bool useConversationMode = false;
    [SerializeField] private DialogueLine[] conversationLines;

    [Header("Animation")]
    [Tooltip("Animator on the NPC sprite. Assign if you want the animation to freeze on interact.")]
    [SerializeField] private Animator npcAnimator;
    [SerializeField] private bool freezeAnimationOnInteract = false;
    [Tooltip("Which frame to freeze on (0 = first frame, 1 = last frame).")]
    [SerializeField] [Range(0f, 1f)] private float idleNormalizedTime = 0f;

    [Header("Enemy Freeze")]
    [SerializeField] private bool freezeEnemies = true;
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueComplete;

    [Header("Bad Ending Timer")]
    [Tooltip("Start the bad ending countdown when this dialogue completes.")]
    [SerializeField] private bool startsBadEndingTimer = false;
    [Tooltip("Timer duration in minutes.")]
    [SerializeField] private float badEndingMinutes = 5f;

    private bool isCurrentlyInDialogue = false;
    private Fascist[] frozenEnemies;
    private Coroutine freezeAnimCoroutine;

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            if (useTriggerMode)
            {
                col.isTrigger = true;
                if (gameObject.tag == "Interactable")
                    gameObject.tag = "Untagged";
            }
            else
            {
                col.isTrigger = false;
                if (gameObject.tag == "Untagged")
                    gameObject.tag = "Interactable";
            }
        }
        else
        {
            Debug.LogError($"SimpleNPCDialogue on {gameObject.name} is missing a Collider2D component!");
        }
    }

    void Start()
    {
        if (string.IsNullOrEmpty(triggerID))
            triggerID = $"{gameObject.scene.name}_{gameObject.name}";

        // If already fired, hide immediately on scene reload
        if (oneTimeOnly && useTriggerMode && HasBeenTalkedTo())
            gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTriggerMode) return;
        if (!other.CompareTag("Player")) return;
        StartDialogue();
    }

    public void OnInteract()
    {
        if (useTriggerMode) return;
        StartDialogue();
    }

    private bool HasBeenTalkedTo()
    {
        return GameStateManager.Instance != null && GameStateManager.Instance.HasTriggerFired(triggerID);
    }

    void StartDialogue()
    {
        if (isCurrentlyInDialogue)
        {
            Debug.Log($"{gameObject.name}: Already in dialogue, ignoring trigger");
            return;
        }

        if (oneTimeOnly && HasBeenTalkedTo())
        {
            Debug.Log($"{gameObject.name}: Already talked to, ignoring");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }

        bool hasLines = (dialogueLines != null && dialogueLines.Length > 0);
        bool hasConversation = (useConversationMode && conversationLines != null && conversationLines.Length > 0);
        if (!hasLines && !hasConversation)
        {
            Debug.LogWarning($"No dialogue or conversation lines set for {gameObject.name}");
            return;
        }

        if (oneTimeOnly && GameStateManager.Instance != null)
            GameStateManager.Instance.MarkTriggerFired(triggerID);

        isCurrentlyInDialogue = true;

        if (freezeEnemies)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            frozenEnemies = new Fascist[enemies.Length];
            for (int i = 0; i < enemies.Length; i++)
            {
                Fascist enemy = enemies[i].GetComponent<Fascist>();
                if (enemy != null)
                {
                    frozenEnemies[i] = enemy;
                    enemy.TriggerStop(enemy.transform.position);
                }
            }
        }

        Debug.Log($"{gameObject.name}: Starting dialogue");

        if (freezeAnimationOnInteract && npcAnimator != null)
        {
            if (freezeAnimCoroutine != null) StopCoroutine(freezeAnimCoroutine);
            freezeAnimCoroutine = StartCoroutine(FreezeAnimationAfterLoop());
        }

        if (useConversationMode && conversationLines != null && conversationLines.Length > 0)
            DialogueManager.Instance.StartConversation(conversationLines, OnDialogueFinished);
        else
            DialogueManager.Instance.StartDialogue(dialogueLines, speakerName, OnDialogueFinished);
    }

    IEnumerator FreezeAnimationAfterLoop()
    {
        float targetTime = Mathf.Floor(npcAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) + 1f;
        while (npcAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < targetTime)
            yield return null;

        AnimatorStateInfo state = npcAnimator.GetCurrentAnimatorStateInfo(0);
        npcAnimator.Play(state.fullPathHash, 0, idleNormalizedTime);
        yield return null;
        npcAnimator.speed = 0f;
        freezeAnimCoroutine = null;
    }

    void OnDialogueFinished()
    {
        Debug.Log($"{gameObject.name}: Dialogue finished");

        if (freezeAnimationOnInteract && npcAnimator != null)
        {
            if (freezeAnimCoroutine != null) { StopCoroutine(freezeAnimCoroutine); freezeAnimCoroutine = null; }
            npcAnimator.speed = 1f;
        }

        isCurrentlyInDialogue = false;

        if (freezeEnemies && frozenEnemies != null)
        {
            foreach (Fascist enemy in frozenEnemies)
                if (enemy != null) enemy.Resume();
            frozenEnemies = null;
        }

        if (startsBadEndingTimer && GameStateManager.Instance != null)
        {
            GameStateManager.Instance.StartBadEndingTimer(badEndingMinutes * 60f);
            QuestManager.Instance?.StartQuest1();
        }

        onDialogueComplete?.Invoke();

        if (oneTimeOnly)
        {
            if (useTriggerMode)
                gameObject.SetActive(false);
            else
                gameObject.tag = "Untagged";
        }
    }

    public void TriggerNow()
    {
        StartDialogue();
    }

    public void ResetDialogue()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.ResetTrigger(triggerID);

        isCurrentlyInDialogue = false;

        if (useTriggerMode)
            gameObject.SetActive(true);
        else
            gameObject.tag = "Interactable";
    }

    public void SetTriggerMode(bool enableTrigger)
    {
        useTriggerMode = enableTrigger;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = useTriggerMode;
            gameObject.tag = useTriggerMode ? "Untagged" : "Interactable";
        }
    }
}