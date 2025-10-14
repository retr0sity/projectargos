using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite hoverSprite;              // Sprite to show on hover
    [SerializeField] private MainMenuManager imageChanger;   // Reference to manager

    public void OnPointerEnter(PointerEventData eventData)
    {
        imageChanger.ChangeImage(hoverSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imageChanger.ClearImage();
    }
}
