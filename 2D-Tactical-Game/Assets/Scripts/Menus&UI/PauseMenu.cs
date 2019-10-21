using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Main Menu Button
    public void MainMenu ()
    {
        SceneManager.LoadScene(0);
    }
    // Quit Game Button
    public void QuitGame()
    {
        Debug.Log("the game has quit.");
        Application.Quit();
    }
}
