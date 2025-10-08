using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    public class RequestRaiseHandPose : ActionRequestSender
    {
        // Inspector properties
        [SerializeField] private BodySide BodySide = BodySide.Both;
        [SerializeField] private HandPose HandPose = HandPose.Forward;
        [SerializeField] private bool PalmsUp = false;

        // interface
        protected override ActionRequest CreateActionRequest(IActionRequestStateObserver observer)
            => new RaiseHand(observer, BodySide, HandPose, PalmsUp);
    }
}
    
