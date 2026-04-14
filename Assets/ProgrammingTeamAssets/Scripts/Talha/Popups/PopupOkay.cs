using System;
using UnityEngine;
using Game.Managers;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Popups
{
    public class PopupOkay : BasePopup
    {
        private InputAction Button => DependencyManager.Instance.InputManager.InputButtonEnter;

        public Image Image;
        private Action ActionOnHide;
        public void Show(Sprite Skin,Action ActionOnHide)
        {
            this.ActionOnHide = ActionOnHide;
            Image.sprite = Skin;
            Image.SetNativeSize();
            Button.performed += HidePopup;
            base.Show();
        }

        private void HidePopup(InputAction.CallbackContext context)
        {
            Button.performed -= HidePopup;
            Hide();
            ActionOnHide?.Invoke();
        }

    }
}