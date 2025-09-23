using UnityEngine;

public interface IInteractable
{
    string GetInteractionPrompt();
    void OnInteract();
    bool CanInteract();
}
