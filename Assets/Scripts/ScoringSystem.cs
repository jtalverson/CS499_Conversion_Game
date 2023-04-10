using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    public float score;
    public float multiplier;
    static private float multiplierIncrease = 0.2f;
    static private float baseScore = 100000.0;

    public void Start()
    {
        score = 0.0f;
        multiplier = 1.0f;
    }

    /* Update is called after every swipe
     * startTime - the time on the timer when the problem is first shown
     * currentTime - the time left on the timer when the problem is answered
     * correct - if the question was scored correctly or not
    */
    public void ScoreUpdate(float startTime, float currentTime, bool correct)
    {
        float inverseTimeLeft = baseScore - (startTime - currentTime); // ASSUME startTime - currentTime > 0
        if (correct == true) // add to score
        {
            multiplier += multiplierIncrease;
            score += inverseTimeLeft;
            score = score * multiplier;
        }
        else // add no points, reset multiplier
        {
            multiplier = 1.0f;
        }
    }
}
