using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using MedicalTraining.AgentBehavior;
using MedicalTraining.Utilities;

namespace MedicalTraining
{
    public class ThrowableIK : Valve.VR.InteractionSystem.Throwable
    {
        
        [Range(0f, 1f)]
        public float attachSpeed = 1f;
        [Range(0f, 1f)]
        public float detachSpeed = 1f;
        Vector3 originPosition;
        Quaternion originRotation;
        public FingerWidget widget;
        
        override protected void Awake(){
            base.Awake();
            originPosition = transform.localPosition;
            originRotation = transform.rotation;
        }

        override protected void OnAttachedToHand( Hand hand ){
            base.OnAttachedToHand(hand);
        }

        override protected void HandAttachedUpdate(Hand hand){ 
            if(widget != null){
                widget.Enabled = true;
            }
            base.HandAttachedUpdate(hand);    
        }

        override protected void OnDetachedFromHand(Hand hand){
            if(widget!=null) {
                widget.Enabled = false;
                transform.localPosition = originPosition;
                transform.rotation = originRotation;
            }
            base.OnDetachedFromHand(hand);
        }

        void Reset(){
            attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic;
            restoreOriginalParent = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}