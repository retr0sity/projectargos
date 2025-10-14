using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Image previewImage;

    public void ChangeImage(Sprite newSprite)
    {
        if (previewImage != null && newSprite != null)
        {
            previewImage.overrideSprite = newSprite;  // force UI redraw
        }
    }

    public void ClearImage()
    {
        if (previewImage != null)
            previewImage.overrideSprite = null;
    }
}
