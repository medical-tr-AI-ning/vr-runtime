using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject Follower;

    Vector3 startPosition;

    void Start(){
        startPosition = transform.position - Follower.transform.position;
    }

    void LateUpdate()
    {


        Follower.transform.position = transform.position + startPosition;
        Follower.transform.rotation = transform.rotation;
    }
}
