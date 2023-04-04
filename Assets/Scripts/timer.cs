using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class timer : MonoBehaviour
{
    public float timeRemaining;
    public bool timerIsRunning = false;
    public float timerStop;
    public Text timeText;
    private void Start()
    {
        /*
            if easy
                timeRemaining = ;
            if medium
                timeRemaning = ;
            if hard
                timeRemaing = ;
         */
        // Starts the timer automatically
        timerIsRunning = true;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                /* if (swipe happens)
                    timerStop = Mathf.FloorToInt(timeToDisplay % 60);
                    timerIsRunning = false;
                 */
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{1:00}", minutes, seconds);
    }
}