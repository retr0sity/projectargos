using UnityEngine;
using Game.Managers;

namespace Game.Popups
{
    public abstract class BasePopup : MonoBehaviour
    {
        protected PopupManager PopupManager => DependencyManager.Instance.PopupManager;

        public virtual void Show(bool KeepLastPopupActive = false)
        {
            gameObject.GetComponent<RectTransform>().SetAsLastSibling();
            gameObject.SetActive(true);
            PopupManager.SetActivePopup(this, KeepLastPopupActive);

        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            PopupManager.HideActivePopup(this);

        }

    }
}