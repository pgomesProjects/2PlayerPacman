using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlescreenManager : MonoBehaviour
{
    [SerializeField] private GameObject[] menuObjects;
    private bool[] menuStates = { true, false};
    private int currentState = 0;
    public string sceneToLoad;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
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
        //Start game function
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }//end of PlayerSelect
    
}
