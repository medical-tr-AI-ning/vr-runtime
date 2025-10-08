using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ContinueTimeline_Function : MonoBehaviour
{
    [SerializeField] private GameObject timeline;
    private PlayableDirector director;

    [SerializeField] private bool once = false;

    private bool played = false;

    void Start()
    {
        director = timeline.GetComponent<PlayableDirector>();
    }

    public void ContinueTheTimeline()
    {
        if(once)
        {
            if (!played)
            {
                director.Play();
                played = true;
                Debug.Log(played);
            }
        }

        else
        {
            director.Play();
            Debug.Log(played);
        }
        Debug.Log("signal gesetzt" + gameObject.name);

    }
}