using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{
    public float maxTime;
    public float timeRemaining;
    public bool timerIsRunning = false;
    public Slider timeSlider;
    public Swipe swipe;
    public Color baseColor;
    public Color noTimeColor;


    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateSlider();
            }
            else
            {
                Debug.Log("Time has run out!");
                timerIsRunning = false;
                StartCoroutine(swipe.TimeUp());
            }
        }
    }

    public void UpdateSlider()
    {
        timeSlider.maxValue = maxTime;
        timeSlider.value = timeRemaining;
        float timeRemainingNormalized = 1 - (timeRemaining / maxTime);
        timeSlider.fillRect.gameObject.GetComponent<Image>().color = Color.Lerp(baseColor, noTimeColor, timeRemainingNormalized);
    }
}