using System;
using UnityEngine;
using Game.Managers;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Game.Scriptables;
using System.ComponentModel;
using Game.Extensions;

namespace Game.Components
{
    public class OptionSelectionComponent : MonoBehaviour
    {
        private InputManager InputManager => DependencyManager.Instance.InputManager;

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

        private int CurrentIndex;
        public List<StringData> Options;
        public GameObject Container;
        public Action<string> ActionOptionChanged;

        public void StartGameplay()
        {
            Container.gameObject.SetActive(true);
            InputManager.InputButtonLeft.performed += OnLeftPressed;
            InputManager.InputButtonRight.performed += OnRightPressed;
            InputManager.InputButtonEnter.performed += OptionSelected;
            CurrentIndex = 0;
            ActionOptionChanged?.Invoke(Options[CurrentIndex].ID);
        }

        private void OnLeftPressed(InputAction.CallbackContext context)
        {
            if (CurrentIndex == 0)
                return;

            CurrentIndex--;
            ActionOptionChanged?.Invoke(Options[CurrentIndex].ID);
        }

        private void OnRightPressed(InputAction.CallbackContext context)
        {
            if (CurrentIndex == Options.Count - 1)
                return;

            CurrentIndex++;
            ActionOptionChanged?.Invoke(Options[CurrentIndex].ID);
        }

        private void OptionSelected(InputAction.CallbackContext context)
        {
            InputManager.InputButtonLeft.performed -= OnLeftPressed;
            InputManager.InputButtonRight.performed -= OnRightPressed;
            InputManager.InputButtonEnter.performed -= OptionSelected;

            Container.gameObject.SetActive(false);
            MainGameplayComponent.ActionOptionSelected?.Invoke(Options[CurrentIndex].ID,Options.GetRandom().ID);
        }
    }

}