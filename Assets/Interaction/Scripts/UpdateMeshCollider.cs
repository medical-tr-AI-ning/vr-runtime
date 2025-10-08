using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMeshCollider : MonoBehaviour
{
    //CollisionEvents script has to be added to Rootbone

    private SkinnedMeshRenderer meshRenderer;
    private MeshCollider coll;
    //private bool isNear = false;
    private float t = 0f;

    public float waitBetweenUpdatesInSec = 5.0f;

    void Start()
    {

        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        coll = GetComponent<MeshCollider>();
    }

    void Update(){
        t += Time.deltaTime;

        if(waitBetweenUpdatesInSec < t){
            t = 0f;
            UpdateCollider();
        }
    }

    public void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        coll.sharedMesh = null;
        coll.sharedMesh = colliderMesh;
    }

}