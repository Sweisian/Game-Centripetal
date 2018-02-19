using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public void StartGame()
    {
        Debug.Log("Trying to play game");
        SceneManager.LoadScene("MainScene");
    }

    public void goToSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void goToMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

}

