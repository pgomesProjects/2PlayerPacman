using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TitlescreenManager : MonoBehaviour
{
    [SerializeField] private GameObject[] menuObjects;
    [SerializeField] private TextMeshProUGUI[] textObjects;
    [SerializeField] private Sprite[] spriteObjects;
    [SerializeField] private RawImage[] playerSprite;
    [SerializeField] private GameObject[] arrowObjects;
    [SerializeField] private GameObject[] readyObjects;
    private bool[] playerReady = { false, false };
    private int[] playerSpriteIndex = { 0, 0 };
    private int[] chosenPlayers = { -1, -1 };
    private bool[] menuStates = { true, false };
    private int currentState = 0;
    private bool isGameStarting = false;
    public string sceneToLoad;

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case 0:
                TitleScreen();
                break;
            case 1:
                PlayerSelect();
                break;
        }
    }

    private void UpdateMenu()
    {
        for (int i = 0; i < menuObjects.Length; i++)
            menuObjects[i].SetActive(menuStates[i]);
    }//end of UpdateMenu

    private void TitleScreen()
    {
        //Start game function
        if (Input.GetKeyDown(KeyCode.Space))
        {
            menuStates[0] = false;
            menuStates[1] = true;
            currentState = 1;
            UpdateMenu();
        }

        //Quit function
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Quitting Game...");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }//end of TitleScreen

    private void PlayerSelect()
    {
        if(!isGameStarting)
            SelectPlayerType();
    }//end of PlayerSelect
    
    private void SelectPlayerType()
    {
        //If player one is not ready, check for input here
        if (!playerReady[0])
        {
            //Player One Left
            if (Input.GetKeyDown(KeyCode.A))
            {
                SwapSprite(-1, 0);
            }
            //Player One Right
            else if (Input.GetKeyDown(KeyCode.D))
            {
                SwapSprite(1, 0);
            }

            //If the player presses 1, they are ready
            if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                ReadyPlayer(0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            UnreadyPlayer(0);

        //If player two is not ready, check for input here
        if (!playerReady[1])
        {
            //Player Two Left
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SwapSprite(-1, 1);
            }
            //Player Two Right
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SwapSprite(1, 1);
            }

            //If the player presses 2, they are ready
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                ReadyPlayer(1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            UnreadyPlayer(1);

    }//end of SelectPlayerType

    private void SwapSprite(int direction, int playerIndex)
    {
        switch (direction)
        {
            case -1:
                playerSpriteIndex[playerIndex]--;
                if (playerSpriteIndex[playerIndex] < 0)
                    playerSpriteIndex[playerIndex] = playerSpriteIndex.Length - 1;
                switch (playerIndex)
                {
                    case 0:
                        if (playerSpriteIndex[playerIndex] == chosenPlayers[1])
                            SwapSprite(-1, 0);
                        break;
                    case 1:
                        if (playerSpriteIndex[playerIndex] == chosenPlayers[0])
                            SwapSprite(-1, 1);
                        break;
                }
                break;
            case 1:
                playerSpriteIndex[playerIndex]++;
                if (playerSpriteIndex[playerIndex] == playerSpriteIndex.Length)
                    playerSpriteIndex[playerIndex] = 0;
                switch (playerIndex)
                {
                    case 0:
                        if (playerSpriteIndex[playerIndex] == chosenPlayers[1])
                            SwapSprite(1, 0);
                        break;
                    case 1:
                        if (playerSpriteIndex[playerIndex] == chosenPlayers[0])
                            SwapSprite(1, 1);
                        break;
                }
                break;
        }

        playerSprite[playerIndex].texture = spriteObjects[playerSpriteIndex[playerIndex]].texture;

        switch (playerSpriteIndex[playerIndex])
        {
            case 0:
                textObjects[playerIndex].text = "Pac-Man";
                break;
            case 1:
                textObjects[playerIndex].text = "Ghost";
                break;
        }

    }//end of SwapSprite

    private void ReadyPlayer(int playerIndex)
    {
        playerReady[playerIndex] = true;
        arrowObjects[playerIndex].SetActive(false);
        readyObjects[playerIndex].SetActive(true);
        chosenPlayers[playerIndex] = playerSpriteIndex[playerIndex];
        switch (chosenPlayers[playerIndex])
        {
            case 0:
                GameManager.instance.currentPlayerSetup[playerIndex] = GameManager.Player.PLAYERONE;
                break;
            case 1:
                GameManager.instance.currentPlayerSetup[playerIndex] = GameManager.Player.PLAYERTWO;
                break;
        }
        if (IsReadyForPlay())
            StartGame();
        else
            UpdateMenuOptions(playerIndex);
    }//end of ReadyPlayer

    private void UnreadyPlayer(int playerIndex)
    {
        playerReady[playerIndex] = false;
        arrowObjects[playerIndex].SetActive(true);
        readyObjects[playerIndex].SetActive(false);
        chosenPlayers[playerIndex] = -1;
    }//end of UnreadyPlayer

    private bool IsReadyForPlay()
    {
        //If any of the player are not ready, return false
        foreach (var i in playerReady)
            if (!i)
                return false;

        //If all players are ready, return true
        return true;
    }//end of IsReadyForPlay

    private void UpdateMenuOptions(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                if(playerSprite[1].texture == spriteObjects[chosenPlayers[playerIndex]].texture)
                    SwapSprite(1, 1);
                break;
            case 1:
                if (playerSprite[0].texture == spriteObjects[chosenPlayers[playerIndex]].texture)
                    SwapSprite(1, 1);
                break;
        }
    }//end of UpdateMenuOptions

    private void StartGame()
    {
        isGameStarting = true;
        StartCoroutine(WaitForSceneLoad());
    }//end of StartGame

    IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(0.5f);

        //Load scene
        SceneManager.LoadScene(sceneToLoad);
    }//end of WaitForSceneLoad
}
