using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;

public class NumberCounter : MonoBehaviour
{
    private TMP_InputField _input;
    public int amountOfPictures = 5;
    void Start()
    {
        _input = GetComponent<TMP_InputField>();
    }

    public void SetAmount(int addend)
    {
        amountOfPictures += addend;
        Debug.Log(amountOfPictures);
    }

    public void UpdateInputField()
    {
        if(amountOfPictures < 0)
        {
            amountOfPictures = 0;
        }
        _input.text = amountOfPictures.ToString();
        Debug.Log(amountOfPictures);

    }
    public void UpdateAmount()
    {
        int.TryParse(_input.text, out amountOfPictures);
        {
            if(amountOfPictures < 0)
            {
                amountOfPictures = 0;
                UpdateInputField();
            }
        }
        Debug.Log(amountOfPictures);
    }

}
