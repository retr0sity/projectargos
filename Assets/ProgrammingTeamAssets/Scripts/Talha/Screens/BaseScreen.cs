using UnityEngine;
using Game.Managers;
using Game.Utilities;
using Game.Controllers;

namespace Game.Screens
{
    public abstract class BaseScreen : MonoBehaviour
    {
        protected ScreenManager ScreenManager => DependencyManager.Instance.ScreenManager;

        public virtual void Show(bool KeepLastScreenActive = false)
        {
            gameObject.GetComponent<RectTransform>().SetAsLastSibling();
            gameObject.SetActive(true);
            ScreenManager.SetActiveScreen(this, KeepLastScreenActive);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            ScreenManager.HideActiveScreen(this);
        }

    }
}