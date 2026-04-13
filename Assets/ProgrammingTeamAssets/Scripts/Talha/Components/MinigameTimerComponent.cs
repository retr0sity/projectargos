using Game.Managers;
using Game.Utilities;
using UnityEngine;

namespace Game.Components
{
    public class MinigameTimerComponent : MonoBehaviour
    {
        private TimerComponent TimerComponent => DependencyManager.Instance.TimerComponent;

        MainGameplayComponent mMainGameplayComponent;
        MainGameplayComponent MainGameplayComponent
        {
            get
            {
                if(mMainGameplayComponent == null)
                {
                    mMainGameplayComponent = FindObjectOfType<MainGameplayComponent>();
                }
                return mMainGameplayComponent;
            }

        }

        MinigameComponent mMinigameComponent;
        MinigameComponent MinigameComponent
        {
            get
            {
                if (mMinigameComponent == null)
                {
                    mMinigameComponent = FindObjectOfType<MinigameComponent>();
                }
                return mMinigameComponent;
            }

        }

        public float TimeLimit;
        public float TimeElapsed;
        public TextComponent TextTimer;

        private bool IsSubscribed;
        private void Start()
        {
            MainGameplayComponent.ActionStartMinigame += StartTimer;
            StartTimer("");
        }

        private void StartTimer(string MinigameID)
        {
            TextTimer.SetupText((TimeLimit - TimeElapsed).ToString("00:00"));
            TimeElapsed = 0;
            TimerComponent.ActionTick += Tick;
            IsSubscribed = true;
        }

        private void Tick(float TickTime)
        {
            TimeElapsed += TickTime;
            TextTimer.SetupText((TimeLimit - TimeElapsed).ToString("00:00"));
            if(TimeElapsed >=TimeLimit)
            {
                TimerComponent.ActionTick -= Tick;
                IsSubscribed = false;
                MinigameComponent.EndMiniGame();
                MainGameplayComponent.ActionStartMainGameplay();
            }
        }

        private void OnDisable()
        {
            if(IsSubscribed)
            {
                IsSubscribed = false;
                TimerComponent.ActionTick -= Tick;
            }
        }
    }
}