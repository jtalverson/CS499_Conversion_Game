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

    private bool _gameended = false;

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

    public void Set_GameEnded(bool val, bool winORlose)
    {
        winMessage.SetActive(false);
        loseMessage.SetActive(false);
        if (winORlose)
        {
            audioSource.PlayOneShot(WinSound);
            winMessage.SetActive(true);
        }

        else
        {
            audioSource.PlayOneShot(LoseSound);
            loseMessage.SetActive(true);
        }

        Utility.FindObject(gameObject, "RecapScoreValue").GetComponent<TextMeshProUGUI>().text = scoringSystem.score.ToString();
        Utility.FindObject(gameObject, "RecapStreakValue").GetComponent<TextMeshProUGUI>().text = scoringSystem.bestStreak.ToString();

        _gameended = val;
    }

    private void Start()
    {
        Application.targetFrameRate = 120;
    }

}
