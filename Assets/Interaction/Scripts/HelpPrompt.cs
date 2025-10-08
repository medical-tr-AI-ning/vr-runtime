using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPrompt : MonoBehaviour
{
    [SerializeField] private GameObject helpUI;

    void Update()
    {
        if(!helpUI.activeSelf)
        {
            this.gameObject.GetComponent<Image>().enabled = false;
        }
        else
        {
            this.gameObject.GetComponent<Image>().enabled = true;
        }
    }
}
