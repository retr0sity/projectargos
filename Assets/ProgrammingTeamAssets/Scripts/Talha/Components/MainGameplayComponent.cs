using System;
using UnityEngine;

namespace Game.Components
{
    public class MainGameplayComponent : MonoBehaviour
    {
        public Action ActionTimerEnded;
        public Action<string,string> ActionShowResult;
        public Action<string,string> ActionOptionSelected;
        public Action<string> ActionStartMinigame;
        public Action<string> ActionStartParagraphGame;

        public MainGameStartTimer MainGameStartTimer;
        public OptionSelectionComponent OptionSelection;
        public GameAnimationComponent GameAnimation;
        public MinigameComponent MinigameComponent;
        public ParagraphGameComponent ParagraphGameComponent;

        public string AIOption;
        public string PlayerOption;

        public void Start()
        {
            ActionOptionSelected += OnOptionSelected;
            ActionTimerEnded += ShowAnimation;
            OptionSelection.StartGameplay();
        }

        private void OnOptionSelected(string PlayerOption,string AIOption)
        {
            this.AIOption = AIOption;
            this.PlayerOption = PlayerOption;
        }

        private void ShowAnimation()
        {
            GameAnimation.Animate(AIOption,ConcludeResult);
        }

        private void ConcludeResult()
        {
            ActionShowResult?.Invoke(PlayerOption, AIOption);
        }
    }
}