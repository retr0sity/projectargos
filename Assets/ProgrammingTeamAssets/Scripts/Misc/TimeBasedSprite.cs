using UnityEngine;

public class TimeBasedSprite : MonoBehaviour
{
    [Header("Sprites for Time of Day")]
    public Sprite morningSprite;
    public Sprite noonSprite;
    public Sprite afternoonSprite;
    public Sprite nightSprite;
    public Sprite afterMidnightSprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        TimeManager.Instance.OnTimeChanged += UpdateSprite;
        UpdateSprite(TimeManager.Instance.CurrentTime); // set initial
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged -= UpdateSprite;
    }

    private void UpdateSprite(TimeOfDay time)
    {
        switch (time)
        {
            case TimeOfDay.Morning: spriteRenderer.sprite = morningSprite; break;
            case TimeOfDay.Noon: spriteRenderer.sprite = noonSprite; break;
            case TimeOfDay.Afternoon: spriteRenderer.sprite = afternoonSprite; break;
            case TimeOfDay.Night: spriteRenderer.sprite = nightSprite; break;
            case TimeOfDay.AfterMidnight: spriteRenderer.sprite = afterMidnightSprite; break;
        }
    }
}