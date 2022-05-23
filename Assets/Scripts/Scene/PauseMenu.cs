using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour

{
    public static bool IsGamePaused = false;


    public string MenuSceneName;

    public GameObject pauseMenuUI;

    public GameObject HowToPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused)
            {
                HowToPanel.SetActive(false);
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume ()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    void Pause ()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsGamePaused = true;
    }

    public void GoToMenu ()
    {
        SceneManager.LoadScene(MenuSceneName);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    public void HowToPlay ()
    {
        pauseMenuUI.SetActive(false);
        HowToPanel.SetActive(true);
    }

    public void GoBack()
    {
        pauseMenuUI.SetActive(true);
        HowToPanel.SetActive(false);
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
