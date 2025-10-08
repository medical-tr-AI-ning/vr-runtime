using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    public class RequestTurn : ActionRequestSender
    {
        // helper
        private enum TurnType
        {
            //User,
            //AwayFromUser,
            Back,
            ExplicitAngle
        }

        [SerializeField] private TurnType TurnTo;
        [SerializeField, Range(-180, 180)] private float TurnAngle;

        protected override ActionRequest CreateActionRequest(IActionRequestStateObserver observer)
        {
            return TurnTo == TurnType.Back ? new Turn(observer, 180.0f)
                                           : new Turn(observer, TurnAngle);
        }
    }
}
