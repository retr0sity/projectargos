using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeController : MonoBehaviour
{
    public Image Fade;

    public void FadeInAndOut(Action OnStepCompleted = null,Action OnCompleted = null)
    {
        Fade.DOFade(1, 1.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            OnStepCompleted?.Invoke();
            Fade.DOFade(0, 1.5f).SetEase(Ease.Linear).SetDelay(0.5f).OnComplete(() =>
            {
                OnCompleted?.Invoke();
            });
        });
    }
}
