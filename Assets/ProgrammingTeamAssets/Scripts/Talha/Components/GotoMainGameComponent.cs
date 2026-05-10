using Game.Components;
using Game.Utilities;
using Unity.VisualScripting;
using UnityEngine;

public class GotoMainGameComponent : ButtonRenderer
{
    private void Awake()
    {
        OnClickEvent.AddListener(GoToMainGame);
    }

    private void GoToMainGame()
    {
        FindObjectOfType<ParagraphGameComponent>().Container.SetActive(false);
        FindObjectOfType<MainGameplayComponent>().OptionSelection.StartGameplay();
    }
}
