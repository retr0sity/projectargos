using Game.Utilities;
using System.ComponentModel;
using UnityEngine;

namespace Game.Components
{
    public class MainGameResultComponent : MonoBehaviour
    {
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

        public GameObject Container;
        public TextComponent AIOptionText;
        public TextComponent PlayerOptionText;

        private void Awake()
        {
            MainGameplayComponent.ActionTimerEnded += ShowResult;
        }

        private void ShowResult()
        {
            Container.SetActive(true);
            AIOptionText.SetupText(MainGameplayComponent.AIOption);
            PlayerOptionText.SetupText(MainGameplayComponent.PlayerOption);
        }
    }
}