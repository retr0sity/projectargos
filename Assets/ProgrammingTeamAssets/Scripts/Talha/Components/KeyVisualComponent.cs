using Game.Components;
using Game.Utilities;
using UnityEngine;

namespace Game.UI.Components
{
    public class KeyVisualComponent : MonoBehaviour
    {
        private MinigameComponent mMinigameComponent;
        private MinigameComponent MinigameComponent
        {
            get
            {
                if (mMinigameComponent == null)
                {
                    mMinigameComponent = FindObjectOfType<MinigameComponent>();
                }
                return mMinigameComponent;
            }
        }

        public TextComponent TextKeyCount;
        private void Start()
        {
            MinigameComponent.ActionUpdateKeys += UpdateKeyVisual;

        }

        private void UpdateKeyVisual(int KeyCount)
        {
            TextKeyCount.SetupText(KeyCount.ToString());
        }
    }
}