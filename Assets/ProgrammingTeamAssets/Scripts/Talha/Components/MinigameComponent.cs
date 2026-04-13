using Game.Controllers;
using Game.Managers;
using Game.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Components
{
    public class MinigameComponent : MonoBehaviour
    {
        private InterfaceController InterfaceController => DependencyManager.Instance.InterfaceController;

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

        private CameraFollowComponent mCameraFollowComponent;
        private CameraFollowComponent CameraFollowComponent
        {
            get
            {
                if(mCameraFollowComponent == null)
                {
                    mCameraFollowComponent = FindObjectOfType<CameraFollowComponent>();
                }
                return mCameraFollowComponent;
            }
        }

        public GameObject Container;
        public StringData MinigameInterface;
        public Action<int,int> ActionUpdateKeys;
        public List<DoorGameplayComponent> DoorGames;

        private void Start()
        {
            MainGameplayComponent.ActionStartMinigame += StartMiniGame;
        }

        private void StartMiniGame(string GameID)
        {
            Container.gameObject.SetActive(true);
            DoorGames.FirstOrDefault(x => x.GameID.ID == GameID).StartGame();
            InterfaceController.ActionSwitchInterface(MinigameInterface.ID);
            CameraFollowComponent.SetFollowStatus(true);
        }

        public void EndMiniGame()
        {
            Container.SetActive(false);
            DoorGames.ForEach(Item => Item.Container.SetActive(false));
            InterfaceController.ActionSwitchInterface("");
            CameraFollowComponent.SetFollowStatus(false);
        }

    }
}