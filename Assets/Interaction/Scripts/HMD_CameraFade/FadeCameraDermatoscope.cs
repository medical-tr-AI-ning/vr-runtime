using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class FadeCameraDermatoscope : FadeCamera
{
    [SerializeField]
    private Volume volume;

    private VolumeProfile profile;
    private Exposure exposure;
    // Start is called before the first frame update
    void Start()
    {
        profile = volume.sharedProfile;
        profile.TryGet<Exposure>(out exposure);
        FadeToClear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FadeFromClear()
    {
        // fade to set color
        if(exposure != null){
            exposure.fixedExposure.overrideState = true;
        }
    }
    public override void FadeToClear()
    {
        // fade to clear view
        if(exposure != null){
            exposure.fixedExposure.overrideState = false;
        }
        
    }
}
