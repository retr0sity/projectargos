using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class TweenColor : MonoBehaviour
{
    private SpriteRenderer mSpriteRenderer;
    private SpriteRenderer SpriteRenderer
    {
        get
        {
            if(mSpriteRenderer == null)
            {
                mSpriteRenderer = GetComponent<SpriteRenderer>();
            }
            return mSpriteRenderer;
        }
    }

    public float Duration;
    public Color StartColor;
    public Color EndColor;

    private void Awake()
    {
        SpriteRenderer.color = StartColor;
        SpriteRenderer.DOColor(EndColor,Duration).SetLoops(-1,LoopType.Yoyo);
    }
}
