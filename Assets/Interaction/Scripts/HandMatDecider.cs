using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.Configuration;
using MedicalTraining.Logger;

public class HandMatDecider : MonoBehaviour
{
    private bool firstMat = true;

    private string studiencode;
    private string name;
    private GameObject config;
    private ConfigurationContainer con;
    private int seed;

    private float count = 0;
    private float odd = 0;
    private float even = 0;

    private SimulationLogger logger = null;
    /*
    void Update()
    {
        string st = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int day = Random.Range(01, 31);
        studiencode = st[Random.Range(0, st.Length)].ToString() + st[Random.Range(0, st.Length)].ToString() + st[Random.Range(0, st.Length)].ToString() + st[Random.Range(0, st.Length)].ToString() + day.ToString("D2");

        //Debug.Log(studiencode);

        SeedFromString();
        if (seed % 2 == 0)
        {
            even += 1;
        }
        else
        {
            odd += 1;
        }

        count += 1;

        if (count % 100 == 0)
        {
            Debug.Log("Odd: " + odd / count);
        }
    }
    */

    void Start()
    {
        {
            StartCoroutine(LateStart(5f));
        }
        ConfigurationContainer.Instance.GetStudentData(out name, out studiencode);
        Debug.Log(studiencode);

        SeedFromString();

        if (seed % 2 == 0)
        {
            firstMat = true;
            Debug.Log("Green Hands");
        }
        else
        {
            firstMat = false;
            Debug.Log("Realistic Hands");
        }
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        logger = ConfigurationContainer.Instance.GetLogger();

        if(firstMat)
        {
            logger.WriteEvent("handColor", "Green Hands");
        }
        else
        {
            logger.WriteEvent("handColor", "Realistic Hands");

        }
    }

    private void SeedFromString()
    {
        //wird durch 10 geteilt denn sonst sind Hashcodes abwechselnd ungerade und gerade
        seed = studiencode.GetHashCode() / 10;
        if(seed < 0)
        {
            seed = seed * -1;
        }
    }

    public bool getFirstMat()
    {
        return firstMat;
    }
}