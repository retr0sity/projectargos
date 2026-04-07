using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Managers
{
    public class InputManager : MonoBehaviour
    {
        public PlayerInputs InputActions;
        public InputAction InputButtonLeft => InputActions.Gameplay.ButtonLeft;
        public InputAction InputButtonRight => InputActions.Gameplay.ButtonRight;
        public InputAction InputButtonEnter => InputActions.Gameplay.ButtonEnter;

        private void Awake()
        {
            InputActions = new PlayerInputs();
        }

        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }

    }
}