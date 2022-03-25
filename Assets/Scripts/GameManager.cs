using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum Player { PLAYERONE, PLAYERTWO };
    public Player[] currentPlayerSetup = {Player.PLAYERONE, Player.PLAYERTWO};
    public static float gameVolume = 0.25f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    public void EndGame(string goToLevel, float seconds)
    {
        StartCoroutine(WaitToEnd(goToLevel, seconds));
    }

    public IEnumerator WaitToEnd(string goToLevel, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(goToLevel);
    }

    private void Update()
    {
        //Quit function
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(QuitGameDelay());
        }
    }

    IEnumerator QuitGameDelay()
    {
        Debug.Log("Quitting Game...");
        FindObjectOfType<AudioManager>().Play("CancelSFX", 1);
        yield return new WaitForSeconds(0.25f);
        FindObjectOfType<AudioManager>().Play("CancelSFX", 1);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
