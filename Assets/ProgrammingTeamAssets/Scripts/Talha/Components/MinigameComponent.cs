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

        private DoorGameplayComponent CurrentDoor;
        private void StartMiniGame(string GameID)
        {
            CurrentDoor = DoorGames.FirstOrDefault(x => x.GameID.ID == GameID);
            FindObjectOfType<PlayerKeyCollectorComonent>(true).transform.localPosition = CurrentDoor.StartPosition;
            Container.gameObject.SetActive(true);
            CurrentDoor.StartGame();
            InterfaceController.ActionSwitchInterface(MinigameInterface.ID);
            CameraFollowComponent.SetFollowStatus(true);
        }

        public void EndMiniGame()
        {
            Container.SetActive(false);
            CurrentDoor.StartPosition = FindObjectOfType<PlayerKeyCollectorComonent>(true).transform.localPosition;
            DoorGames.ForEach(Item => Item.Container.SetActive(false));
            InterfaceController.ActionSwitchInterface("");
            CameraFollowComponent.SetFollowStatus(false);
        }

    }
}