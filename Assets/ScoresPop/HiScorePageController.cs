using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using TMPro;
using ExternalUI.Pagination;

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
        Debug.Log("reading data");
        

        for (int i = 0; i < PlayerPrefs.GetInt("num_overalls"); i++)
        {
            OverallHighScore currentHighScore = new();
            currentHighScore.difficulty = PlayerPrefs.GetString(
                String.Format("overall_{0}_difficulty", i));
            currentHighScore.score = PlayerPrefs.GetString(
                String.Format("overall_{0}_score", i));
            currentHighScore.streak = PlayerPrefs.GetString(
                String.Format("overall_{0}_streak", i));
            overallHighScores.Add(currentHighScore);
        }

        for (int i = 0; i < PlayerPrefs.GetInt("num_dailies"); i++)
        {
            DailyHighScore currentDailyScore = new();
            currentDailyScore.date = PlayerPrefs.GetString(
                String.Format("daily_{0}_datetime", i));

            currentDailyScore.easy.difficulty = "easy";
            currentDailyScore.easy.score = PlayerPrefs.GetString(
                String.Format("daily_{0}_easy_score", i));
            currentDailyScore.easy.streak = PlayerPrefs.GetString(
                String.Format("daily_{0}_easy_streak", i));


            currentDailyScore.normal.difficulty = "normal";
            currentDailyScore.normal.score = PlayerPrefs.GetString(
                String.Format("daily_{0}_normal_score", i));
            currentDailyScore.normal.streak = PlayerPrefs.GetString(
                String.Format("daily_{0}_normal_streak", i));


            currentDailyScore.hard.difficulty = "hard";
            currentDailyScore.hard.score = PlayerPrefs.GetString(
                String.Format("daily_{0}_hard_score", i));
            currentDailyScore.hard.streak = PlayerPrefs.GetString(
                String.Format("daily_{0}_hard_streak", i));
            dailyHighScores.Add(currentDailyScore);
        }
    }

    public void StripLastChar(string[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
            array[i] = array[i].Remove(array[i].Length - 1);
    }

    public void WriteData()
    {
        if (overallHighScores.Count > 0 || dailyHighScores.Count > 0)
        {
            PlayerPrefs.SetInt("num_overalls", overallHighScores.Count);
            for (int i = 0; i < overallHighScores.Count; i++)
            {
                PlayerPrefs.SetString(String.Format("overall_{0}_difficulty", i), overallHighScores[i].difficulty);
                PlayerPrefs.SetString(String.Format("overall_{0}_score", i), overallHighScores[i].score);
                PlayerPrefs.SetString(String.Format("overall_{0}_streak", i), overallHighScores[i].streak);
            }
            int stoppingPoint = Mathf.Min(dailyHighScores.Count, 5);
            PlayerPrefs.SetInt("num_dailies", stoppingPoint);
            for (int i = 0; i < stoppingPoint; i++)
            {
                PlayerPrefs.SetString(String.Format("daily_{0}_datetime", i), dailyHighScores[i].date);
                PlayerPrefs.SetString(String.Format("daily_{0}_easy_score", i), dailyHighScores[i].easy.score);
                PlayerPrefs.SetString(String.Format("daily_{0}_easy_streak", i), dailyHighScores[i].easy.streak);
                PlayerPrefs.SetString(String.Format("daily_{0}_normal_score", i), dailyHighScores[i].normal.score);
                PlayerPrefs.SetString(String.Format("daily_{0}_normal_streak", i), dailyHighScores[i].normal.streak);
                PlayerPrefs.SetString(String.Format("daily_{0}_hard_score", i), dailyHighScores[i].hard.score);
                PlayerPrefs.SetString(String.Format("daily_{0}_hard_streak", i), dailyHighScores[i].hard.streak);
            }
        }
        PlayerPrefs.Save();
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
        if (dailyHighScores.Count > 0 && dailyHighScores[0].date == DateTime.Now.Date.ToShortDateString())
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
