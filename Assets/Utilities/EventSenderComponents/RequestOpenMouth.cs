using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    public class RequestOpenMouth : ActionRequestSender
    {
        protected override ActionRequest CreateActionRequest(IActionRequestStateObserver observer)
            => new OpenMouth(observer);
    }
}
