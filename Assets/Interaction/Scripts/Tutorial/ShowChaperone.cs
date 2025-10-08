using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve;

public class ShowChaperone : MonoBehaviour
{
    void Start()
    {
        Show();
    }
    
    public void Show()
    {
        Valve.VR.OpenVR.Chaperone.ForceBoundsVisible(true);
    }

    public void Hide()
    {
        Valve.VR.OpenVR.Chaperone.ForceBoundsVisible(false);
    }
}
