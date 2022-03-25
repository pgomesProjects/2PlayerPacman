using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Level;

    public float secondsUntilStart = 5;

    [SerializeField]private GameObject readyText;
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
        FindObjectOfType<AudioManager>().Play("StartingJingle", GameManager.gameVolume);

        yield return new WaitForSeconds(secondsUntilStart);
        readyText.SetActive(false);
        FindObjectOfType<AudioManager>().Play("InGameMusic", GameManager.gameVolume);
    }

    IEnumerator EndingAnimation()
    {
        FindObjectOfType<AudioManager>().Stop("InGameMusic");

        foreach (var i in player)
            i.canMove = false;

        yield return new WaitForSeconds(2);

        foreach (var i in player)
        {
            if(i is GhostController)
                i.gameObject.SetActive(false);
        }

        for (int i = 0; i < 8; i++)
        {
            if (currentColor == levelColor)
                currentColor = new Color(255, 255, 255, 255);
            else
                currentColor = levelColor;

            levelMap.color = currentColor;

            yield return new WaitForSeconds(0.25f);
        }

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
