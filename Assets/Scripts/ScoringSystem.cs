using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoringSystem : MonoBehaviour
{
    public float score;
    public float currentStreak;
    public float bestStreak;
    public float multiplier;
    public float multiplierIncrease = 0.2f;
    public float baseScore = 100000.0f;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;

    public void ResetScoring()
    {
        score = 0.0f;
        currentStreak = 0.0f;
        bestStreak = 0.0f;
        multiplier = 1.0f;

        scoreText.text = score.ToString();
        streakText.text = bestStreak.ToString();
    }

    /* Update is called after every swipe
     * startTime - the time on the timer when the problem is first shown
     * currentTime - the time left on the timer when the problem is answered
     * correct - if the question was scored correctly or not
    */
    public void ScoreUpdate(float timeRemaining, bool correct)
    {
        float inverseTimeLeft = baseScore - timeRemaining; // ASSUME startTime - currentTime > 0
        if (correct == true) // add to score
        {
            multiplier += multiplierIncrease;
            score += inverseTimeLeft;
            score *= multiplier;
            score = Mathf.Round(score);
            currentStreak += 1;
            scoreText.text = score.ToString();
        }
        else // add no points, reset multiplier
        {
            multiplier = 1.0f;
            currentStreak = 0.0f;
        }

        if (currentStreak > bestStreak)
        {
            bestStreak = currentStreak;
            streakText.text = bestStreak.ToString();
        }
    }
}
