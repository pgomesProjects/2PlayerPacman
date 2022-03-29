using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TitlescreenManager : MonoBehaviour
{
    [SerializeField] private GameObject[] menuObjects;
    [Header("Character Select Screen Variables")]
    [SerializeField] private TextMeshProUGUI[] textObjects;
    [SerializeField] private Sprite[] spriteObjects;
    [SerializeField] private RawImage[] playerSprite;
    [SerializeField] private GameObject[] arrowObjects;
    [SerializeField] private GameObject[] readyObjects;

    [Header("Character Customization Screen Variables")]
    [SerializeField] private TextMeshProUGUI[] customizerTextObjects;
    [SerializeField]private Sprite[] pacmanSprites;
    [SerializeField]private string[] pacmanNames;
    [SerializeField]private Color[] ghostColors;
    [SerializeField]private string[] ghostColorNames;
    [SerializeField] private RawImage[] playerCustomizationSprites;
    [SerializeField] private GameObject[] customizationArrowObjects;
    [SerializeField] private GameObject[] customizeReadyObjects;
    [Space(20)]

    public string sceneToLoad;

    private bool[] playerReady = { false, false };
    private int[] playerSpriteIndex = { 0, 0 };
    private int[] chosenPlayers = { -1, -1 };
    private bool[] menuStates = { true, false, false };
    private int currentState = 0;
    private int[] playerCustomizerIndex = { 0, 0 };
    private bool isGameStarting = false;

    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("TitleMusic", GameManager.gameVolume);
    }

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
            case 2:
                CustomizePlayers();
                break;
        }

        //Quit function
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(QuitGameDelay());
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
            FindObjectOfType<AudioManager>().Play("ClickSFX", 1);
            menuStates[0] = false;
            menuStates[1] = true;
            currentState = 1;
            UpdateMenu();
        }
    }//end of TitleScreen

    #region PlayerSelect
    private void PlayerSelect()
    {
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
                FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                SwapSprite(-1, 0);
            }
            //Player One Right
            else if (Input.GetKeyDown(KeyCode.D))
            {
                FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                SwapSprite(1, 0);
            }

            //If the player presses 1, they are ready
            if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                FindObjectOfType<AudioManager>().Play("ClickSFX", 1);
                ReadyPlayerSetup(0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            FindObjectOfType<AudioManager>().Play("CancelSFX", 1);
            UnreadyPlayer(0);
        }

        //If player two is not ready, check for input here
        if (!playerReady[1])
        {
            //Player Two Left
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                SwapSprite(-1, 1);
            }
            //Player Two Right
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                SwapSprite(1, 1);
            }

            //If the player presses 2, they are ready
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                FindObjectOfType<AudioManager>().Play("ClickSFX", 1);
                ReadyPlayerSetup(1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            FindObjectOfType<AudioManager>().Play("CancelSFX", 1);
            UnreadyPlayer(1);
        }

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

    private void ReadyPlayerSetup(int playerIndex)
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
        {
            menuStates[1] = false;
            menuStates[2] = true;
            currentState = 2;
            for (int i = 0; i < playerReady.Length; i++)
            {
                playerReady[i] = false;
            }
            UpdateMenu();
        }
        else
            UpdateMenuOptions(playerIndex);
    }//end of ReadyPlayerSetup

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
                    SwapSprite(1, 0);
                break;
        }
    }//end of UpdateMenuOptions
    #endregion
    private void CustomizePlayers()
    {
        if (!isGameStarting)
        {
            //If player one is not ready, check for input here
            if (!playerReady[0])
            {
                switch (chosenPlayers[0])
                {
                    //Player 1 is Pacman
                    case 0:
                        //Player One Left
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                            SwapPacmanSprites(-1);
                        }
                        //Player One Right
                        else if (Input.GetKeyDown(KeyCode.D))
                        {
                            FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                            SwapPacmanSprites(1);
                        }

                        //If the player presses 1, they are ready
                        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                        {
                            FindObjectOfType<AudioManager>().Play("ClickSFX", 1);
                            ReadyCustomPlayer(0, 0);
                        }
                        break;
                    //Player 1 is Ghost
                    case 1:
                        //Player One Left
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                            SwapGhostSprites(-1);
                        }
                        //Player One Right
                        else if (Input.GetKeyDown(KeyCode.D))
                        {
                            FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                            SwapGhostSprites(1);
                        }

                        //If the player presses 1, they are ready
                        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                        {
                            FindObjectOfType<AudioManager>().Play("ClickSFX", 1);
                            ReadyCustomPlayer(0, 1);
                        }
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                FindObjectOfType<AudioManager>().Play("CancelSFX", 1);
                UnreadyPlayer(0);
            }

            //If player two is not ready, check for input here
            if (!playerReady[1])
            {
                switch (chosenPlayers[1])
                {
                    //Player 2 is Pacman
                    case 0:
                        //Player Two Left
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                            SwapPacmanSprites(-1);
                        }
                        //Player Two Right
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                            SwapPacmanSprites(1);
                        }

                        //If the player presses 2, they are ready
                        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                        {
                            FindObjectOfType<AudioManager>().Play("ClickSFX", 1);
                            ReadyCustomPlayer(1, 0);

                        }
                        break;
                    //Player 2 is Ghost
                    case 1:
                        //Player Two Left
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                            SwapGhostSprites(-1);
                        }
                        //Player Two Right
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            FindObjectOfType<AudioManager>().Play("SelectSFX", 1);
                            SwapGhostSprites(1);
                        }

                        //If the player presses 2, they are ready
                        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                        {
                            FindObjectOfType<AudioManager>().Play("ClickSFX", 1);
                            ReadyCustomPlayer(1, 1);
                        }
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                FindObjectOfType<AudioManager>().Play("CancelSFX", 1);
                UnreadyPlayer(1);
            }
        }
    }//end of CustomizePlayers

    private void SwapPacmanSprites(int direction)
    {
        playerCustomizerIndex[0] += direction;
        
        if (playerCustomizerIndex[0] < 0)
            playerCustomizerIndex[0] = pacmanSprites.Length - 1;
        else if (playerCustomizerIndex[0] > pacmanSprites.Length - 1)
            playerCustomizerIndex[0] = 0;

        playerCustomizationSprites[0].texture = pacmanSprites[playerCustomizerIndex[0]].texture;

        customizerTextObjects[0].text = pacmanNames[playerCustomizerIndex[0]];
    }//end of SwapPacmanSprites

    private void SwapGhostSprites(int direction)
    {
        playerCustomizerIndex[1] += direction;

        if (playerCustomizerIndex[1] < 0)
            playerCustomizerIndex[1] = ghostColors.Length - 1;
        else if (playerCustomizerIndex[1] > ghostColors.Length - 1)
            playerCustomizerIndex[1] = 0;

        playerCustomizationSprites[1].color = ghostColors[playerCustomizerIndex[1]];

        customizerTextObjects[1].text = ghostColorNames[playerCustomizerIndex[1]];
    }//end of SwapGhostSprites


    private void ReadyCustomPlayer(int playerIndex, int playerObject)
    {
        playerReady[playerIndex] = true;
        customizationArrowObjects[playerIndex].SetActive(false);
        customizeReadyObjects[playerIndex].SetActive(true);

        switch (playerObject)
        {
            //Save Pacman data
            case 0:
                Debug.Log("Pacman Data Saved!");
                //TODO: Save the Pacman Sprite Data
                break;
            //Save Ghost data
            case 1:
                Debug.Log("Ghost Data Saved!");
                GameManager.instance.ghostColor = playerCustomizationSprites[1].color;
                break;
        }

        if (IsReadyForPlay())
        {
            StartGame();
        }
    }//end of ReadyCustomPlayer

    #region StartGame
    private void StartGame()
    {
        isGameStarting = true;
        StartCoroutine(WaitForSceneLoad());
    }//end of StartGame

    IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(0.5f);

        FindObjectOfType<AudioManager>().Stop("TitleMusic");

        //Load scene
        SceneManager.LoadScene(sceneToLoad);
    }//end of WaitForSceneLoad

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
    #endregion
}
