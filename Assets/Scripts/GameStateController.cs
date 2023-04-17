using UnityEngine;
using TMPro;

public class GameStateController : MonoBehaviour
{
    /// <summary>
    /// This class handles controlling which canvases are visible at any time according to the current game state.
    /// Game states are set according to the button that was pressed
    /// </summary>
    /// 
    public GameObject winMessage;
    public GameObject loseMessage;
    public ScoringSystem scoringSystem;
    public AudioSource audioSource;
    public AudioClip LoseSound;
    public AudioClip WinSound;
    public AudioClip BtnPressSound;
    public Color CorrectGlowColor;
    public Color WrongGlowColor;

    private bool _playCorrectColor = false;
    private bool _playWrongColor = false;
    private bool _gameended = false;

    public bool PlayCorrectColor
    {
        get { return _playCorrectColor; }
        set { _playCorrectColor = value; }
    }

    public bool PlayWrongColor
    {
        get { return _playWrongColor; }
        set { _playWrongColor = value; }
    }

    public bool Get_GameEnded()
    {
        return _gameended;
    }

    public void init_game()
    {
        GameObject[] canvases = GameObject.FindGameObjectsWithTag("GameCanvas");
        foreach (GameObject gao in canvases)
        {
            gao.SetActive(false);
        }

    }

    public void BtnPress()
    {
        audioSource.PlayOneShot(BtnPressSound);
    }

    public void Set_GameEnded(bool val)
    {
        _gameended = val;
    }
    // End the game with the winORlose passed in
    public void Set_GameEnded(bool val, bool winORlose)
    {
        // Disable both the win and lose message
        winMessage.SetActive(false);
        loseMessage.SetActive(false);
        // If you have won play the sound and set the winMessage to active
        if (winORlose)
        {
            audioSource.PlayOneShot(WinSound);
            winMessage.SetActive(true);
        }
        // Otherwise play the lose sound and set loseMessage to active
        else
        {
            audioSource.PlayOneShot(LoseSound);
            loseMessage.SetActive(true);
        }
        // Update the streak and score recap
        Utility.FindObject(gameObject, "RecapScoreValue").GetComponent<TextMeshProUGUI>().text = scoringSystem.score.ToString();
        Utility.FindObject(gameObject, "RecapStreakValue").GetComponent<TextMeshProUGUI>().text = scoringSystem.bestStreak.ToString();
        // Update _gameended
        _gameended = val;
    }

    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    public bool FTUEDone
    {
        get 
        {
            if (PlayerPrefs.HasKey("ftue_done"))
            {
                if (PlayerPrefs.GetInt("ftue_done") == 1) return true;
                else return false;
            }
            else return false;
                
        }
        set { PlayerPrefs.SetInt("ftue_done", value ? 1 : 0); PlayerPrefs.Save(); }
    }

}
