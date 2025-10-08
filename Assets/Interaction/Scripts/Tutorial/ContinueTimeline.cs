using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ContinueTimeline : MonoBehaviour
{
    [SerializeField] private string collisionTag;

    [SerializeField] private GameObject timeline;
    private PlayableDirector director;

    private bool played = false;

    void Start()
    {
        director = timeline.GetComponent<PlayableDirector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(collisionTag) && !played)
        {
            director.Play();
            played = true;
            //Debug.Log(played + gameObject.name);
        }
    }
}