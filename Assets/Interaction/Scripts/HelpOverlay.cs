using System;
using UnityEngine;

public class HelpOverlay : MonoBehaviour
{
    [SerializeField] private GameObject DesktopOverlay;
    [SerializeField] private GameObject VROverlay;

    private AudioSource _sound;
    public void Start()
    {
        _sound = GetComponentInChildren<AudioSource>();
    }

    public void SetOverlayVisible(bool visible)
    {
        if(visible) playSound();
        DesktopOverlay.SetActive(visible);
        VROverlay.SetActive(visible);
    }

    private void playSound()
    {
        if(_sound != null) _sound.Play();
    }
}
