using UnityEngine;

public class GameStateController : MonoBehaviour
{
    /// <summary>
    /// This class handles controlling which canvases are visible at any time according to the current game state.
    /// Game states are set according to the button that was pressed
    /// </summary>
    /// 


    private bool _gameended = false;

    public bool Get_GameEnded()
    {
        return _gameended;
    }

    public void Set_GameEnded(bool val)
    {
        _gameended = val;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

}
