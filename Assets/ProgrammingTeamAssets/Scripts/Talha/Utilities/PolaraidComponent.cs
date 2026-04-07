using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PolaraidComponent : MonoBehaviour
{
    public float StartingScale; 
    public float EndingScale; 
    public float ScaleDuration;
    public float ScaleDelay;

    public float StartingRotation; 
    public float EndingRotation;
    public float RotationDuration;
    public float RotationDelay;

    public Ease EaseType = Ease.Linear;

    private void OnEnable()
    {
        transform.DOKill();

        transform.localScale = Vector3.one * StartingScale;
        transform.localEulerAngles = new Vector3(0,0, StartingRotation);
        transform.DOScale(EndingScale * Vector3.one, ScaleDuration).SetEase(EaseType).SetLoops(-1, LoopType.Yoyo).SetDelay(ScaleDelay);
        transform.DOLocalRotate(new Vector3(0,0,EndingRotation),RotationDuration).SetEase(EaseType).SetLoops(-1,LoopType.Yoyo).SetDelay(RotationDelay);
    }
}
