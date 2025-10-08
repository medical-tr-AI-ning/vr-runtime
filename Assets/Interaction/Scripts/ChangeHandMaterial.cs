using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.Configuration;

public class ChangeHandMaterial : MonoBehaviour
{
    [SerializeField] private Material oldMat;
    [SerializeField] private Material oldMat2;

    [SerializeField] private Material newMat;
    private AudioSource audioSrc;

    private HandMatDecider decider;

    SkinnedMeshRenderer rend;
    Valve.VR.InteractionSystem.Hand hand; 
    private bool changed = false;

    IEnumerator Start()
    {
        decider = GetComponentInParent<HandMatDecider>();
        
        if(decider == null)
        {
            yield break;
        }

        while (hand == null)
        {
            yield return new WaitForSeconds(0.3f);
            hand = GetComponent<Valve.VR.InteractionSystem.HandCollider>().hand.hand;
        }

        while (rend == null)
        {
            yield return new WaitForSeconds(0.5f);
            rend = hand.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        Material[] m = new Material[rend.materials.Length];

        for (int i = 0; i < rend.sharedMaterials.Length; i++)
        {
            if (decider == null)
            {
                Debug.Log("kein decider");
            }
            if (decider.getFirstMat())
            {
                m[i] = oldMat;
            }
            else
            {
                m[i] = oldMat2;
            }
        }

        rend.materials = m;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Glovebox") && !changed)
        {
            changed = true;
            ChangeMat();
            audioSrc = other.gameObject.GetComponent<AudioSource>();
            if(!audioSrc.isPlaying)            
            {
                audioSrc.Play();
            }
            ConfigurationContainer.Instance.GetLogger().WriteEvent("Glovebox", "was Triggered");
        }
    }

    void ChangeMat()
    {
        Material[] m = new Material[rend.materials.Length];

        for (int i = 0; i < rend.sharedMaterials.Length; i++)
        {
            m[i] = newMat;
        }

        rend.materials = m;

    }

}
