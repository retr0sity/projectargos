using TMPro;
using UnityEngine;

namespace Game.Utilities
{
    public class TextComponent : MonoBehaviour
    {
        private TMP_Text mText = null;
        private TMP_Text Text
        {
            get
            {
                if (mText == null)
                {
                    mText = GetComponent<TMP_Text>();
                }
                return mText;
            }
        }

        public void SetupText(string Text)
        {
            this.Text.SetText(Text);
        }

        public void SetupColor(Color color)
        {
            Text.color = color;
        }

        public string GetText()
        {
            return Text.text;
        }
    }
}