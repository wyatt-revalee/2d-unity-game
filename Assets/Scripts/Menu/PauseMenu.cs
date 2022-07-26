using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public PlayerMovement playerMovement;

    public void Resume() {
        //Resume game
    }

    public void QuitToMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
