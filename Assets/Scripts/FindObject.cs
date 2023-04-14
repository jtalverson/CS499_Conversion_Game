using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FindObject : MonoBehaviour
{
    public GameObject EasyAllTime;
    public GameObject NormalAllTime;
    public GameObject HardAllTime;
    public static Transform Find_Object(GameObject parent, string childName)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == childName)
                return t;
        }
        return null;
    }
    public static GameObject Find_GameObject(GameObject parent, string childName)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == childName)
                return t.gameObject;
        }
        return null;
    }
    public void DeleteSaveData()
    {
        // DELETES ALL SAVE DATA FROM PLAYERPREFS
        Debug.Log("Save Data deleted");
        PlayerPrefs.DeleteAll();
        // Reset high score screen
        Find_GameObject(EasyAllTime, "Score").GetComponent<TextMeshProUGUI>().text = "Score:\n0";
        Find_GameObject(EasyAllTime, "Highest Streak").GetComponent<TextMeshProUGUI>().text = "Best Streak:\n0";
        Find_GameObject(NormalAllTime, "Score").GetComponent<TextMeshProUGUI>().text = "Score:\n0";
        Find_GameObject(NormalAllTime, "Highest Streak").GetComponent<TextMeshProUGUI>().text = "Best Streak:\n0";
        Find_GameObject(HardAllTime, "Score").GetComponent<TextMeshProUGUI>().text = "Score:\n0";
        Find_GameObject(HardAllTime, "Highest Streak").GetComponent<TextMeshProUGUI>().text = "Best Streak:\n0";
    }
}
