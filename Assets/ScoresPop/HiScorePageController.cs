using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using TMPro;
using UI.Pagination;

public class HiScorePageController : MonoBehaviour
{
    public GameObject dailyScoreParent;
    public GameObject overallScoreParent;
    public GameObject pageParent;
    public PagedRect pageController;

    public GameController controller;
    public TextAsset allData;
    public string[] data;

    [System.Serializable]
    public class OverallHighScore
    {
        public string difficulty = "";
        public string score = "0";
        public string streak = "0";
    }

    [System.Serializable]
    public class DailyHighScore
    {
        public string date;
        public OverallHighScore easy = new();
        public OverallHighScore normal = new();
        public OverallHighScore hard = new();
    }

    public List<OverallHighScore> overallHighScores = new();
    public List<DailyHighScore> dailyHighScores = new();

    private void Start()
    {
        FillPages();
    }

    public void FillPages()
    {
        overallHighScores.Clear();
        dailyHighScores.Clear();
        ReadData();
        StartCoroutine("PopulateData");
    }

    public void ReadData()
    {
        data = allData.text.Split(new string[] { "\n" }, StringSplitOptions.None);
        //StripLastChar(data);
        int index = 0;

        while (index < data.Length && data[index] != "") // Loads all time high scores
        {
            string[] currentData = data[index].Split(new string[] { "," }, StringSplitOptions.None);
            OverallHighScore currentHighScore = new();
            currentHighScore.difficulty = currentData[0];
            currentHighScore.score = currentData[1];
            currentHighScore.streak = currentData[2];
            overallHighScores.Add(currentHighScore);
            //Debug.Log(data[index]);
            index += 1;
        }
        index += 1;
        while (index < data.Length) // Loads all time high scores
        {
            int moveIndex = 2;
            DailyHighScore currentDailyScore = new();
            currentDailyScore.date = data[index];
            for (int i = 1; i <= 3; i++)
            {
                if (index + i >= data.Length)
                    break;
                //Debug.Log(data[index + i]);
                string[] currentData = data[index + i].Split(new string[] { "," }, StringSplitOptions.None);
                if (currentData[0] == "easy")
                {
                    currentDailyScore.easy.difficulty = currentData[0];
                    currentDailyScore.easy.score = currentData[1];
                    currentDailyScore.easy.streak = currentData[2];
                    moveIndex += 1;
                }
                else if (currentData[0] == "normal")
                {
                    currentDailyScore.normal.difficulty = currentData[0];
                    currentDailyScore.normal.score = currentData[1];
                    currentDailyScore.normal.streak = currentData[2];
                    moveIndex += 1;
                }
                else if (currentData[0] == "hard")
                {
                    currentDailyScore.hard.difficulty = currentData[0];
                    currentDailyScore.hard.score = currentData[1];
                    currentDailyScore.hard.streak = currentData[2];
                    moveIndex += 1;
                }
            }
            dailyHighScores.Add(currentDailyScore);
            //Debug.Log(data[index]);
            index += moveIndex;
        }

        /*DateTime dt = DateTime.Now;
        Debug.Log(dt.Date.ToShortDateString());*/
    }

    public void StripLastChar(string[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
            array[i] = array[i].Remove(array[i].Length - 1);
    }

    public void WriteData()
    {
        string toWrite = "";
        if (overallHighScores.Count > 0 || dailyHighScores.Count > 0)
        {
            for (int i = 0; i < overallHighScores.Count; i++)
            {
                toWrite += overallHighScores[i].difficulty + "," + overallHighScores[i].score + "," + overallHighScores[i].streak + "\n";
            }
            toWrite += "\n";
            int stoppingPoint = Mathf.Min(dailyHighScores.Count, 5);
            for (int i = 0; i < stoppingPoint; i++)
            {
                toWrite += dailyHighScores[i].date + "\n";
                if (dailyHighScores[i].easy.difficulty != "")
                    toWrite += dailyHighScores[i].easy.difficulty + "," + dailyHighScores[i].easy.score + "," + dailyHighScores[i].easy.streak + "\n";
                if (dailyHighScores[i].normal.difficulty != "")
                    toWrite += dailyHighScores[i].normal.difficulty + "," + dailyHighScores[i].normal.score + "," + dailyHighScores[i].normal.streak + "\n";
                if (dailyHighScores[i].hard.difficulty != "")
                    toWrite += dailyHighScores[i].hard.difficulty + "," + dailyHighScores[i].hard.score + "," + dailyHighScores[i].hard.streak + "\n";
                if (i + 1 != stoppingPoint)
                    toWrite += "\n";
            }
        }
        Debug.Log(toWrite);
        File.WriteAllText(AssetDatabase.GetAssetPath(allData), toWrite);
    }

    public IEnumerator PopulateData()
    {
        foreach (Transform t in pageParent.GetComponentsInChildren<Transform>())
            if (t.name.Contains("Page"))
                t.gameObject.SetActive(false);
        pageController.UpdatePagination();

        //Daily high scores
        for (int i = 0; i < dailyHighScores.Count; i++)
        {
            DailyHighScore currentScore = dailyHighScores[i];
            Transform currentPage = Utility.FindObject(dailyScoreParent, "Page " + (i + 1));
            TextMeshProUGUI date = Utility.FindObject(currentPage.gameObject, "Date").GetComponent<TextMeshProUGUI>();
            date.text = currentScore.date;

            string easyScore = "0";
            string easyStreak = "0";
            if (dailyHighScores[i].easy.difficulty != "")
            {
                easyScore = dailyHighScores[i].easy.score;
                easyStreak = dailyHighScores[i].easy.streak;
            }
            Transform easy = Utility.FindObject(currentPage.gameObject, "Easy");
            TextMeshProUGUI score = Utility.FindObject(easy.gameObject, "Score").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI streak = Utility.FindObject(easy.gameObject, "Highest Streak").GetComponent<TextMeshProUGUI>();
            score.text = "Score:\n" + easyScore;
            streak.text = "Best Streak:\n" + easyStreak;

            string normalScore = "0";
            string normalStreak = "0";
            if (dailyHighScores[i].normal.difficulty != "")
            {
                normalScore = dailyHighScores[i].normal.score;
                normalStreak = dailyHighScores[i].normal.streak;
            }
            Transform normal = Utility.FindObject(currentPage.gameObject, "Medium");
            score = Utility.FindObject(normal.gameObject, "Score").GetComponent<TextMeshProUGUI>();
            streak = Utility.FindObject(normal.gameObject, "Highest Streak").GetComponent<TextMeshProUGUI>();
            score.text = "Score:\n" + normalScore;
            streak.text = "Best Streak:\n" + normalStreak;

            string hardScore = "0";
            string hardStreak = "0";
            if (dailyHighScores[i].hard.difficulty != "")
            {
                hardScore = dailyHighScores[i].hard.score;
                hardStreak = dailyHighScores[i].hard.streak;
            }
            Transform hard = Utility.FindObject(currentPage.gameObject, "Hard");
            score = Utility.FindObject(hard.gameObject, "Score").GetComponent<TextMeshProUGUI>();
            streak = Utility.FindObject(hard.gameObject, "Highest Streak").GetComponent<TextMeshProUGUI>();
            score.text = "Score:\n" + hardScore;
            streak.text = "Best Streak:\n" + hardStreak;


            if (!currentPage.gameObject.activeInHierarchy)
                currentPage.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(.1f);
        pageController.UpdatePagination();

        for (int i = 0; i < overallHighScores.Count; i++)
        {
            Transform current = null;
            if (overallHighScores[i].difficulty == "easy")
                current = Utility.FindObject(overallScoreParent, "Easy");
            if (overallHighScores[i].difficulty == "normal")
                current = Utility.FindObject(overallScoreParent, "Medium");
            if (overallHighScores[i].difficulty == "hard")
                current = Utility.FindObject(overallScoreParent, "Hard");

            if (current != null)
            {
                TextMeshProUGUI score = Utility.FindObject(current.gameObject, "Score").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI streak = Utility.FindObject(current.gameObject, "Highest Streak").GetComponent<TextMeshProUGUI>();
                score.text = "Score:\n" + overallHighScores[i].score;
                streak.text = "Best Streak:\n" + overallHighScores[i].streak;
            }
        }
    }

    public void UpdateLists()
    {
        // Update today's data
        if (dailyHighScores[0].date == DateTime.Now.Date.ToShortDateString())
        {
            if (controller.diffString == "easy")
            {
                dailyHighScores[0].easy.difficulty = "easy";
                if (controller.scoringSystem.score > float.Parse(dailyHighScores[0].easy.score))
                    dailyHighScores[0].easy.score = controller.scoringSystem.score.ToString();
                if (controller.scoringSystem.bestStreak > float.Parse(dailyHighScores[0].easy.streak))
                    dailyHighScores[0].easy.streak = controller.scoringSystem.bestStreak.ToString();
            }
            if (controller.diffString == "normal")
            {
                dailyHighScores[0].normal.difficulty = "normal";
                if (controller.scoringSystem.score > float.Parse(dailyHighScores[0].normal.score))
                    dailyHighScores[0].normal.score = controller.scoringSystem.score.ToString();
                if (controller.scoringSystem.bestStreak > float.Parse(dailyHighScores[0].normal.streak))
                    dailyHighScores[0].normal.streak = controller.scoringSystem.bestStreak.ToString();
            }
            if (controller.diffString == "hard")
            {
                dailyHighScores[0].hard.difficulty = "hard";
                if (controller.scoringSystem.score > float.Parse(dailyHighScores[0].hard.score))
                    dailyHighScores[0].hard.score = controller.scoringSystem.score.ToString();
                if (controller.scoringSystem.bestStreak > float.Parse(dailyHighScores[0].hard.streak))
                    dailyHighScores[0].hard.streak = controller.scoringSystem.bestStreak.ToString();
            }
        }
        else
        {
            DailyHighScore newDaily = new();
            newDaily.date = DateTime.Now.Date.ToShortDateString();
            if (controller.diffString == "easy")
            {
                newDaily.easy.difficulty = "easy";
                newDaily.easy.score = controller.scoringSystem.score.ToString();
                newDaily.easy.streak = controller.scoringSystem.bestStreak.ToString();
            }
            if (controller.diffString == "normal")
            {
                newDaily.normal.difficulty = "normal";
                newDaily.normal.score = controller.scoringSystem.score.ToString();
                newDaily.normal.streak = controller.scoringSystem.bestStreak.ToString();
            }
            if (controller.diffString == "hard")
            {
                newDaily.hard.difficulty = "hard";
                newDaily.hard.score = controller.scoringSystem.score.ToString();
                newDaily.hard.streak = controller.scoringSystem.bestStreak.ToString();
            }
            dailyHighScores.Add(newDaily);
        }
        // Update overall scores
        if (controller.diffString == "easy")
        {
            int index = -1;
            for (int i = 0; i < overallHighScores.Count; i++)
                if (overallHighScores[i].difficulty == "easy")
                    index = i;

            if (index != -1)
            {
                if (controller.scoringSystem.score > float.Parse(overallHighScores[index].score))
                    overallHighScores[index].score = controller.scoringSystem.score.ToString();
                if (controller.scoringSystem.bestStreak > float.Parse(overallHighScores[index].streak))
                    overallHighScores[index].streak = controller.scoringSystem.bestStreak.ToString();
            }
            else
            {
                OverallHighScore newHighScore = new();
                newHighScore.difficulty = "easy";
                newHighScore.score = controller.scoringSystem.score.ToString();
                newHighScore.streak = controller.scoringSystem.bestStreak.ToString();
                overallHighScores.Add(newHighScore);
            }
        }
        if (controller.diffString == "normal")
        {
            int index = -1;
            for (int i = 0; i < overallHighScores.Count; i++)
                if (overallHighScores[i].difficulty == "normal")
                    index = i;

            if (index != -1)
            {
                if (controller.scoringSystem.score > float.Parse(overallHighScores[index].score))
                    overallHighScores[index].score = controller.scoringSystem.score.ToString();
                if (controller.scoringSystem.bestStreak > float.Parse(overallHighScores[index].streak))
                    overallHighScores[index].streak = controller.scoringSystem.bestStreak.ToString();
            }
            else
            {
                OverallHighScore newHighScore = new();
                newHighScore.difficulty = "normal";
                newHighScore.score = controller.scoringSystem.score.ToString();
                newHighScore.streak = controller.scoringSystem.bestStreak.ToString();
                overallHighScores.Insert(0, newHighScore);
            }
        }
        if (controller.diffString == "hard")
        {
            int index = -1;
            for (int i = 0; i < overallHighScores.Count; i++)
                if (overallHighScores[i].difficulty == "hard")
                    index = i;

            if (index != -1)
            {
                if (controller.scoringSystem.score > float.Parse(overallHighScores[index].score))
                    overallHighScores[index].score = controller.scoringSystem.score.ToString();
                if (controller.scoringSystem.bestStreak > float.Parse(overallHighScores[index].streak))
                    overallHighScores[index].streak = controller.scoringSystem.bestStreak.ToString();
            }
            else
            {
                OverallHighScore newHighScore = new();
                newHighScore.difficulty = "hard";
                newHighScore.score = controller.scoringSystem.score.ToString();
                newHighScore.streak = controller.scoringSystem.bestStreak.ToString();
                overallHighScores.Add(newHighScore);
            }
        }

        WriteData();
    }
}
