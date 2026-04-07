using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace Game.Utilities
{
    public class OnEnableUIAnimation : MonoBehaviour
    {
        private Sequence Sequence;

        public List<TweenInfo> Tweens;

        private void Awake()
        {
            ResetTween();
        }

        [Button]
        public void ResetTween()
        {
            Sequence = DOTween.Sequence();
            for (int i = 0; i < Tweens.Count; i++)
            {
                Tweens[i].EndPosition = Tweens[i].RectTransform.localPosition;
                Sequence.Join(Tweens[i].RectTransform.DOLocalMove(Tweens[i].EndPosition + Tweens[i].TweenOffset, Tweens[i].TweenSpeed).
                    SetEase(Tweens[i].Ease).
                    SetDelay(Tweens[i].Delay));
            }
        }

        private void OnEnable()
        {
            Sequence?.Kill();
            Sequence.Play();
        }

        [System.Serializable]
        public class TweenInfo
        {
            public float Delay;
            public float TweenSpeed = 0.5f;
            public Vector3 TweenOffset;
            public RectTransform RectTransform;
            public Ease Ease = Ease.Linear;
            [HideInInspector] public Vector3 EndPosition;
        }
    }
}