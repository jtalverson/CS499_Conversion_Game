using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Security.Cryptography.X509Certificates;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using Random = UnityEngine.Random;
//using UnityEngine.CoreModule;

public class CSVCalc : MonoBehaviour
{
    //creating an assest that will connect to the CSV we want to read in
    public TextAsset textAssetData;

    //creating Text Components that will
    public TextMeshProUGUI question1Text;
    public TextMeshProUGUI question2Text;
    public TextMeshProUGUI question3Text;
    public string answerText;

    //creating the UI Game Objects
    public GameObject question1Object;
    public GameObject question2Object;
    public GameObject question3Object;

    //Class to hold the different variables read in from the CSV
    [System.Serializable]
    public class Question
    {
        public string question1;
        public string question2;
        public string question3;
        public string answers;
    }

    //Class that creates a list of the questions
    [System.Serializable]
    public class QuestionList
    {
        public Question[] question;
    }

    //new question List
    public QuestionList myQList = new QuestionList();

    // Start is called before the first frame update
    void Start()
    {
        ReadCSV();
        //PopulateCalculation();
    }

    //Function to choose a random entry in the list and display on the screen
    public void PopulateCalculation()
    {
        //random number from the options in the Question List
        int random = Random.Range(0, 112);
        List<int> numbersUsed = new List<int>();

        //check if that question has already been asked
        if (!numbersUsed.Contains(random))
        {
            question1Text = question1Object.GetComponent<TextMeshProUGUI>();
            question2Text = question2Object.GetComponent<TextMeshProUGUI>();
            question3Text = question3Object.GetComponent<TextMeshProUGUI>();

            question1Text.text = myQList.question[random].question1;
            question2Text.text = myQList.question[random].question2;
            question3Text.text = myQList.question[random].question3;

            answerText = myQList.question[random].answers;

            //add new number into used number list
            numbersUsed.Add(random);
        }
    }

    //Function to take in the data from the CSV
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