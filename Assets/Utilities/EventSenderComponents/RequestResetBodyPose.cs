using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    public class RequestResetBodyPose : ActionRequestSender
    {
        // interface
        protected override ActionRequest CreateActionRequest(IActionRequestStateObserver observer)
            => new ResetBodyPose(observer);
    }
}