using UnityEngine;
using UnityEngine.UI;

namespace Game.Utilities
{
    public class ButtonComponent : MonoBehaviour
    {
        private Button mButton = null;
        public Button Button
        {
            get
            {
                if (mButton == null)
                {
                    mButton = GetComponent<Button>();
                    if(mButton == null)
                    {
                        mButton = gameObject.AddComponent<Button>();    
                    }
                }
                return mButton;
            }
        }

        public virtual void Start()
        {
            Button.onClick.AddListener(OnButtonClicked);
        }

        public virtual void OnButtonClicked()
        {
            
        }
    }
}