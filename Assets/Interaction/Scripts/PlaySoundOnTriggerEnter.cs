using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private string tag;
    [SerializeField] private bool tagNeeded;

    [SerializeField] private bool once = false;
    private bool played = false;

    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void OnTriggerEnter (Collider other)
    {
        if (!once)
        {
            if (tagNeeded)
            {
                if (other.gameObject.CompareTag(tag))
                {
                    PlaySound();
                }
            }
            else
            {
                PlaySound();
            }
        }

        else
        {
            if (!played)
            {
                if (tagNeeded)
                {
                    if (other.gameObject.CompareTag(tag))
                    {
                        PlaySound();
                        played = true;
                    }
                }
                else
                {
                    PlaySound();
                    played = true;
                }
            }
        }

    }

    private void PlaySound()
    {
        if (!source.isPlaying)
        {
            source.Play();
            Debug.Log("played sound");
        }

    }
}
