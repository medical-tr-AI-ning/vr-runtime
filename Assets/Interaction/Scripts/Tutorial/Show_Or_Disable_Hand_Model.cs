using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Show_Or_Disable_Hand_Model : MonoBehaviour
{
    private Valve.VR.InteractionSystem.RenderModel[] hands;

    IEnumerator Start()
    {
        while (hands == null)
        {
            yield return new WaitForSeconds(1);
            hands = FindObjectsOfType<Valve.VR.InteractionSystem.RenderModel>();
        }
    }

    public void ShowHand()
    {
        foreach(Valve.VR.InteractionSystem.RenderModel hand in hands)
        {
            hand.gameObject.SetActive(true);
        }
    }

    public void DisableHand()
    {
        foreach (Valve.VR.InteractionSystem.RenderModel hand in hands)
        {
            hand.gameObject.SetActive(false);
        }
    }
}
