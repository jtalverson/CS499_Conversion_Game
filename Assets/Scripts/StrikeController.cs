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
    public int NumStrikesEasy;
    public int NumStrikesNormal;
    public int NumStrikesHard;

    public void setNumStrikes(int numStrikes)
    {
        // disable all strikes
        foreach (var strike in strikes) strike.SetActive(false);
        for (int i = 0; i < numStrikes; i++)
        {
            strikes[i].SetActive(true);
            setStrikeStatus(i, false);
        }
    }

    public void setStrikeStatus(int strikeIndex, bool status)
    {
        var strikeImage = Utility.FindObject(strikes[strikeIndex], "Controller").GetComponent<Image>();
        if (status)
            strikeImage.color = onColor;
        else
            strikeImage.color = offColor;
    }

}
