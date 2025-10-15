using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int buttonIndex;                  // Unique ID for this button (1, 2, 3, etc.)
    [SerializeField] private MainMenuManager mainMenuManager;  // Reference to the menu manager

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mainMenuManager != null)
            mainMenuManager.ChangeImage(buttonIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mainMenuManager != null)
            mainMenuManager.ClearImage();
    }
}