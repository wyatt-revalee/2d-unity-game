using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{

    public Animator transition;

    public void PlayGame() {        
        StartCoroutine(SceneTransition(4));
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
        transition.SetTrigger("End");

        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(sceneIndex);
    }

}
