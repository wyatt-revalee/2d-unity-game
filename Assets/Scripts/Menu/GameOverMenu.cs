using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{

    public Animator transition;

    public void PlayGame() {        
        StartCoroutine(SceneTransition(1));
    }

    public void MainMenuPress() {
        StartCoroutine(SceneTransition(0));
    }

    public void QuitGame() {
        Debug.Log("QUIT");
        Application.Quit();
    }

    IEnumerator SceneTransition(int sceneIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(sceneIndex);
    }

}
