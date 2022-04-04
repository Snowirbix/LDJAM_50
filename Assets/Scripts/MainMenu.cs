using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string PlaySceneName;

    public GameObject HowToPanel;

    public Animator FadeAnimator;
    public GameObject FadeImage;


    public void PlayGame()
    {
        StartCoroutine(PlayCoroutine());
    }

    IEnumerator PlayCoroutine()
    {
        FadeImage.SetActive(true);
        FadeAnimator.SetTrigger("Fade");
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(PlaySceneName);
    }

    public void HowToPlay()
    {
        HowToPanel.SetActive(true);
    }

    public void GoBack()
    {
        HowToPanel.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
