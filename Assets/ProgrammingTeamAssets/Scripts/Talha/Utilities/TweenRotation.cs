using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;


public class TweenRotation : MonoBehaviour
{
    public float Duration;
    public Vector3 StartRotation;
    public Vector3 TargetRotation;
    public LoopType LoopType = LoopType.Yoyo;

    private void Start()
    {
        transform.localEulerAngles = StartRotation;
        Tween();
    }

    private void Tween()
    {
        transform.DORotate(TargetRotation,Duration).SetLoops(-1, LoopType);
    }
}
