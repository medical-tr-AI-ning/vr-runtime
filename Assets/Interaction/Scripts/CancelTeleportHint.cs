using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CancelTeleportHint : MonoBehaviour
{
    //removes the teleportation tooltip, as seen here: https://steamcommunity.com/app/250820/discussions/0/141136086925011504/

    void Update()
    {
        Teleport.instance.CancelTeleportHint();
    }
}