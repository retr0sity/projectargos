using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Game.Extensions;
using Game.Screens;
using Game.Popups;

namespace Game.Managers
{
    public class ScreenManager : MonoBehaviour
    {
        private Stack<BaseScreen> CacheScreens = new Stack<BaseScreen>();

        private List<BaseScreen> mScreens;
        public List<BaseScreen> Screens
        {
            get
            {
                if(mScreens == null)
                {
                    mScreens = GetComponentsInChildren<BaseScreen>(true).ToList();
                }
                return mScreens;
            }
        }

        public GameObject Container;

        public TScreen GetScreen<TScreen>()
        where TScreen : BaseScreen
        {
            Type RequestType = typeof(TScreen);
            var Screen = (TScreen)Screens.FirstOrDefault(x => x.GetType() == RequestType);
            return Screen;
        }

        BaseScreen GenerateInstance(BaseScreen Refrence)
        {
            var GO = Instantiate(Refrence);
            GO.transform.SetParent(Container.transform);
            GO.transform.ResetLocalTransform();
            GO.gameObject.SetActive(false);
            Screens.Add(GO);
            return GO;
        }

        public void HideActiveScreen(BaseScreen Screen)
        {
            CacheScreens?.Pop();
            if (CacheScreens.Count > 0)
            {
                var LastScreen = CacheScreens.Peek();
                LastScreen.gameObject.SetActive(true);
            }
            ShowCacheScrees();
        }

        public void SetActiveScreen(BaseScreen Screen, bool KeepLastScreenActive)
        {
            if (CacheScreens.Count > 0)
            {
                var LastScreen = CacheScreens.Peek();
                LastScreen.gameObject.SetActive(KeepLastScreenActive);
            }
            CacheScreens.Push(Screen);
            ShowCacheScrees();
        }

        public void HideAllScreens()
        {
            while(CacheScreens.Count>0)
            {
                CacheScreens.Peek().gameObject.SetActive(false);
                CacheScreens.Pop();
            }
            ShowCacheScrees();
        }

        private List<BaseScreen> CacheList = new List<BaseScreen>();
        private void ShowCacheScrees()
        {
            CacheList = CacheScreens.ToList();
        }

    }
}