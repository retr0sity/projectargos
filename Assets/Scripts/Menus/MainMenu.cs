using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
