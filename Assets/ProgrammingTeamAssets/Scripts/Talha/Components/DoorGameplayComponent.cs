using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Game.Scriptables;

namespace Game.Components
{
    public class DoorGameplayComponent : MonoBehaviour
    {
        private MinigameComponent mMinigameComponent;
        private MinigameComponent MinigameComponent
        {
            get
            {
                if(mMinigameComponent == null)
                {
                    mMinigameComponent = GetComponentInParent<MinigameComponent>();
                }
                return mMinigameComponent;
            }
        }

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

        public StringData GameID;
        public Action ActionKeyCollected;
        public DoorComponent DoorComponent;

        private List<KeyComponent> mRequiredKeys;
        private List<KeyComponent> RequiredKeys
        {             get
            {
                if(mRequiredKeys == null)
                {
                    mRequiredKeys = GetComponentsInChildren<KeyComponent>().ToList();
                }
                return mRequiredKeys;
            }
        }
        public GameObject Container;

        private void Awake()
        {
            ActionKeyCollected += OnKeyCollected;
        }

        public void StartGame()
        {
            Container.SetActive(true);
            Debug.Log("Door game started for : "+GameID);
        }

        public void OnKeyCollected()
        {
            MinigameComponent.ActionUpdateKeys?.Invoke(RequiredKeys.Count(x => x.IsCollected));
            if (RequiredKeys.All(x=> x.IsCollected))
            {
                Debug.Log("Minigame Ended");
            }
            else
            {
                Container.SetActive(false);
                MinigameComponent.Container.SetActive(false);
                MainGameplayComponent.OptionSelection.StartGameplay();
            }
        }

        public (int KeysTotal,int KeysCollected) GetKeyCount()
        {
            return (RequiredKeys.Count(),RequiredKeys.Count(x=>x.IsCollected));
        }
    }
}