using UnityEngine;
using Game.Components;

public class DeskItemComponent : MonoBehaviour
{
    private OptionSelectionComponent mOptionSelection;
    private OptionSelectionComponent OptionSelection
    {
        get
        {
            if (mOptionSelection == null)
                mOptionSelection = GetComponentInParent<OptionSelectionComponent>(true);
            return mOptionSelection;
        }
    }

    public DeskItemData Data;
    public GameObject NormalSprite;
    public GameObject HighlightSprite;

    private void Awake()
    {
        OptionSelection.ActionOptionChanged += OnOptionChanged;
        HighlightSprite.SetActive(false);
    }

    private void OnOptionChanged(string ID)
    {
        NormalSprite.SetActive(ID != Data.ID);
        HighlightSprite.SetActive(ID == Data.ID);
    }
}