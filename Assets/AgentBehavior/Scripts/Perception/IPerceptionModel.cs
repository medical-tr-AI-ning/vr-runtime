using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {       
        // TODO: dv: level of interest, interest-decay, etc. should be part
        //           of this model, not in the property lists in the scene objects.
        public interface IPerceptionModel
        {
            public abstract bool HasEvents {  get; }

            public abstract void EnqueueEvent(InteractionEvent interactionEvent);

            public abstract InteractionEvent DequeueEvent();

            public abstract IEnumerable<GameObject> ObjectsWithTag(CustomTagProperty property);

            public abstract GameObject User { get; set; }

            public abstract float UserLevelOfInterest { get; set; } // TODO: dv: fix this!

            // TODO: dv: maybe change this to AgentComponent
            public abstract List<GameObject> Agents { get; }

            // TODO: dv: maybe change this to CustomPropertyList
            public abstract List<GameObject> Objects { get; } 

            // TODO: dv: add sound sources, etc.
        }
    }
}
