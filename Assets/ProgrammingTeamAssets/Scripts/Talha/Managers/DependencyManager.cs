using UnityEngine;
using Game.Components;

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

    }
}