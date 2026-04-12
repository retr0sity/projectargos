using UnityEngine;
using Game.Components;
using Game.Scriptables;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DeskSelectionComponent : OptionSelectionComponent
{
    // Your desk items instead of his StringData list
    public List<DeskItemData> DeskItems;

    protected override void OptionSelected(InputAction.CallbackContext context)
    {
        // Only do something if the current item is the console
        if (!DeskItems[CurrentIndex].IsConsole)
            return;

        // Otherwise run the normal behavior
        SceneManager.LoadScene("MainGame 1");
    }
}