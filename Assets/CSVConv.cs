using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
//using UnityEngine.CoreModule;

public class CSVConv : MonoBehaviour
{
    public TextAsset textAssetData;

    // public TMP_InputField textInput;
    //  private string Text;

    public TextMeshProUGUI question1;
    public TextMeshProUGUI question2;
    public TextMeshProUGUI question3;
    // public TextMeshProUGUI answers;

    [System.Serializable]
    public class Question
    {
        public string question1;
        public string question2;
        public string question3;
        public string answers;
    }

    [System.Serializable]
    public class QuestionList
    {
        public Question[] question;
    }

    public QuestionList myQList = new QuestionList();

    // Start is called before the first frame update
    void Start()
    {
        ReadCSV();
    }

    void ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / 4 - 1;

        myQList.question = new Question[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            myQList.question[i] = new Question();
            myQList.question[i].question1 = data[4 * (i + 1)];
            myQList.question[i].question2 = data[4 * (i + 1) + 1];
            myQList.question[i].question3 = data[4 * (i + 1) + 2];
            myQList.question[i].answers = data[4 * (i + 1) + 3];
        }
    }

}