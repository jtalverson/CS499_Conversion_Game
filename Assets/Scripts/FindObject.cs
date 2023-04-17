using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FindObject : MonoBehaviour
{
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
}
