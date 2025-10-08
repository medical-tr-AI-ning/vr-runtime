using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Vibration : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float frequency;
    [SerializeField] private float amplitude;

    [SerializeField] private bool left = true;
    [SerializeField] private bool right = true;


    public SteamVR_Action_Vibration hapticAction;

    public void Vibrate()
    {
        if (left)
        {
            hapticAction.Execute(0, duration, frequency, amplitude, SteamVR_Input_Sources.LeftHand);
        }

        if (right)
        {
            hapticAction.Execute(0, duration, frequency, amplitude, SteamVR_Input_Sources.RightHand);
        }
    }

}
