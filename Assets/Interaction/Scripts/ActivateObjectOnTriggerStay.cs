using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectOnTriggerStay : MonoBehaviour
{
    [SerializeField] private new string tag;
    [SerializeField] private bool tagNeeded;

    [SerializeField] private GameObject obj;

    void Start()
    {
        obj.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {

        if (tagNeeded)
        {
            if (other.gameObject.CompareTag(tag))
            {
                obj.SetActive(true);
            }
        }

        else
        {
            obj.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (tagNeeded)
        {
            if (other.gameObject.CompareTag(tag))
            {
                obj.SetActive(false);
            }
        }
        else
        {
            obj.SetActive(false);
        }
    }
}
