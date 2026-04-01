using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Managers;

/// <summary>
/// Controls the feather minigame scene.
/// Player presses Space to flutter up, auto-drifts right.
/// White feathers push left after 45 spawned, grey feathers push down after 75.
/// Touch ground = fade and restart. Reach the door = credits.
/// </summary>
public class FeatherGameManager : MonoBehaviour
{
    public static FeatherGameManager Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private GameObject playerFeather;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform doorTrigger;
    [SerializeField] private string creditsSceneName = "09_Credits";
    [SerializeField] private string transitionObjectName = "Crossfade";

    [Header("Feather Physics")]
    [SerializeField] private float flapForce = 5f;
    [SerializeField] private float autoMoveSpeed = 1.2f;
    [SerializeField] private float gravityScale = 0.8f;

    [Header("Obstacle Feathers")]
    [SerializeField] private GameObject whiteFeatherPrefab;
    [SerializeField] private GameObject greyFeatherPrefab;
    [SerializeField] private float spawnInterval = 0.6f;
    [SerializeField] private float spawnX = 12f;
    [SerializeField] private float spawnYMin = -1f;
    [SerializeField] private float spawnYMax = 4f;
    [SerializeField] private float whiteFeatherForce = 3f;
    [SerializeField] private float greyFeatherForce = 2.5f;

    [Header("Intro")]
    [SerializeField] private float darkenDuration = 1.5f;
    [SerializeField] private float introPauseDuration = 1f;
    [SerializeField] private AudioClip introSound;

    // Add these fields to FeatherGameManager
    private float _pushRecoveryTimer = 0f;
    private const float PushRecoveryTime = 0.3f;

    private Rigidbody2D _featherRb;
    private AudioSource _audioSource;
    private int _feathersSpawned = 0;
    private bool _gameActive = false;
    private bool _isDead = false;
    private SpriteRenderer _featherRenderer;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (playerFeather != null)
        {
            _featherRb = playerFeather.GetComponent<Rigidbody2D>();
            _featherRenderer = playerFeather.GetComponent<SpriteRenderer>();

            if (_featherRb != null)
            {
                _featherRb.gravityScale = 0f; // off during intro
                _featherRb.constraints  = RigidbodyConstraints2D.FreezeAll;
            }
        }

        _audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(IntroSequence());
    }

        void Update()
    {
        if (!_gameActive || _isDead) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            Flap();

        // Only override x velocity when not recently pushed
        if (_featherRb != null && _pushRecoveryTimer <= 0f)
        {
            Vector2 vel = _featherRb.linearVelocity;
            vel.x = autoMoveSpeed;
            _featherRb.linearVelocity = vel;
        }

        if (_pushRecoveryTimer > 0f)
            _pushRecoveryTimer -= Time.deltaTime;

        // Ground check
        if (groundCheck != null && playerFeather != null)
        {
            if (playerFeather.transform.position.y <= groundCheck.position.y)
                StartCoroutine(Die());
        }

        // Door check
        if (doorTrigger != null && playerFeather != null)
        {
            if (playerFeather.transform.position.x >= doorTrigger.position.x)
                StartCoroutine(ReachDoor());
        }
    }

    public void NotifyFeatherHit()
    {
        _pushRecoveryTimer = PushRecoveryTime;
    }

    private void Flap()
    {
        if (_featherRb == null) return;
        _featherRb.linearVelocity = new Vector2(_featherRb.linearVelocity.x, flapForce);
    }

    private IEnumerator IntroSequence()
    {
        // Lock input
        if (InputManager.Instance != null)
            InputManager.Instance.SetControlLock(true);

                // Hide all persistent UI
        if (BadEndingTimerUI.Instance != null)
            BadEndingTimerUI.Instance.gameObject.SetActive(false);

        if (QuestUI.Instance != null)
            QuestUI.Instance.gameObject.SetActive(false);

        // Darken screen
        yield return StartCoroutine(DarkenScreen(darkenDuration));

        // Play intro sound
        if (introSound != null && _audioSource != null)
            _audioSource.PlayOneShot(introSound);

        yield return new WaitForSeconds(introPauseDuration);

        // Brighten back
        yield return StartCoroutine(BrightenScreen(darkenDuration));

        // Unlock and start game
        if (InputManager.Instance != null)
            InputManager.Instance.SetControlLock(false);

        if (_featherRb != null)
        {
            _featherRb.constraints  = RigidbodyConstraints2D.FreezeRotation;
            _featherRb.gravityScale = gravityScale;
        }

        _gameActive = true;
        StartCoroutine(SpawnFeathers());
    }

    private IEnumerator SpawnFeathers()
    {
        while (_gameActive && !_isDead)
        {
            SpawnObstacleFeather();
            _feathersSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnObstacleFeather()
    {
        Debug.Log($"[Feathers] Spawned: {_feathersSpawned}, white force active: {_feathersSpawned >= 45}, grey active: {_feathersSpawned >= 75}");
        float spawnY = Random.Range(spawnYMin, spawnYMax);
        Vector3 spawnPos = new Vector3(
            playerFeather.transform.position.x + spawnX,
            spawnY,
            0f
        );

        // After 75: also spawn grey feathers
        if (_feathersSpawned >= 75 && greyFeatherPrefab != null)
        {
            GameObject grey = Instantiate(greyFeatherPrefab, spawnPos, Quaternion.identity);
            ObstacleFeather gof = grey.GetComponent<ObstacleFeather>();
            if (gof != null) gof.Init(Vector2.left * 2f + Vector2.down * greyFeatherForce);
        }

        // After 45: white feathers push left. Before 45: they just float harmlessly
        if (whiteFeatherPrefab != null)
        {
            GameObject white = Instantiate(whiteFeatherPrefab, spawnPos, Quaternion.identity);
            ObstacleFeather wof = white.GetComponent<ObstacleFeather>();
            if (wof != null)
            {
                Vector2 force = Vector2.left * whiteFeatherForce;
                wof.Init(force);
            }
        }
    }

    private IEnumerator Die()
    {
        if (_isDead) yield break;
        _isDead   = true;
        _gameActive = false;

        if (_featherRb != null)
            _featherRb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Crossfade then restart
        Animator transition = null;
        GameObject transitionGO = GameObject.Find(transitionObjectName);
        if (transitionGO != null)
            transition = transitionGO.GetComponentInChildren<Animator>();

        if (transition != null)
        {
            transition.updateMode = AnimatorUpdateMode.UnscaledTime;
            transition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(1f);
            transition.updateMode = AnimatorUpdateMode.Normal;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator ReachDoor()
    {
        if (_isDead) yield break;
        _isDead     = true;
        _gameActive = false;

        if (_featherRb != null)
            _featherRb.constraints = RigidbodyConstraints2D.FreezeAll;


        Animator transition = null;
        GameObject transitionGO = GameObject.Find(transitionObjectName);
        if (transitionGO != null)
            transition = transitionGO.GetComponentInChildren<Animator>();

        if (transition != null)
        {
            transition.updateMode = AnimatorUpdateMode.UnscaledTime;
            transition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(1f);
            transition.updateMode = AnimatorUpdateMode.Normal;
        }

        SceneManager.LoadScene(creditsSceneName);
    }

    // ── Screen darkening using a full screen overlay ───────────────────

    private GameObject _overlay;

    private IEnumerator DarkenScreen(float duration)
    {
        _overlay = CreateOverlay();
        CanvasGroup cg = _overlay.GetComponent<CanvasGroup>();
        float t = 0f;
        while (t < duration)
        {
            if (cg == null) yield break; // <-- add this
            cg.alpha = Mathf.Lerp(0f, 1f, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        if (cg != null) cg.alpha = 1f; // <-- and this
    }

    private IEnumerator BrightenScreen(float duration)
    {
        if (_overlay == null) yield break;
        CanvasGroup cg = _overlay.GetComponent<CanvasGroup>();
        if (cg == null) yield break; // <-- add this
        float t = 0f;
        while (t < duration)
        {
            if (cg == null) yield break; // <-- and this
            cg.alpha = Mathf.Lerp(1f, 0f, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        if (cg != null) cg.alpha = 0f;
        if (_overlay != null) Destroy(_overlay); // <-- guard this too
    }

    private GameObject CreateOverlay()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject cgo = new GameObject("OverlayCanvas");
            canvas = cgo.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 99;
            cgo.AddComponent<UnityEngine.UI.CanvasScaler>();
            cgo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        GameObject overlay = new GameObject("DarkOverlay", typeof(RectTransform));
        overlay.transform.SetParent(canvas.transform, false);

        RectTransform rt = overlay.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        UnityEngine.UI.Image img = overlay.AddComponent<UnityEngine.UI.Image>();
        img.color = Color.black;

        CanvasGroup cg = overlay.AddComponent<CanvasGroup>();
        cg.alpha            = 0f;
        cg.blocksRaycasts   = false;
        cg.interactable     = false;

        return overlay;
    }

    public void StopGame()
    {
        _gameActive = false;
    }
}