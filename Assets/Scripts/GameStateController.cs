using System.Collections;
using System.Collections.Generic;
using UI.Pagination;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    /// <summary>
    /// This class handles controlling which canvases are visible at any time according to the current game state.
    /// Game states are set according to the button that was pressed
    /// </summary>
    /// 

    public enum GameStateType
    {
        MainMenu,
        HiScore,
        PreLobby,
        Gameplay,
        ScoreRecap,
        Pause
    }
    public enum GameDifficulty
    {
        Easy = 1,
        Normal = 2,
        Hard = 3 
    }
    [HideInInspector]
    public GameObject currentGameState;
    [HideInInspector]
    public GameObject lastGameState;
    [HideInInspector]
    public GameDifficulty gameDifficulty;
    [Header("MainMenu")]
    public GameObject MainMenuCanvas;
    [Header("High Score")]
    public GameObject HiScoreCanvas;
    [Header("Difficulty")]
    public GameObject DifficultyCanvas;
    [Header("Gameplay")]
    public GameObject GameplayCanvas;
    public GameObject StrikesCanvas;
    [Header("Score Recap")]
    public GameObject ScoreRecapCanvas;
    [Header("Pause")]
    public GameObject PauseCanvas;
    [Header("Credits")]
    public GameObject CreditsCanvas;
    [Header("Overlay Buttons")]
    public GameObject BackButton;
    public GameObject PauseButton;

    void Start()
    {
        currentGameState = MainMenuCanvas;
        // disable all game states that are not the main menu
        PauseButton.SetActive(false);
        BackButton.SetActive(false);
        MainMenuCanvas.SetActive(true);
        HiScoreCanvas.SetActive(false);
        DifficultyCanvas.SetActive(false);
        GameplayCanvas.SetActive(false);
        ScoreRecapCanvas.SetActive(false);
        PauseCanvas.SetActive(false);
        CreditsCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterGame(GameObject DifficultyPagination)
    {
        PagedRect paginationComponent = DifficultyPagination.GetComponent<PagedRect>();
        switch(paginationComponent.CurrentPage)
        {
            case 1:
                gameDifficulty = GameDifficulty.Easy; break;
            case 2:
                gameDifficulty = GameDifficulty.Normal; break;
            case 3:
                gameDifficulty = GameDifficulty.Hard; break;
            default:
                break;
        }
        Debug.Log(string.Format("Entering game with difficulty: {0}", gameDifficulty.ToString()));
        // setup other canvases

        // set strikes
        var strikeController = StrikesCanvas.GetComponent<StrikeController>();
        strikeController.setNumStrikes(gameDifficulty);


        SetActiveCanvas(GameplayCanvas);
    }

    public void EnterPause()
    {
        // pause game functions here
        // show pause canvas
        PauseCanvas.SetActive(true);
        PauseButton.SetActive(false);
    }

    public void ExitPause()
    {
        // Restart game functions
        PauseCanvas.SetActive(false);
        PauseButton.SetActive(true);
    }

    public void ExitGame()
    {
        // reset gameplay state here
        Start();
    }

    public void SetLastCanvas()
    {
        GameObject tmp_;
        currentGameState.SetActive(false);
        tmp_ = currentGameState;
        currentGameState = lastGameState;
        lastGameState = tmp_;
        currentGameState.SetActive(true);

        if (currentGameState.name == "Main Menu")
        {
            // if the scene we're going to is the main menu
            // disable the back button
            BackButton.SetActive(false);
            PauseButton.SetActive(false);
        }

    }

    public void SetActiveCanvas(GameObject newCanvas)
    {
        if (newCanvas.name == "Gameplay") {
            // enable pause button when in game
            // disable the back button
            BackButton.SetActive(false);
            PauseButton.SetActive(true);
        }
        else
        {
            // all other scenes get the back button
            BackButton.SetActive(true);
            PauseButton.SetActive(false);
        }
        lastGameState = currentGameState;
        currentGameState.SetActive(false);
        currentGameState = newCanvas;
        newCanvas.SetActive(true);
    }
}
