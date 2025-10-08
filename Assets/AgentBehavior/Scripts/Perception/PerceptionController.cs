using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public abstract class PerceptionController
        {
            // refresh all objects, etc.
            // process all notifications, perception events, etc.
            public abstract void Update(IPerceptionModel model);

            // gather or process the InteractionEvent-s 
            public abstract void EnqueueEvent(InteractionEvent interactionEvent);
        }
    }
}
