using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum Player { PLAYERONE, PLAYERTWO };
    public Player[] currentPlayerSetup = {Player.PLAYERONE, Player.PLAYERTWO};

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
            Debug.Log("Quitting Game...");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

}
