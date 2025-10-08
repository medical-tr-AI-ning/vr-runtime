using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
public class FingerDrive : LinearDrive
{
    [SerializeField]
    private FingerWidget widget;
    private Vector3 originPosition;
    public Hand.AttachmentFlags AttachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;

    override protected void Awake(){
        base.Awake();
        originPosition = transform.localPosition;
        attachmentFlags = AttachmentFlags;
    }

    protected virtual void OnAttachedToHand(Hand hand)
    {
        widget.Enabled = true;
    }

    override protected void OnDetachedFromHand(Hand hand)
    {
        base.OnDetachedFromHand(hand);
        widget.Enabled = false;
        transform.localPosition = originPosition;
        linearMapping.value = Mathf.Clamp01(CalculateLinearMapping( transform ) );
    }
}
