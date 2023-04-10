using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    public float score;
    public float multiplier;
    static private float multiplierIncrease = 0.2f;

    public void Start()
    {
        score = 0.0f;
        multiplier = 1.0f;
    }

    // Update is called after every swipe
    public void ScoreUpdate(float points)
    {
        if (points > 0)
        {
            multiplier += multiplierIncrease;
            score += points;
            score = score * multiplier;
        }
        else
        {
            multiplier = 1.0f;
        }
    }
}
