using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Animator transition;
    public float transitionTime = 1f;

    public void PlayGame() {
        StartCoroutine(SceneTransition());
    }

    public void QuitGame() {
        Debug.Log("QUIT");
        Application.Quit();
    }

    IEnumerator SceneTransition()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
