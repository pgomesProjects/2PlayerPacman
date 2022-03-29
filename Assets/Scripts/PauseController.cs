using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string[] playerPauseText;
    [SerializeField] private TextMeshProUGUI pauseText;
    private int playerIndex;
    private bool isPaused = false;

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isGameAnimationActive)
        {
            if (!isPaused)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    playerIndex = 0;
                    PauseGame();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    playerIndex = 1;
                    PauseGame();
                }
            }
            else
            {
                switch (playerIndex)
                {
                    case 0:
                        if (Input.GetKeyDown(KeyCode.Alpha1))
                        {
                            ResumeGame();
                        }
                        else if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            ReturnToMain();
                        }
                        break;
                    case 1:
                        if (Input.GetKeyDown(KeyCode.Alpha2))
                        {
                            ResumeGame();
                        }
                        else if (Input.GetKeyDown(KeyCode.Return))
                        {
                            ReturnToMain();
                        }
                        break;
                }
            }
        }
    }

    private void PauseGame()
    {
        if (!isPaused)
        {
            foreach (var i in LevelManager.Level.pausePrompts)
                i.SetActive(false);
            isPaused = true;
            FindObjectOfType<AudioManager>().Pause("InGameMusic");
            FindObjectOfType<AudioManager>().Play("ClickSFX", 1);
            pauseText.text = playerPauseText[playerIndex];
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void ResumeGame()
    {
        if (isPaused)
        {
            foreach (var i in LevelManager.Level.pausePrompts)
                i.SetActive(true);
            isPaused = false;
            FindObjectOfType<AudioManager>().Resume("InGameMusic");
            FindObjectOfType<AudioManager>().Play("CancelSFX", 1);
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    private void ReturnToMain()
    {
        FindObjectOfType<AudioManager>().Stop("InGameMusic");
        FindObjectOfType<AudioManager>().Play("CancelSFX", 1);
        Time.timeScale = 1;
        SceneManager.LoadScene("Titlescreen");
    }
}
