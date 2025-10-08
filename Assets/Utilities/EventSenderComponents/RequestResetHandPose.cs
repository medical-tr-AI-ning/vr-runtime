using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    public class RequestResetHandPose : ActionRequestSender
    {
        // Inspector
        [SerializeField] private BodySide BodySide = BodySide.Both;

        // interface
        protected override ActionRequest CreateActionRequest(IActionRequestStateObserver observer)
            => new ResetHandPose(observer, BodySide);
    }
}
