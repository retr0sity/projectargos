using UnityEngine;

namespace Game.Components
{
    public class ParagraphGameComponent : MonoBehaviour
    {
        private MainGameplayComponent mMainGameplayComponent;
        private MainGameplayComponent MainGameplayComponent
        {
            get
            {
                if(mMainGameplayComponent == null)
                {
                    mMainGameplayComponent = GetComponentInParent<MainGameplayComponent>();
                }
                return mMainGameplayComponent;
            }
        }

        public GameObject Container;

        private void Awake()
        {
            MainGameplayComponent.ActionStartParagraphGame += StartGame;
        }

        private void StartGame(string ID)
        {
            Container.SetActive(true);
        }
    }
}