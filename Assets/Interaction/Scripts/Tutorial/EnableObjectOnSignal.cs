using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectOnSignal : MonoBehaviour
{
    [SerializeField] private GameObject obj;

    public void EnableGameobject()
    {
        obj.SetActive(true);
    }

    public void DisableGameobject()
    {
        obj.SetActive(false);
    }
}
