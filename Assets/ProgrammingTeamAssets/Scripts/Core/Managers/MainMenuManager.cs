using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Image oImage;  // Reference to the image that updates

    public void ChangeImage(Sprite newSprite)
    {
        if (oImage != null)
            oImage.sprite = newSprite;
    }

    public void ClearImage()
    {
        if (oImage != null)
            oImage.sprite = null;
    }
}
