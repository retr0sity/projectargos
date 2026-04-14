using System;
using Game.Popups;
using UnityEngine;
using System.Linq;
using Game.Extensions;
using System.Collections.Generic;

namespace Game.Managers
{
    public class PopupManager : MonoBehaviour
    {
        private Stack<BasePopup> CachePopups = new Stack<BasePopup>();

        public GameObject Container;
        private List<BasePopup> mPopups;
        private List<BasePopup> Popups
        {
            get
            {
                if(mPopups.IsNullOrEmpty())
                {
                    mPopups = GetComponentsInChildren<BasePopup>(true).ToList();
                }
                return mPopups;
            }
        }

        public TPopup GetPopup<TPopup>()
        where TPopup : BasePopup
        {
            Type RequestType = typeof(TPopup);
            var Popup = (TPopup)Popups.FirstOrDefault(x => x.GetType() == RequestType);
            return Popup;
        }

        public void HideActivePopup(BasePopup Popup)
        {
            CachePopups.Pop();
            if (CachePopups.Count > 0)
            {
                var LastPopup = CachePopups.Peek();
                LastPopup.gameObject.SetActive(true);
            }
            ShowCachePopup();
        }

        public void SetActivePopup(BasePopup Popup, bool KeepLastPopupActive)
        {
            if (CachePopups.Count > 0)
            {
                var LastPopup = CachePopups.Peek();
                LastPopup.gameObject.SetActive(KeepLastPopupActive);
            }
            CachePopups.Push(Popup);
            ShowCachePopup();
        }

        //public List<BasePopup> CacheList = new List<BasePopup>();
        private void ShowCachePopup()
        {
            //CacheList = CachePopups.ToList();
        }
    }
}