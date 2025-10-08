// https://discussions.unity.com/t/how-to-render-severals-cameras-at-differents-frame-rate/149550/3

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class setCameraFPS : MonoBehaviour
{

    public float FPS = 5f;
    public Camera renderCam;
    // Use this for initialization
    void Start () {
        InvokeRepeating ("Render", 0f, 1f / FPS);
    }
    void OnDestroy(){
        //CancelInvoke ();
    }

    void Render(){
        renderCam.enabled = true;

    }
    void OnPostRender(){
        renderCam.enabled = false;
    }
}
