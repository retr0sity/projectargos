using UnityEngine;
using Game.Managers;
using Game.Utilities;
using DG.Tweening;

namespace Game.Components
{
    public class MainGameStartTimer : MonoBehaviour
    {
        private TimerComponent TimerComponent => DependencyManager.Instance.TimerComponent;

        private MainGameplayComponent mMainGameplayComponent;
        private MainGameplayComponent MainGameplayComponent
        {
            get
            {
                if (mMainGameplayComponent == null)
                {
                    mMainGameplayComponent = GetComponentInParent<MainGameplayComponent>();
                }
                return mMainGameplayComponent;
            }
        }

        public float TimeLeft;
        public GameObject Container;
        public float TimeToStart = 3f;
        public float TimeToDisplayText;
        public TextComponent TextTimer;

        private void Awake()
        {
            MainGameplayComponent.ActionOptionSelected += StartTimer;
        }

        public void StartTimer(string PlayerOptionID,string AIOptionID)
        {
            Container.SetActive(true);
            TextTimer.SetupText(TimeLeft.ToString("0"));
            TimeToDisplayText = 1;
            TimeLeft = TimeToStart;
            TimerComponent.ActionTick += Tick;
        }

        public void Tick(float tickTime)
        {
            Debug.Log("Tick : "+TimeToDisplayText.ToString()+" : "+TimeLeft);
            if (TimeToDisplayText == 1f)
            {
                TextTimer.transform.DOKill();
                TextTimer.transform.localScale = Vector3.zero;
                TextTimer.transform.DOScale(Vector3.one, 0.5f);
                TextTimer.SetupText(TimeLeft.ToString("0"));
            }

            TimeLeft -= tickTime;
            TimeToDisplayText -= tickTime;

            if(TimeLeft <=0)
            {
                TimeLeft = 0;
                TimerComponent.ActionTick -= Tick;
                Container.SetActive(false);
                MainGameplayComponent.ActionTimerEnded?.Invoke();
            }
            if (TimeToDisplayText <= 0)
            {
                TimeToDisplayText = 1f;
            }
        }
    }
}
