using UnityEngine;
using Game.Components;
using Game.Controllers;

namespace Game.Managers
{
    public class DependencyManager : MonoBehaviour
    {
        public static DependencyManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            Destroy(this);
        }

        #region Managers

        private InputManager mInputManager;
        public InputManager InputManager
        {
            get
            {
                if (mInputManager == null)
                {
                    mInputManager = FindObjectOfType<InputManager>(true);
                }
                return mInputManager;
            }
        }

        private PopupManager mPopupManager;
        public PopupManager PopupManager
        {
            get
            {
                if (mPopupManager == null)
                {
                    mPopupManager = FindObjectOfType<PopupManager>(true);
                }
                return mPopupManager;
            }
        }

        private ScreenManager mScreenManager;
        public ScreenManager ScreenManager
        {
            get
            {
                if (mScreenManager == null)
                {
                    mScreenManager = FindObjectOfType<ScreenManager>(true);
                }
                return mScreenManager;
            }
        }

        #endregion

        #region Components
        private UpdateComponent mUpdateComponent;
        public UpdateComponent UpdateComponent
        {
            get
            {
                if (mUpdateComponent == null)
                {
                    mUpdateComponent = FindObjectOfType<UpdateComponent>(true);
                }
                return mUpdateComponent;
            }
        }

        private TimerComponent mTimerComponent;
        public TimerComponent TimerComponent
        {
            get
            {
                if (mTimerComponent == null)
                {
                    mTimerComponent = FindObjectOfType<TimerComponent>(true);
                }
                return mTimerComponent;
            }
        }
        #endregion

        #region Controllers

        private InterfaceController mInterfaceController;
        public InterfaceController InterfaceController
        {
            get
            {
                if (mInterfaceController == null)
                {
                    mInterfaceController = FindObjectOfType<InterfaceController>(true);
                }
                return mInterfaceController;
            }
        }

        private ScreenFadeController mScreenFadeController;
        public ScreenFadeController ScreenFadeController
        {
            get
            {
                if (mScreenFadeController == null)
                {
                    mScreenFadeController = FindObjectOfType<ScreenFadeController>(true);
                }
                return mScreenFadeController;
            }
        }

        #endregion

    }
}