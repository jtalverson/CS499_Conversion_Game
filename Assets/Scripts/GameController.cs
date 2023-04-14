using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExternalUI.Pagination;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public CSVCalc calculations; // Calculations script
    public CSVConv conversions;  // Conversions script

    public PopulateDiff difficulties; // Difficulties script

    public PagedRect diffScroller; // Used to determine difficulty when the game begins
    public int diffIndex = -1; // Set diffIndex to -1 to start
    public PopulateDiff.Difficulty currentDifficulty; // Holds the information on the current difficulty
    public PopulateDiff.Difficulty counter; // Keeps track of important game data
    public string diffString; // String which indicates difficulty

    public StrikeController strikeController; // StrikeController script
    public GameStateController stateController; // StateController script

    public Timer timer; // Timer script

    public ScoringSystem scoringSystem; // ScoringSystem script

    public HiScorePageController pageController; // HiScorePageController script

    // Start is called before the first frame update
    private void Start()
    {
        // Reset counter and scoring system
        ResetCounter();
        scoringSystem.ResetScoring();
    }
    // Determines the difficulty and assigns values as necessary
    public void DetermineDifficulty()
    {
        // Resets the counter
        ResetCounter();
        // Resets scoring system
        scoringSystem.ResetScoring();
        // Determines the index of difficulty based on the page object
        diffIndex = diffScroller.GetCurrentPage().PageNumber - 1;
        // Gets the difficulty information 
        currentDifficulty = difficulties.config.populateData[diffIndex];
        // Sets the difficulty string appropriately
        if (diffIndex == 0)
            diffString = "easy";
        if (diffIndex == 1)
            diffString = "normal";
        if (diffIndex == 2)
            diffString = "hard";
        // Set up timer and set to max
        timer.maxTime = currentDifficulty.TimePerConversion;
        timer.timeRemaining = timer.maxTime;
        timer.timerIsRunning = true;
    }    

    public void Populate(bool questionRight)
    {
        // If we havent gotten the question right and there are strikes left
        if (!questionRight && counter.StrikeLimit < currentDifficulty.StrikeLimit)
        {
            // Color the current strike and increment the counter
            strikeController.setStrikeStatus(counter.StrikeLimit, true);
            counter.StrikeLimit += 1;
        }
        // Sets up bools to determine if all strikes have been used
        bool usedAllStrikes = counter.StrikeLimit == currentDifficulty.StrikeLimit;
        // When conversions have been completed
        bool conversionDone = counter.ConversionCount == currentDifficulty.ConversionCount;
        // And when calculations have been completed
        bool calculationDone = counter.CalculationCount == currentDifficulty.CalculationCount;
        // Update the timer's max time if conversions are done
        if (conversionDone)
            timer.maxTime = currentDifficulty.TimePerCalculation;
        // If we have not used all strikes and we are not done with conversions
        if (!usedAllStrikes && !conversionDone)
        {
            // Populate a new set of conversions and if we got it right increment the counter
            conversions.PopulateConversion();
            if (questionRight)
                counter.ConversionCount += 1;
        }
        // If we have not used all strikes and we are not done with calculations
        else if (!usedAllStrikes && !calculationDone)
        {
            // Populate a new set of calculations and if we got it right increment the counter
            calculations.PopulateCalculation();
            if (questionRight)
                counter.CalculationCount += 1;
        }
        // If we have not used all strikes and we are done with conversions and calculations
        else if (!usedAllStrikes && conversionDone && calculationDone)
        {
            // The game is over and we have won the game
            Debug.Log("You WON!");
            pageController.UpdateLists();
            stateController.Set_GameEnded(true, true);
        }
        // Otherwise we have used all of our strikes
        else
        {
            // The game is over and we have lost the game
            Debug.Log("You lost...");
            pageController.UpdateLists();
            stateController.Set_GameEnded(true, false);
        }
    }
    // Resets the counter to its default values
    public void ResetCounter()
    {
        counter.CalculationCount = 0;
        counter.ConversionCount = 0;
        counter.StrikeLimit = 0;
    }
}
