using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopulateDiff : MonoBehaviour
{
    private int propertyCount = 5;
    public TextAsset textAssetData; 

    /// Class <c>Difficulty</c> models a difficulty level with numeric constraints.
    [System.Serializable]
    public class Difficulty
    {   
        // Properties
        public int ConversionCount; // Number of conversion problems in a set
        public int TimePerConversion; // Time (in secs) allowed to spend on a conversion problem
        public int CalculationCount; // Number of calculation problems in a set
        public int TimePerCalculation; // Time (in secs) allowed to spend on a calculation problem
        public int StrikeLimit; // Number of incorrect answers allowed before game forced to be over
    }

    /// Class <c>DifficultyList</c> holds a list of all the difficulty levels' configurations.
    [System.Serializable]
    public class DifficultyList
    {
        public Difficulty[] populateData;
    }

    public DifficultyList config = new DifficultyList();

    // Start is called before the first frame update
    void Start()
    {
        // Read configurations from csv file
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / propertyCount - 1; // Number of properties
        
        config.populateData = new Difficulty[tableSize];
    
        for (int i = 0; i < tableSize; i++)
        {
            // Assign each level's configuration to corresponding property
            config.populateData[i] = new Difficulty(); 
            config.populateData[i].ConversionCount = Int32.Parse(data[propertyCount * (i+1)]);
            config.populateData[i].TimePerConversion = Int32.Parse(data[propertyCount * (i+1) + 1]);
            config.populateData[i].CalculationCount = Int32.Parse(data[propertyCount * (i+1) + 2]);
            config.populateData[i].TimePerCalculation = Int32.Parse(data[propertyCount * (i+1) + 3]);
            config.populateData[i].StrikeLimit = Int32.Parse(data[propertyCount * (i+1) + 4]);
        }
    }
}
