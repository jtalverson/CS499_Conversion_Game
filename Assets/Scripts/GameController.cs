using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Pagination;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public CSVCalc calculations;
    public CSVConv conversions;

    public PopulateDiff difficulties;

    public PagedRect diffScroller;
    public int diffIndex = -1;
    public PopulateDiff.Difficulty currentDifficulty;
    public PopulateDiff.Difficulty counter;

    public StrikeController strikeController;

    public Timer timer;

    // Start is called before the first frame update
    private void Start()
    {
        ResetCounter();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DetermineDifficulty()
    {
        diffIndex = diffScroller.GetCurrentPage().PageNumber - 1;
        currentDifficulty = difficulties.config.populateData[diffIndex];
        timer.maxTime = currentDifficulty.TimePerCalculation;
        timer.timeRemaining = timer.maxTime;
    }    

    public void Populate(bool questionRight)
    {
        if (!questionRight && counter.StrikeLimit < currentDifficulty.StrikeLimit)
        {
            Image currentStrike = strikeController.strikes[counter.StrikeLimit].GetComponentInChildren<Image>();
            currentStrike.color = strikeController.onColor;
            counter.StrikeLimit += 1;
        }

        bool usedAllStrikes = counter.StrikeLimit == currentDifficulty.StrikeLimit;
        bool conversionDone = counter.ConversionCount == currentDifficulty.ConversionCount;
        bool calculationDone = counter.CalculationCount == currentDifficulty.CalculationCount;

        if (conversionDone)
            timer.maxTime = currentDifficulty.TimePerCalculation;

        if (!usedAllStrikes && !conversionDone)
        {
            conversions.PopulateConversion();
            if (questionRight)
                counter.ConversionCount += 1;
        }
        else if (!usedAllStrikes && !calculationDone)
        {
            calculations.PopulateCalculation();
            if (questionRight)
                counter.CalculationCount += 1;
        }
        else if (!usedAllStrikes && conversionDone && calculationDone)
        {
            // Game Over WIN
            Debug.Log("You WON!");
        }
        else
        {
            // Game Over LOST
            Debug.Log("You lost...");
        }
    }

    public void ResetCounter()
    {
        counter.CalculationCount = 0;
        counter.ConversionCount = 1;
        counter.StrikeLimit = 0;
    }
}
