using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dermatoscope_Scale : MonoBehaviour
{
    private RaycastHit hit;
    [SerializeField] private GameObject scaleObj;
    [SerializeField] private LayerMask agentLayerMask;

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, agentLayerMask))
        {
            scaleObj.SetActive(true); 
            scaleObj.transform.position = hit.point;
        }
        else
        {
            scaleObj.SetActive(false);
        }
    }
}