using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PopulateDiff : MonoBehaviour
{
    private int columnCount = 5;
    public TextAsset textAssetData;

    [System.Serializable]
    public class Difficulty
    {
        public string conversionCount;
        public string timePerConversion;
        public string calculationCount;
        public string timePerCalculation;
        public string strikeLimit;
    }

    [System.Serializable]
    public class DifficultyList
    {
        public Difficulty[] populateData;
    }

    public DifficultyList config = new DifficultyList();

    // Start is called before the first frame update
    void Start()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / columnCount - 1;
        // for (int i = 0; i < data.Length; i++)
        //     Debug.Log(data[i]);
        //Debug.Log(data[5*1]);
        config.populateData = new Difficulty[tableSize];
    
        for (int i = 0; i < tableSize; i++)
        {
            Debug.Log(columnCount * (i+1) + 1);
            config.populateData[i] = new Difficulty();
            config.populateData[i].conversionCount = data[columnCount * (i+1)];
            config.populateData[i].timePerConversion = data[columnCount * (i+1) + 1];
            config.populateData[i].conversionCount = data[columnCount * (i+1) + 2];
            config.populateData[i].timePerConversion = data[columnCount * (i+1) + 3];
            config.populateData[i].strikeLimit = data[columnCount * (i+1) + 4];
        }
    }
}
