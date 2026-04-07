using System;
using UnityEngine;

namespace Game.Components
{
    public class MainGameplayComponent : MonoBehaviour
    {
        public Action ActionTimerEnded;
        public Action<string,string> ActionOptionSelected;

        public MainGameStartTimer MainGameStartTimer;
        public OptionSelectionComponent OptionSelection;

        public string AIOption;
        public string PlayerOption;

        public void Start()
        {
            ActionOptionSelected += OnOptionSelected;
            OptionSelection.StartGameplay();
        }

        private void OnOptionSelected(string PlayerOption,string AIOption)
        {
            this.AIOption = AIOption;
            this.PlayerOption = PlayerOption;
        }
    }
}