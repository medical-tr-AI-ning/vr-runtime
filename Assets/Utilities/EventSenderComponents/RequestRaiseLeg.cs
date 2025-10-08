using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    public class RequestRaiseLegPose : ActionRequestSender
    {
        // Inspector properties
        [SerializeField] private BodySide BodySide = BodySide.Left;

        // interface
        protected override ActionRequest CreateActionRequest(IActionRequestStateObserver observer)
            => new RaiseLeg(observer, BodySide, true);
    }
}