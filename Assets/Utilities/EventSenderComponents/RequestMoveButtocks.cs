using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    public class RequestMoveButtocks : ActionRequestSender
    {
        // helper
        public enum ButtockState
        {
            Spread,
            Release
        };

        // Inspector properties
        [SerializeField] private ButtockState State = ButtockState.Release;

        // interface
        protected override ActionRequest CreateActionRequest(IActionRequestStateObserver observer)
            => new MoveButtocks(observer, State == ButtockState.Spread, true);
    }
}
