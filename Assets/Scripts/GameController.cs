using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Pagination;

public class GameController : MonoBehaviour
{
    public CSVCalc calculations;
    public CSVConv conversions;

    public PopulateDiff difficulties;

    public PagedRect diffScroller;
    public int diffIndex = -1;
    public PopulateDiff.Difficulty currentDifficulty;
    public PopulateDiff.Difficulty counter;


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
    }    

    public void Populate()
    {
        bool usedAllStrikes = counter.StrikeLimit == currentDifficulty.StrikeLimit;
        bool conversionDone = counter.ConversionCount == currentDifficulty.ConversionCount;
        bool calculationDone = counter.CalculationCount == currentDifficulty.CalculationCount;
        if (!usedAllStrikes && !conversionDone)
        {
            conversions.PopulateConversion();
            counter.ConversionCount += 1;
        }
        else if (!usedAllStrikes && !calculationDone)
        {
            calculations.PopulateCalculation();
            counter.CalculationCount += 1;
        }
        else if (!usedAllStrikes && conversionDone && calculationDone)
        {
            // Game Over WIN
        }
        else
        {
            // Game Over LOST
        }
    }

    public void ResetCounter()
    {
        counter.CalculationCount = 0;
        counter.ConversionCount = 1;
        counter.StrikeLimit = 0;
    }
}