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
    public float baseMultiplier = 1.0f;

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

    /* ScoreUpdate is called after every swipe
     * timeRemaining - how much time is left when the player answered 
     * correct - if the question was scored correctly or not
    */
    public void ScoreUpdate(float timeRemaining, bool correct)
    {
        if (correct == true) // increase multiplier and streak, add to overall score
        {
            multiplier += multiplierIncrease;
            score += (timeRemaining * baseMultiplier * multiplier);
            Mathf.Round(score);
            currentStreak += 1;
            scoreText.text = score.ToString();
        }
        else // add no points, reset multiplier and streak
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
