using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Components
{
    public class MinigameComponent : MonoBehaviour
    {
        private MainGameplayComponent mMainGameplayComponent;
        private MainGameplayComponent MainGameplayComponent
        {
                       get
            {
                if(mMainGameplayComponent == null)
                {
                    mMainGameplayComponent = FindObjectOfType<MainGameplayComponent>();
                }
                return mMainGameplayComponent;
            }
        }

        public Action<int> ActionUpdateKeys;
        public List<DoorGameplayComponent> DoorGames;
        public GameObject Container;

        private void Start()
        {
            MainGameplayComponent.ActionStartMinigame += StartMiniGame;
        }

        private void StartMiniGame(string GameID)
        {
            Container.gameObject.SetActive(true);
            DoorGames.FirstOrDefault(x => x.GameID.ID == GameID).StartGame();
        }

    }
}