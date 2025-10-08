using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject teleportLocation;

    [SerializeField] private string collisionTag;

    //[SerializeField] private GameObject timeline;
    //private PlayableDirector director;

    private bool played = false;

    /*
    void Start()
    {
        director = timeline.GetComponent<PlayableDirector>();
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(collisionTag) && !played)
        {
            //director.Play();
            played = true;
            Debug.Log(played);
            Teleport();

        }
    }

    private void Teleport()
        {
            player.transform.position = teleportLocation.transform.position;
        }
}
