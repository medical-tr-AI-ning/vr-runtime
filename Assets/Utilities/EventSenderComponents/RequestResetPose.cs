using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;
using UnityEngine.Serialization;

namespace MedicalTraining.Utilities
{
    public class RequestResetPose : ActionRequestSender
    {
        [SerializeField] private bool resetBodyPose = true;
        [SerializeField] private bool resetHandPose = true;
        [SerializeField] private bool resetLegsPose = true;

        protected override ActionRequest CreateActionRequest(IActionRequestStateObserver observer)
            => new ResetPose(resetBodyPose: resetBodyPose, resetHandPose: resetHandPose, resetLegsPose: resetLegsPose,
                observer: observer);
    }
}