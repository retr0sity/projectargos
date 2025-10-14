using UnityEngine;

public class MainMenuIntro : MonoBehaviour
{
    [SerializeField] private GameObject buttonsGroup;

    // Called by animation event
    public void ShowButtons()
    {
        buttonsGroup.SetActive(true);
    }
}
