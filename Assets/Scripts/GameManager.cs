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
    public bool isGameAnimationActive = false;

    //Data that saves the sprite of Pacman
    public Sprite pacmanSprite;

    //Data that saves the color of Ghost
    public Color ghostColor;

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
        isGameAnimationActive = false;
        SceneManager.LoadScene(goToLevel);
    }

}
