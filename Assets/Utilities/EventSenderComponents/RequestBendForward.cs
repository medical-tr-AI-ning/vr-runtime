using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    public class RequestBendForward : ActionRequestSender
    {
        protected override ActionRequest CreateActionRequest(IActionRequestStateObserver observer)
            => new BendForward(observer);
    }
}
