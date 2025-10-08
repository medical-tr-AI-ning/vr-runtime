using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Dropzone : MonoBehaviour
{

    public class StringEvent : UnityEvent<string> {}
    public static StringEvent dropZoneTransparent;
    public static StringEvent dropZoneVisible;

    private MeshRenderer renderer;
    public List<string> tagWhiteList;
    List<GameObject> objects = new List<GameObject>();


    void Start()
    {
        if(dropZoneTransparent == null){
            dropZoneTransparent = new StringEvent();
        }

        if(dropZoneVisible == null){
            dropZoneVisible = new StringEvent();
        }

        renderer = GetComponent<MeshRenderer>();
        setTransparent(tagWhiteList[0]);

        dropZoneTransparent.AddListener(setTransparent);
        dropZoneVisible.AddListener(setVisible);
    }

    void OnDestroy(){
        dropZoneTransparent.RemoveListener(setTransparent);
        dropZoneVisible.RemoveListener(setVisible);
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.attachedRigidbody.gameObject.GetComponent<ThrowableCustom>() != null && tagWhiteList.Contains(col.tag)){
            col.attachedRigidbody.gameObject.GetComponent<ThrowableCustom>().setInDropzone(false);
        }
    }

    void OnTriggerEnter(Collider col) 
    {
        if (col.gameObject.GetComponent<ThrowableCustom>() != null && !objects.Contains(col.gameObject) && tagWhiteList.Contains(col.tag)) 
        {
            objects.Add(col.attachedRigidbody.gameObject); 
            col.attachedRigidbody.gameObject.GetComponent<ThrowableCustom>().setInDropzone(true);
        }
    }
 
    void OnTriggerStay(Collider col) {
        OnTriggerEnter(col);
    }

    void FixedUpdate() {
        bool active = false;
        foreach(GameObject obj in objects){
            if(obj.GetComponent<Valve.VR.InteractionSystem.Interactable>().attachedToHand != null){
                active = true;
            }
        }
        
    /*    if(active){
            Material[] materials = renderer.materials;
            materials[0] = selected;
            renderer.materials = materials;
        } else {
            Material[] materials = renderer.materials;
            materials[0] = unselected;
            renderer.materials = materials;
        }*/
        objects.Clear();
    }

    private void setVisible(string tag){
        if(tagWhiteList.Contains(tag))
            renderer.enabled = true;
    }

    private void setTransparent(string tag){
        if(tagWhiteList.Contains(tag))
            renderer.enabled = false;
    }
}
