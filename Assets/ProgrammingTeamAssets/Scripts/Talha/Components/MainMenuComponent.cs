using DG.Tweening;
using Game.Controllers;
using Game.Extensions;
using Game.Managers;
using Game.Popups;
using Game.Scriptables;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Components
{
    public class MainMenuComponent : OptionSelectionComponent
    {
        private InterfaceController InterfaceController => DependencyManager.Instance.InterfaceController;
        public CanvasGroup CanvasGroup;

        public Image ImagePage;
        public List<Sprite> PageSprites;
        public StringData DefaultOption;
        public StringData DefaultScreen;

        private PopupOkay mPopupOkay;
        private PopupOkay PopupOkay
        {
            get
            {
                if(mPopupOkay == null)
                {
                    mPopupOkay = DependencyManager.Instance.PopupManager.GetPopup<PopupOkay>();
                }
                return mPopupOkay;
            }
        }

        public Sprite SpriteBasicControls;
        protected override InputAction InputButtonLeft => InputManager.InputButtonUp;
        protected override InputAction InputButtonRight => InputManager.InputButtonDown;

        private void Start()
        {
            InterfaceController.ActionSwitchInterface?.Invoke(DefaultScreen.ID);
            StartCoroutine(PageRoutine());
            //this.RunAfter(1f, () =>
            //{
            ActionOptionChanged?.Invoke(DefaultOption.ID);
            //});
        }

        IEnumerator PageRoutine()
        {
            for (int i = 0; i < PageSprites.Count; i++)
            {
                ImagePage.sprite = PageSprites[i];
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.5f);
            CanvasGroup.DOFade(1, 2f).OnComplete(() =>
            {
                StartGameplay();
            });
        }

        protected override void OptionSelected(InputAction.CallbackContext context)
        {
            InterfaceController.ActionSwitchInterface?.Invoke("");
            InputButtonLeft.performed -= OnLeftPressed;
            InputButtonRight.performed -= OnRightPressed;
            InputManager.InputButtonEnter.performed -= OptionSelected;
            Container.gameObject.SetActive(false);
            AudioManager.Instance.Play("Console");

            PopupOkay.Show(SpriteBasicControls, () =>
            {
                FindObjectOfType<MainGameplayComponent>().ActionStartMainGameplay?.Invoke()
                ;
            });
        }
    }
}