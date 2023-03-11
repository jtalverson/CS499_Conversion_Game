using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrikeController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> strikes;
    public GameObject strikePrefab;
    public Color offColor;
    public Color onColor;
    void Start()
    {

    }
    public void setNumStrikes(GameStateController.GameDifficulty gameDifficulty)
    {
        // disable all strikes
        foreach (var strike in strikes) strike.SetActive(false);
        int numStrikes = 0;
        switch (gameDifficulty)
        {
            case GameStateController.GameDifficulty.Easy: numStrikes = 5; break;
            case GameStateController.GameDifficulty.Normal: numStrikes = 3; break;
            case GameStateController.GameDifficulty.Hard: numStrikes = 1; break;
        }
        for (int i = 0; i < numStrikes; i++)
        {
            strikes[i].SetActive(true);
            setStrikeStatus(i, false);
        }
    }
    
    void setStrikeStatus(int strikeIndex, bool status)
    {
        var strikeImage = Utility.FindObject(strikes[strikeIndex], "Controller").GetComponent<Image>();
        if (status)
            strikeImage.color = onColor;
        else
            strikeImage.color = offColor;
    }
}
