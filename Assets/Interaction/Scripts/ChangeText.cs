using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;
using UnityEngine.UI;

public class ChangeText : MonoBehaviour
{
    private Text txt;
    //private TextMeshProUGUI highlightText;
    [SerializeField] private string highlightName = "empty";

    void Start()
    {
        txt = GetComponentInChildren<Text>();
        txt.text = highlightName;
        //highlightText = GetComponentInChildren<TextMeshProUGUI>();
        //highlightText.text = highlightName;
        txt.gameObject.SetActive(false);
    }
}
