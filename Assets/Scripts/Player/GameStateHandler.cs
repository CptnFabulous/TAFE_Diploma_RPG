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
    PlayerHandler ph;

    [Header("Menus")]
    public Canvas headsUpDisplay;
    public Canvas inventoryScreen;
    public Canvas pauseMenu;
    public Canvas winMenu;
    public Canvas failMenu;
    public bool pauseWhileInMenus = true;

    public string mainMenuSceneName;

    GameState currentState;

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
        if (Input.GetButtonDown("AccessMenus")) // Toggles in-game menu/inventory
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
        
        if (Input.GetButtonDown("Pause")) // Toggles pausing the game
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

    #region Functions for changing game state
    public void PauseGame()
    {
        currentState = GameState.Paused; // Set gamestate
        Time.timeScale = 0; // Time is paused
        Cursor.lockState = CursorLockMode.None; // Unlock cursor
        Cursor.visible = true; // Show cursor
        SwitchMenu(pauseMenu);
    }

    public void GoIntoMenus()
    {
        currentState = GameState.InMenus; // Set gamestate
        if (pauseWhileInMenus == true) // Time is paused, but only if pauseWhileInMenus is enabled
        {
            Time.timeScale = 0;
        }
        Cursor.lockState = CursorLockMode.None; // Unlock cursor
        Cursor.visible = true; // Show cursor
        SwitchMenu(inventoryScreen); // Switch to appropriate menu
    }

    public void ResumeGame()
    {
        currentState = GameState.Active; // Set gamestate
        Time.timeScale = 1; // Time moves at normal rate
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor
        Cursor.visible = false; // Hide cursor
        SwitchMenu(headsUpDisplay); // Switch to appropriate menu
    }

    public void WinGame()
    {
        currentState = GameState.Won; // Set gamestate
        Time.timeScale = 1; // Time moves at normal rate
        Cursor.lockState = CursorLockMode.None; // Unlock cursor
        Cursor.visible = true; // Show cursor
        SwitchMenu(winMenu); // Switch to appropriate menu
    }

    public void FailGame()
    {
        currentState = GameState.Failed; // Set gamestate
        Time.timeScale = 1; // Time moves at normal rate
        Cursor.lockState = CursorLockMode.None; // Unlock cursor
        Cursor.visible = true; // Show cursor
        SwitchMenu(failMenu); // Switch to appropriate menu
    }
    #endregion

    public void QuitToMenu() // Allows coroutine to be run as an event from a button
    {
        print("About to quit to menu");
        StartCoroutine(SaveAndLoad.SaveAndQuit(ph, mainMenuSceneName)); // Saves inventory data and quits to menu
    }

    void SwitchMenu(Canvas menu) // Enables desired menu and disables all others
    {
        headsUpDisplay.gameObject.SetActive(false);
        inventoryScreen.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);
        failMenu.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
    }
}


