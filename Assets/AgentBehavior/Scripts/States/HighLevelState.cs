using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        /// <summary>
        /// Describes the current high-level state of the agent.
        /// </summary>
        public enum HLState
        {
            Invalid,

            Idle,           // The agent is in idle state
            Posing,         // The agent is in some specific pose

            WalkHome,       // The agent is walking toward it's "home" position. 
            Turning,        // The agent is turning in place

            WalkToChangingRoom, // The agent is walking toward the changing room 
            ChangingClothes,    // The agent is changing the clothes.

            // WalkToBed,
            // SittingOnBed,
            // LyingOnBed
        }
    }
}