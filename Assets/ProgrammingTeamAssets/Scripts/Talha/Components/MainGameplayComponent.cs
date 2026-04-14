using System;
using System.Collections.Generic;
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
        public Action<List<string>> ActionStartHangmanGame;
        public Action ActionStartMainGameplay;

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
            ActionStartMainGameplay += StartMainGameplay;
            //StartMainGameplay();
        }

        private void StartMainGameplay()
        {
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