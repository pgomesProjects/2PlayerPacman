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
    private bool resumeVulnerable = false, resumeWaka = false;

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
            if (FindObjectOfType<AudioManager>().IsPlaying("PowerPelletSFX"))
            {
                resumeVulnerable = true;
                FindObjectOfType<AudioManager>().Pause("PowerPelletSFX");
            }

            if (FindObjectOfType<AudioManager>().IsPlaying("Waka"))
            {
                resumeWaka = true;
                FindObjectOfType<AudioManager>().Pause("Waka");
            }
            FindObjectOfType<AudioManager>().Play("ClickSFX", GameManager.sfxVolume);
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
            if (resumeVulnerable)
            {
                FindObjectOfType<AudioManager>().Resume("PowerPelletSFX");
                resumeVulnerable = false;
            }
            if (resumeWaka)
            {
                FindObjectOfType<AudioManager>().Resume("Waka");
                resumeWaka = false;
            }
            FindObjectOfType<AudioManager>().Play("CancelSFX", GameManager.sfxVolume);
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    private void ReturnToMain()
    {
        FindObjectOfType<AudioManager>().Stop("InGameMusic");
        if (resumeVulnerable)
        {
            FindObjectOfType<AudioManager>().Stop("PowerPelletSFX");
        }
        if (resumeWaka)
        {
            FindObjectOfType<AudioManager>().Stop("Waka");
        }
        FindObjectOfType<AudioManager>().Play("CancelSFX", GameManager.sfxVolume);
        Time.timeScale = 1;
        SceneManager.LoadScene("Titlescreen");
    }
}
