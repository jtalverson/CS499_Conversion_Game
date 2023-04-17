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
    public GameObject dailyScoreParent;   // GameObject that is parent to all daily score holders
    public GameObject overallScoreParent; // GameObject that is parent to all overall score holders
    public GameObject pageParent;         // Parent of all page objects
    public PagedRect pageController;      // PagedRect that allows me to update the Pagination

    public GameController controller;     // Overall GameController
    public TextAsset allData;             // Text file that contains all of the data
    public string[] data;                 // Current data being processed

    // Overall high score class which holds difficulty, score, and streak 
    [System.Serializable]
    public class OverallHighScore
    {
        public string difficulty = "";
        public string score = "0";
        public string streak = "0";
    }
    // Daily high score which holds the date and three overall high scores sybolizing difficulty
    [System.Serializable]
    public class DailyHighScore
    {
        public string date;
        public OverallHighScore easy = new();
        public OverallHighScore normal = new();
        public OverallHighScore hard = new();
    }
    // Lists containing overall and daily high scores
    public List<OverallHighScore> overallHighScores = new();
    public List<DailyHighScore> dailyHighScores = new();
    // Fills the pages on start
    private void Start()
    {
        FillPages();
    }
    // Clears the two lists, reads data, and populates it on the high score page
    public void FillPages()
    {
        overallHighScores.Clear();
        dailyHighScores.Clear();
        ReadData();
        StartCoroutine("PopulateData");
    }
    // Reads the data from PlayerPrefs
    public void ReadData()
    {
        Debug.Log("reading data");
        
        // Iterates over all of the high scores pushing them to the correct list
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
        // Iterates over all of the daily high scores pushing them to the correct list
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
    // Writes the data to player preferences
    public void WriteData()
    {
        // Ensures there is data to write
        if (overallHighScores.Count > 0 || dailyHighScores.Count > 0)
        {
            // Update number of overall high scores
            PlayerPrefs.SetInt("num_overalls", overallHighScores.Count);
            // Iterates over the number of high scores pushing them into the PlayerPrefs
            for (int i = 0; i < overallHighScores.Count; i++)
            {
                PlayerPrefs.SetString(String.Format("overall_{0}_difficulty", i), overallHighScores[i].difficulty);
                PlayerPrefs.SetString(String.Format("overall_{0}_score", i), overallHighScores[i].score);
                PlayerPrefs.SetString(String.Format("overall_{0}_streak", i), overallHighScores[i].streak);
            }
            // Sets stopping point so no more than 5 scores are pushed
            int stoppingPoint = Mathf.Min(dailyHighScores.Count, 5);
            // Updates number of daily high scores
            PlayerPrefs.SetInt("num_dailies", stoppingPoint);
            // Iterates until the stopping point pushing the correct data into player prefs
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
        // Disable all pages then update Pagination to reflect these changes
        foreach (Transform t in pageParent.GetComponentsInChildren<Transform>())
            if (t.name.Contains("Page"))
                t.gameObject.SetActive(false);
        pageController.UpdatePagination();

        //Daily high scores
        for (int i = 0; i < dailyHighScores.Count; i++)
        {
            // Get the current daily high score
            DailyHighScore currentScore = dailyHighScores[i];
            // Find the page associated with it
            Transform currentPage = Utility.FindObject(dailyScoreParent, "Page " + (i + 1));
            // Update the date of this object
            TextMeshProUGUI date = Utility.FindObject(currentPage.gameObject, "Date").GetComponent<TextMeshProUGUI>();
            date.text = currentScore.date;
            // Initialize strings to zero
            string easyScore = "0";
            string easyStreak = "0";
            // If the easy difficulty has scores in it update the values
            if (dailyHighScores[i].easy.difficulty != "")
            {
                easyScore = dailyHighScores[i].easy.score;
                easyStreak = dailyHighScores[i].easy.streak;
            }
            // Find the easy object and its score and streak children
            Transform easy = Utility.FindObject(currentPage.gameObject, "Easy");
            TextMeshProUGUI score = Utility.FindObject(easy.gameObject, "Score").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI streak = Utility.FindObject(easy.gameObject, "Highest Streak").GetComponent<TextMeshProUGUI>();
            // Update the text in them
            score.text = "Score:\n" + easyScore;
            streak.text = "Best Streak:\n" + easyStreak;

            // FOLLOW A SIMILAR PROCEDURE FOR NORMAL AND HARD DIFFICULTIES

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

            // Enable the page in the hierarchy
            if (!currentPage.gameObject.activeInHierarchy)
                currentPage.gameObject.SetActive(true);
        }
        // Wait for .1 seconds then update pagination to reflect changes
        yield return new WaitForSeconds(.1f);
        pageController.UpdatePagination();

        // Sometimes the overall High Score objects remain populated even after a save data deletion
        // this ensures that unless there are actual scores to be reported, the objects will say 0
        if (overallHighScores.Count == 0)
        {
            GameObject easyBox = FindObject.Find_GameObject(overallScoreParent, "Easy");
            GameObject normalBox = FindObject.Find_GameObject(overallScoreParent, "Medium");
            GameObject hardBox = FindObject.Find_GameObject(overallScoreParent, "Hard");
            if (easyBox != null)
            {
                TextMeshProUGUI score = Utility.FindObject(easyBox, "Score").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI streak = Utility.FindObject(easyBox, "Highest Streak").GetComponent<TextMeshProUGUI>();
                score.text = "Score:\n0";
                streak.text = "Best Streak:\n0";
            }
            if (normalBox != null)
            {
                TextMeshProUGUI score = Utility.FindObject(normalBox, "Score").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI streak = Utility.FindObject(normalBox, "Highest Streak").GetComponent<TextMeshProUGUI>();
                score.text = "Score:\n0";
                streak.text = "Best Streak:\n0";
            }
            if (hardBox != null)
            {
                TextMeshProUGUI score = Utility.FindObject(hardBox, "Score").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI streak = Utility.FindObject(hardBox, "Highest Streak").GetComponent<TextMeshProUGUI>();
                score.text = "Score:\n0";
                streak.text = "Best Streak:\n0";
            }


        }
        // Iterate over all of the overallHighScores
        for (int i = 0; i < overallHighScores.Count; i++)
        {
            // Establish the current transform
            Transform current = null;
            // Check the difficulty and assign current appropriately
            if (overallHighScores[i].difficulty == "easy")
                current = Utility.FindObject(overallScoreParent, "Easy");
            if (overallHighScores[i].difficulty == "normal")
                current = Utility.FindObject(overallScoreParent, "Medium");
            if (overallHighScores[i].difficulty == "hard")
                current = Utility.FindObject(overallScoreParent, "Hard");
            // If current is not null populate it correctly
            if (current != null)
            {
                TextMeshProUGUI score = Utility.FindObject(current.gameObject, "Score").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI streak = Utility.FindObject(current.gameObject, "Highest Streak").GetComponent<TextMeshProUGUI>();
                score.text = "Score:\n" + overallHighScores[i].score;
                streak.text = "Best Streak:\n" + overallHighScores[i].streak;
            }
        }
    }
    // Updates the lists when new scores are present
    public void UpdateLists()
    {
        // Update today's data
        if (dailyHighScores.Count > 0 && dailyHighScores[0].date == DateTime.Now.Date.ToShortDateString())
        {
            // If the difficulty is easy
            if (controller.diffString == "easy")
            {
                // Make the difficulty string easy instead of the empty string in case this is the first time playing this difficulty
                dailyHighScores[0].easy.difficulty = "easy";
                // Check if score is higher then update it
                if (controller.scoringSystem.score > float.Parse(dailyHighScores[0].easy.score))
                    dailyHighScores[0].easy.score = controller.scoringSystem.score.ToString();
                // Check if streak is higher then update it
                if (controller.scoringSystem.bestStreak > float.Parse(dailyHighScores[0].easy.streak))
                    dailyHighScores[0].easy.streak = controller.scoringSystem.bestStreak.ToString();
            }
            // REPEAT PROCESS FOR NORMAL AND HARD
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
        // Otherwise this is the first time we have played today
        else
        {
            // Create a new daily high score
            DailyHighScore newDaily = new();
            // Set the date
            newDaily.date = DateTime.Now.Date.ToShortDateString();
            // Depending on the difficulty set the values appropriately
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
            // Insert this new dailt score into the list
            dailyHighScores.Insert(0, newDaily);
        }
        // Update overall scores
        if (controller.diffString == "easy") // If easy is the current difficulty
        {
            // Create a search index
            int index = -1;
            // Look for it in the list updating index if found
            for (int i = 0; i < overallHighScores.Count; i++)
                if (overallHighScores[i].difficulty == "easy")
                    index = i;
            // If we have found it, check to see if the score or streak is any better and update if so
            if (index != -1)
            {
                if (controller.scoringSystem.score > float.Parse(overallHighScores[index].score))
                    overallHighScores[index].score = controller.scoringSystem.score.ToString();
                if (controller.scoringSystem.bestStreak > float.Parse(overallHighScores[index].streak))
                    overallHighScores[index].streak = controller.scoringSystem.bestStreak.ToString();
            }
            // Otherwise create a new OverallHighScore
            else
            {
                OverallHighScore newHighScore = new();
                // Populate all fields correctly and append to the list
                newHighScore.difficulty = "easy";
                newHighScore.score = controller.scoringSystem.score.ToString();
                newHighScore.streak = controller.scoringSystem.bestStreak.ToString();
                overallHighScores.Add(newHighScore);
            }
        }
        // FOLLOW THE SAME PROCEDURE FOR NORMAL AND HARD
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
        // Write the updated data to PlayerPrefs
        WriteData();
    }
    public void DeleteSaveData()
    {
        // create tmp gameobject with find gameobject function
        // DELETES ALL SAVE DATA FROM PLAYERPREFS
        Debug.Log("Save Data deleted");
        PlayerPrefs.DeleteAll();
    }
}
