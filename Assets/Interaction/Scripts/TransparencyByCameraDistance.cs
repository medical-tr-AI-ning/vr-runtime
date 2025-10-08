using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyByCameraDistance : MonoBehaviour
{

    public float distanceTransparent;
    public float distanceUnTransparent;
    public float animationTimeInSec = 2f;
    public Camera cam;
    public LayerMask mask;
    public Renderer ren;
    private Material material;

    void Start(){
        material = ren.material;
    }

    void LateUpdate()
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit,Mathf.Infinity, mask)){
            setTransparency(0f);
            return;
        }
    
        if(hit.distance < distanceTransparent && hit.distance > distanceUnTransparent){
            float a = (hit.distance - distanceUnTransparent)/(distanceTransparent-distanceUnTransparent);
            setTransparency(1-a);
        } else if(hit.distance < distanceUnTransparent){
            setTransparency(1f);
        } else {
            setTransparency(0f);
        }
    }

    void setTransparency(float goal){

        float change = Time.deltaTime/animationTimeInSec, transparency = material.color.a;

        if(material.color.a > goal){
            change = -change;
            if(transparency + change < goal){
                change = goal - material.color.a;
            }
        } else if(material.color.a < goal){
            if(transparency + change > goal){
                change = goal - material.color.a;
            }
        } else if(material.color.a == goal){
            change = 0f;
        }

        if(transparency + change <= 0f){
            Color c = new Color(material.color.r,material.color.g,material.color.b,0f);
            material.color = c;
        } else if(transparency + change >= 1f){
            Color c = new Color(material.color.r,material.color.g,material.color.b,1f);
            material.color = c;
        } else {
            Color c = new Color(material.color.r,material.color.g,material.color.b,transparency + change);
            material.color = c;
        }
    }

    public void setTransparent() {
        Color c = new Color(material.color.r,material.color.g,material.color.b,0f);
        material.color = c;
    }
}
