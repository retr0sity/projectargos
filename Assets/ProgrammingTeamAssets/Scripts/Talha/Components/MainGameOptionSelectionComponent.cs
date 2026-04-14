using Game.Extensions;
using Game.Managers;
using Game.Popups;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Components
{
    public class MainGameOptionSelectionComponent : OptionSelectionComponent
    {
        private PopupOkay mPopupOkay;
        private PopupOkay PopupOkay
        {
            get
            {
                if (mPopupOkay == null)
                {
                    mPopupOkay = DependencyManager.Instance.PopupManager.GetPopup<PopupOkay>();
                }
                return mPopupOkay;
            }
        }

        public Sprite Confirmation;

        protected override InputAction InputButtonLeft => InputManager.InputButtonUp;
        protected override InputAction InputButtonRight => InputManager.InputButtonDown;

        protected override void OptionSelected(InputAction.CallbackContext context)
        {
            InputButtonLeft.performed -= OnLeftPressed;
            InputButtonRight.performed -= OnRightPressed;
            InputManager.InputButtonEnter.performed -= OptionSelected;

            Container.gameObject.SetActive(false);

            PopupOkay.Show(Confirmation, () =>
            {
                AudioManager.Instance.Play("Console");
                MainGameplayComponent.ActionOptionSelected?.Invoke(Options[CurrentIndex].ID, Options.GetRandom().ID);
            });
        }
    }
}