using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{
    public float maxTime;
    public float timeRemaining;
    public bool timerIsRunning = false;
    public Slider timeSlider;
    public Swipe swipe;

    void Update()
    {
        //checks if the timer is running
        if (timerIsRunning)
        {
            //if the value is bigger than zero the timer counts down and updates the slider
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateSlider();
            }
            //else the timer is stopped and tells the program its at zero
            else
            {
                Debug.Log("Time has run out!");
                timerIsRunning = false;
                StartCoroutine(swipe.TimeUp());
            }
        }
    }
    //updates slider forst sets the max value to the max time then sets the slider value to the time left
    public void UpdateSlider()
    {
        timeSlider.maxValue = maxTime;
        timeSlider.value = timeRemaining;
    }
}
