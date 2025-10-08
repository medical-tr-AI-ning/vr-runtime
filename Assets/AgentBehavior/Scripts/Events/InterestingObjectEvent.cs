using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {

        /// <summary>
        /// Perception event that allows the "outer world" to inform the agent
        /// that there is a new, potentially interesting object in the scene.
        /// Currently, this will make the agent consider it as a potential look-at target.
        /// </summary>
        public class InterestingObjectEvent : PerceptionEvent
        {
            public InterestingObjectEvent(IPerceptionEventStateObserver observer, GameObject handle)
                : base(observer)
            {
                Handle = handle;
            }

            public GameObject Handle { get; }
        }
    }
}
