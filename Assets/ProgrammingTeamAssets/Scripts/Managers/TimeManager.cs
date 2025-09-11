using UnityEngine;
using System;

public enum TimeOfDay
{
    Morning,
    Noon,
    Afternoon,
    Night,
    AfterMidnight
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public TimeOfDay CurrentTime { get; private set; } = TimeOfDay.Morning;

    public event Action<TimeOfDay> OnTimeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ProgressTime()
    {
        CurrentTime = (TimeOfDay)(((int)CurrentTime + 1) % Enum.GetNames(typeof(TimeOfDay)).Length);
        Debug.Log("Time progressed to: " + CurrentTime);
        OnTimeChanged?.Invoke(CurrentTime);
    }
}