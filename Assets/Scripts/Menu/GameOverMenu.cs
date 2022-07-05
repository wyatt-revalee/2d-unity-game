using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{

    public void PlayGame() {
        SceneManager.LoadScene(1);
    }

    public void MainMenuPress() {
        SceneManager.LoadScene(0);
    }

    public void QuitGame() {
        Debug.Log("QUIT");
        Application.Quit();
    }

}
