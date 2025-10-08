// Purpose: Custom Throwable for Dropzones

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System.Linq;



//-------------------------------------------------------------------------
[RequireComponent( typeof( Valve.VR.InteractionSystem.Interactable ) )]
[RequireComponent( typeof( Rigidbody ) )]
public class ThrowableCustom : Valve.VR.InteractionSystem.Throwable
{
    
    public Material transparent;
    public SteamVR_Action_Boolean actionObject = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabGrip");
    private Material[][] mats;
    private Renderer[] ren;
    public GameObject connectedRenderers;
    private bool inDropzone = false;

    public UnityEvent onOpenHand;
    public UnityEvent onCloseHand;

    void Start() {
        if(connectedRenderers){
            ren = gameObject.GetComponentsInChildren<Renderer>().Concat(connectedRenderers.GetComponentsInChildren<Renderer>()).ToArray();
        } else {
            ren = gameObject.GetComponentsInChildren<Renderer>();
        }
        mats = new Material[ren.Length][];
        for(int i = 0; i< ren.Length; i++){

            mats[i] = ren[i].materials;
        }
    }

    // Set ReleaseVelocity to 0 for Objects to land in the Dropzone
    public override void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
    { 
        velocity = new Vector3(0,0,0);
        angularVelocity = new Vector3(0,0,0);
        return;
    }

    // Only detach from Hand when inside of Dropzone
    protected override void OnDetachedFromHand(Hand hand){
        if(inDropzone){
            setOriginal();
            hand.Show();
            base.OnDetachedFromHand(hand);
        }
    }

    void Update() {
        if(interactable.attachedToHand == null){
            return;
        }

        Hand hand = interactable.attachedToHand;

        if (actionObject.GetState(hand.handType))
        {
            setOriginal();
            hand.Hide();
        } else {
            if(!inDropzone) {
                setTransparent();
                hand.Show();
            }
        }

    }
    
    protected override void HandAttachedUpdate(Hand hand)
    {
        if(inDropzone){
            base.HandAttachedUpdate(hand);
        } 
    }

    // Is called by the Dropzone when Entering or Exiting the Dropzone
    public void setInDropzone(bool isInside){
        inDropzone = isInside;    
    }

    private void setTransparent(){
        for(int i = 0; i< ren.Length; i++){
            Material[] transparents = new Material[ren[i].materials.Length];
            for(int j = 0; j < ren[i].materials.Length; j++ ){
                transparents[j] = transparent;
            }
            ren[i].materials = transparents;
        }
        Dropzone.dropZoneVisible.Invoke(this.tag);
        onOpenHand.Invoke();
    }

    private void setOriginal(){
        for(int i = 0; i< ren.Length; i++){
            ren[i].materials = mats[i];
        }
        Dropzone.dropZoneTransparent.Invoke(this.tag);
        onCloseHand.Invoke();
    }

        protected override void OnAttachedToHand(Hand hand)
        {
            if(enabled) base.OnAttachedToHand(hand);
        }

        protected override void OnHandHoverBegin(Hand hand)
        {
            if(enabled) base.OnHandHoverBegin(hand);
        }

        protected override void HandHoverUpdate(Hand hand)
        {
            if(enabled) base.HandHoverUpdate(hand);
        }
}


