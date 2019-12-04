using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Active,
    Paused,
    InMenus,
    Won,
    Failed
}

public class GameStateHandler : MonoBehaviour
{
    [Header("Menus")]
    public Canvas headsUpDisplay;
    public Canvas inventoryScreen;
    public Canvas pauseMenu;
    public Canvas winMenu;
    public Canvas failMenu;
    public bool pauseWhileInMenus = true;

    public string mainMenuSceneName;

    GameState currentState;

    PlayerHandler ph;

    void Awake()
    {
        ph = GetComponent<PlayerHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //ChangeGameState(GameState.Active);
        ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("AccessMenus"))
        {
            if (currentState == GameState.Active)
            {
                GoIntoMenus();
            }
            else if (currentState == GameState.InMenus)
            {
                ResumeGame();
            }
        }
        
        if (Input.GetButtonDown("Pause"))
        {
            if (currentState == GameState.Active)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused || currentState == GameState.InMenus)
            {
                ResumeGame();
            }
        }
    }


    /*

        IMPORTANT: Replace ChangeGameState() with different functions for each state, with appropriate variables

    */
    #region Functions for changing game state
    public void PauseGame()
    {
        currentState = GameState.Paused;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SwitchMenu(pauseMenu);
        //print(Cursor.lockState + ", " + Cursor.visible);
    }

    public void GoIntoMenus()
    {
        currentState = GameState.InMenus;

        if (pauseWhileInMenus == true)
        {
            Time.timeScale = 0;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SwitchMenu(inventoryScreen);
        //print(Cursor.lockState + ", " + Cursor.visible);
    }

    public void ResumeGame()
    {
        currentState = GameState.Active;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SwitchMenu(headsUpDisplay);
        //print(Cursor.lockState + ", " + Cursor.visible);
    }

    public void WinGame()
    {
        currentState = GameState.Won;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SwitchMenu(winMenu);
    }

    public void FailGame()
    {
        currentState = GameState.Failed;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SwitchMenu(failMenu);
    }
    #endregion


    public void QuitToMenu()
    {
        print("About to quit to menu");
        StartCoroutine(SaveAndLoad.SaveAndQuit(ph, mainMenuSceneName));
    }

    void SwitchMenu(Canvas menu)
    {
        headsUpDisplay.gameObject.SetActive(false);
        inventoryScreen.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);
        failMenu.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
    }

    
}


