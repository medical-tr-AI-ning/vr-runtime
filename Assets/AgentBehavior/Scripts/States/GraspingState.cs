using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public enum GraspingState
        {
            Invalid,
            Idle,
            WaitingForTargetReachable,
            Grasping,
            Grasped,
            Retracing
        };
    }
}