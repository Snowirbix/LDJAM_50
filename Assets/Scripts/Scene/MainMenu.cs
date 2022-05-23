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
        FadeAnimator.SetTrigger("Fade");
        StartCoroutine(PlayCoroutine());
    }

    IEnumerator PlayCoroutine()
    {
        Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
        yield return new WaitForSeconds(1);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(PlaySceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
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
