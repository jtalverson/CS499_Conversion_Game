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
    public TextAsset textAssetData;

    //Text Components 
    public TextMeshProUGUI question1Text;
    public TextMeshProUGUI question2Text;
    public TextMeshProUGUI question3Text;
    public string answerText;

    //UI Game Objects
    public GameObject question1Object;
    public GameObject question2Object;
    public GameObject question3Object;

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
        PopulateCalculation();
    }

    public void PopulateCalculation()
    {
        int random = Random.Range(0, 112);
        List<int> numbersUsed = new List<int>();

        while (numbersUsed.Contains(random))
        {
            random = Random.Range(0, 112);
        }

        question1Text = question1Object.GetComponent<TextMeshProUGUI>();
        question2Text = question2Object.GetComponent<TextMeshProUGUI>();
        question3Text = question3Object.GetComponent<TextMeshProUGUI>();

        question1Text.text = myQList.question[random].question1;
        question2Text.text = myQList.question[random].question2;
        question3Text.text = myQList.question[random].question3;

        answerText = myQList.question[random].answers;

        numbersUsed.Add(random);
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