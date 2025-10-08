using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LookAtRaycast : MonoBehaviour
{
    //public LayerMask mask;

    [SerializeField] private GameObject timeline;
    [SerializeField] private GameObject obj;
    private PlayableDirector director;

    private bool played = false;

    void Start()
    {
        director = timeline.GetComponent<PlayableDirector>();
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, Mathf.Infinity) && !played)
        {
            //Debug.Log(hit);
            if (hit.collider.gameObject == obj)
            {
                Debug.Log("you have turned successfully");
                director.Play();
                played = true;
            }
        }
    }

}