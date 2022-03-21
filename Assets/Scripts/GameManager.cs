using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
