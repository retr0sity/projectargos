using DG.Tweening;
using Game.Scriptables;
using System.Globalization;
using UnityEngine;

namespace Game.Components
{
    public class GameOptionComponent : MonoBehaviour
    {
        private OptionSelectionComponent mMainGameplay;
        private OptionSelectionComponent MainGameplay
        {
            get
            {
                if(mMainGameplay == null)
                {
                    mMainGameplay = FindObjectOfType<OptionSelectionComponent>(true);
                }
                return mMainGameplay;
            }
        }

        public StringData Option;

        private void Awake()
        {
            MainGameplay.ActionOptionChanged += OnOptionChanged;
        }

        public virtual void OnOptionChanged(string ID)
        {
            var TargetScale = ID == this.Option.ID ? Vector3.one * 1.2f : Vector3.one;
            transform.DOScale(TargetScale, 0.2f).SetEase(Ease.OutBack);
        }
    }
}