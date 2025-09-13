using UnityEngine;

public class TimeButton : MonoBehaviour
{
    // Called by the Button's OnClick event in the Inspector
    public void OnTimeButtonClicked()
    {
        TimeManager.Instance.ProgressTime();
    }
}