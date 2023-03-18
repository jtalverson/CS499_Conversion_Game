using UnityEngine;

public class Utility : MonoBehaviour
{
    public static Transform FindObject(GameObject parent, string childName)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == childName)
                return t;
        }
        return null;
    }
}
