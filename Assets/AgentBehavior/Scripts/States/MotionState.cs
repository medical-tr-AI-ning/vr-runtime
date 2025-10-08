using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public enum MotionState
        {
            Invalid,
            Transient,

            Standing,   // Idle
            Turning,
            Walking,
            //Sitting,
            //Lying,
        }
    }
}