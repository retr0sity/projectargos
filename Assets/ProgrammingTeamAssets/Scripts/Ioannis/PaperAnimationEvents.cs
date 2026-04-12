using UnityEngine;
using Game.Utilities;

public class PaperAnimationEvents : MonoBehaviour
{
    public TextComponent DescriptionText;
    public GameObject DescriptionPanel;

    public void OnPaperOpened()
    {
        DescriptionText.gameObject.SetActive(true);
    }

    public void OnPaperClosed()
    {
        DescriptionText.gameObject.SetActive(false);
    }

    public void OnPaperFullyClosed()
    {
        DescriptionPanel.SetActive(false);
    }
}