using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using MedicalTraining.AgentBehavior;
using MedicalTraining.Utilities;

namespace MedicalTraining
{
    [RequireComponent( typeof( Valve.VR.InteractionSystem.Interactable ) )]
    public class InteractableIK : MonoBehaviour
{       
        protected Valve.VR.InteractionSystem.Interactable interactable;
        [Range(0f, 1f)]
        public float attachSpeed = 1f;
        [Range(0f, 1f)]
        public float detachSpeed = 1f;
        public bool returnOnDetach = true;
        ArmatureHandle handle;
        public List<ArmatureHandle> disabledTarget;
        protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;
        bool inAnimation = false;
        Hand grabHand;
        Quaternion originalHandRotation;
        Quaternion originalTargetRotation;
        
        protected virtual void Awake()
        {
            interactable = GetComponent<Valve.VR.InteractionSystem.Interactable>();
            handle = gameObject.GetComponent<ArmatureHandle>();
            ActionAdapter.Instance.OnActionStarted += startAnimation;
            ActionAdapter.Instance.OnActionStopped += stopAnimation;
        }

        void Update(){
            if(grabHand && handle){
                handle.SetPosition(grabHand.transform.position);
                handle.SetRotation(grabHand.transform.rotation * originalHandRotation * originalTargetRotation);
            }
        }

        protected virtual void HandHoverUpdate( Hand hand )
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (!inAnimation && hand.AttachedObjects.Count == 0 && interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
            {
                hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
                if(handle!=null){
                    foreach(ArmatureHandle h in disabledTarget){
                        h.Disable(detachSpeed);
                    }
                    handle.enabled = true;
                    grabHand = hand;
                    originalTargetRotation = transform.rotation;
                    originalHandRotation = Quaternion.Inverse(grabHand.transform.rotation);
                    handle.Enable(attachSpeed);
                }
            }
		}

        protected virtual void HandAttachedUpdate(Hand hand)
        {
            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject);
                if(handle != null) {
                    if(!returnOnDetach){
                        handle.SetTarget(transform);
                    } else {
                        disableHandle();
                    }   
                }
            }
        }

        private void startAnimation(){
            inAnimation = true;
            disableHandle();
        }

        private void stopAnimation(){
            inAnimation = false;
        }

        private void disableHandle(){
            handle.Disable(detachSpeed);
            grabHand = null;
        }

        void OnDestroy(){
            ActionAdapter.Instance.OnActionStarted -= startAnimation;
            ActionAdapter.Instance.OnActionStopped -= stopAnimation;
        }
    }
}
