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
        Easy,
        Normal,
        Hard
    }
    [HideInInspector]
    public GameObject currentGameState;
    [HideInInspector]
    public GameObject lastGameState;
    [HideInInspector]
    public GameDifficulty gameDifficulty;
    public GameObject MainMenuCanvas;
    public GameObject HiScoreCanvas;
    public GameObject PreLobbyCanvas;
    public GameObject GameplayCanvas;
    public GameObject ScoreRecapCanvas;
    public GameObject PauseCanvas;
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
        PreLobbyCanvas.SetActive(false);
        GameplayCanvas.SetActive(false);
        ScoreRecapCanvas.SetActive(false);
        PauseCanvas.SetActive(false);
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
        SetActiveCanvas(GameplayCanvas);
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
