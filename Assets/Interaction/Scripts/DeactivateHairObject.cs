using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateHairObject : MonoBehaviour
{
    [SerializeField] private GameObject hair;
    private SkinnedMeshRenderer rend;

    void Start()
    {
        rend = hair.GetComponent<SkinnedMeshRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hand") || other.gameObject.CompareTag("DropzoneItem"))
        {
            rend.enabled = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Hand") || other.gameObject.CompareTag("DropzoneItem"))
        {
            rend.enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hand") || other.gameObject.CompareTag("DropzoneItem"))
        {
            rend.enabled = true;
        }
    }
}
