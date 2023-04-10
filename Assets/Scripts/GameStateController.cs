using UnityEngine;

public class GameStateController : MonoBehaviour
{
    /// <summary>
    /// This class handles controlling which canvases are visible at any time according to the current game state.
    /// Game states are set according to the button that was pressed
    /// </summary>
    /// 
    public GameObject winMessage;
    public GameObject loseMessage;

    private bool _gameended = false;

    public bool Get_GameEnded()
    {
        return _gameended;
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
            winMessage.SetActive(true);
        else
            loseMessage.SetActive(true);
        _gameended = val;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

}
