using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Level;

    public float secondsUntilStart = 5;

    [SerializeField]private GameObject readyText;
    public TextMeshProUGUI gameOverText;
    public Image[] allLiveSprites;
    public Sprite jeffuSkin;
    public GameObject[] pausePrompts;
    private int totalPellets;
    [SerializeField] private Tilemap levelMap;
    private Color levelColor;
    private Color currentColor;
    [SerializeField] private Tilemap pelletMap;
    [SerializeField] private PlayerController[] player;

    private void Awake()
    {
        Level = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        totalPellets = GetTotalOfPellets();
        levelColor = levelMap.color;
        currentColor = levelColor;

        //TODO: Set the Pacman and Ghost Data before the game starts
        FindObjectOfType<PacmanController>().GetComponent<Animator>().SetInteger("PacmanSkin", GameManager.instance.pacmanSkinNumber);
        FindObjectOfType<PacmanController>().GetComponent<Animator>().SetFloat("Speed", 0);
        FindObjectOfType<GhostController>().GetComponent<SpriteRenderer>().color = GameManager.instance.ghostColor;
        FindObjectOfType<GhostController>().GetComponent<Animator>().SetFloat("Speed", 0);

        switch (GameManager.instance.pacmanSkinNumber)
        {
            case 1:
                foreach (var i in allLiveSprites)
                    i.sprite = jeffuSkin;
                break;
        }

        StartCoroutine(StartingAnimation());
    }
    private int GetTotalOfPellets()
    {
        pelletMap.CompressBounds();
        int amount = 0;
        foreach (var pos in pelletMap.cellBounds.allPositionsWithin)
        {
            Tile tile = pelletMap.GetTile<Tile>(pos);
            if (tile != null) { amount += 1; }
        }
        return amount + 4;
    }

    public void EndAnimation()
    {
        StartCoroutine(EndingAnimation());
    }

    IEnumerator StartingAnimation()
    {
        GameManager.instance.isGameAnimationActive = true;
        FindObjectOfType<AudioManager>().Play("StartingJingle", GameManager.gameVolume);

        yield return new WaitForSeconds(secondsUntilStart);
        readyText.SetActive(false);
        GameManager.instance.isGameAnimationActive = false;
        foreach (var i in pausePrompts)
            i.SetActive(true);
        FindObjectOfType<AudioManager>().Play("InGameMusic", GameManager.gameVolume);

        FindObjectOfType<PacmanController>().GetComponent<Animator>().SetFloat("Speed", 1);
        FindObjectOfType<GhostController>().GetComponent<Animator>().SetFloat("Speed", 1);
    }

    public IEnumerator DeathAnimation()
    {
        GameManager.instance.isGameAnimationActive = true;
        int currentLives = FindObjectOfType<PacmanController>().life;

        foreach (var i in pausePrompts)
            i.SetActive(false);
        FindObjectOfType<AudioManager>().Stop("InGameMusic");

        FindObjectOfType<PacmanController>().GetComponent<Animator>().SetFloat("Speed", 0);
        FindObjectOfType<GhostController>().GetComponent<Animator>().SetFloat("Speed", 0);

        foreach (var i in player)
        {
            i.canMove = false;
            i.canRotateSprite = false;
        }

        yield return new WaitForSeconds(2);

        foreach (var i in player)
        {
            if (i is PacmanController)
                i.gameObject.SetActive(false);
        }

        FindObjectOfType<AudioManager>().Play("DeathSFX", GameManager.gameVolume);

        yield return new WaitForSeconds(1.5f);

        if(currentLives == -1)
        {
            GameOverAnimation();
        }
        else
        {
            StartCoroutine(RestartAnimation());
        }

    }

    private void GameOverAnimation()
    {
        gameOverText.gameObject.SetActive(true);
        GameManager.instance.EndGame("Titlescreen", 3);
    }

    IEnumerator RestartAnimation()
    {
        foreach (var i in player)
        {
            if (i is PacmanController)
                i.gameObject.SetActive(true);

            i.direction = new Vector2(1, 0);
        }

        PacmanController pacmanController = FindObjectOfType<PacmanController>();
        pacmanController.GetComponent<Animator>().SetInteger("PacmanSkin", GameManager.instance.pacmanSkinNumber);
        pacmanController.GetComponent<Animator>().SetFloat("Speed", 0);
        pacmanController.FlipSprite(0, 1);

        GhostController ghostController = FindObjectOfType<GhostController>();
        ghostController.ChangeEyes(0);
        allLiveSprites[pacmanController.life].gameObject.SetActive(false);
        pacmanController.transform.position = pacmanController.spawnPoint.transform.position;
        ghostController.transform.position = ghostController.spawnPoint.transform.position;
        readyText.SetActive(true);
        yield return new WaitForSeconds(1);
        readyText.SetActive(false);
        GameManager.instance.isGameAnimationActive = false;
        foreach (var i in pausePrompts)
            i.SetActive(true);
        FindObjectOfType<AudioManager>().Play("InGameMusic", GameManager.gameVolume);

        foreach (var i in player)
        {
            i.canMove = true;
            i.canRotateSprite = true;
        }

        FindObjectOfType<PacmanController>().GetComponent<Animator>().SetFloat("Speed", 1);
        FindObjectOfType<GhostController>().GetComponent<Animator>().SetFloat("Speed", 1);
    }

    IEnumerator EndingAnimation()
    {
        GameManager.instance.isGameAnimationActive = true;
        foreach (var i in pausePrompts)
            i.SetActive(false);
        FindObjectOfType<AudioManager>().Stop("InGameMusic");

        FindObjectOfType<PacmanController>().GetComponent<Animator>().SetFloat("Speed", 0);
        FindObjectOfType<GhostController>().GetComponent<Animator>().SetFloat("Speed", 0);

        foreach (var i in player)
            i.canMove = false;

        yield return new WaitForSeconds(2);

        foreach (var i in player)
        {
            if(i is GhostController)
                i.gameObject.SetActive(false);
        }

        float secondsPlayed = 2;

        float randomEasterEggChance = Random.Range(1, 101);

        //3% chance for a joke song at the end
        if(randomEasterEggChance <= 3)
        {
            secondsPlayed = 15.5f;
            FindObjectOfType<AudioManager>().Play("EasterEgg", GameManager.gameVolume * 0.4f);
        }

        for (int i = 0; i < secondsPlayed * 4; i++)
        {
            if (currentColor == levelColor)
                currentColor = new Color(255, 255, 255, 255);
            else
                currentColor = levelColor;

            levelMap.color = currentColor;

            yield return new WaitForSeconds(0.25f);
        }

        GameManager.instance.isGameAnimationActive = false;
        ReturnToMain();
    }

    private void ReturnToMain()
    {
        SceneManager.LoadScene("Titlescreen");
    }

    public int GetTotalPellets() { return totalPellets; }
    public void SetTotalPellets(int pellets) { totalPellets = pellets; }
    public void RemovePellet() { totalPellets--; }
}
