using UnityEngine;

public static class FindExtensions
{
    public static GameObject FindObject(this GameObject parent, string name)
    {
        Transform[] childTransforms = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in childTransforms)
        {
            if (transform.name == name)
            {
                return transform.gameObject;
            }
        }
        return null;
    }
}
