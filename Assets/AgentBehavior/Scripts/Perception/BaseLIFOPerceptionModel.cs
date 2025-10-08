using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class BaseLIFOPerceptionModel : IPerceptionModel
        {
            public virtual bool HasEvents
                => _events.Count > 0;

            public virtual InteractionEvent DequeueEvent()
                => HasEvents ? _events.Dequeue() : null;

            public virtual void EnqueueEvent(InteractionEvent evt)
                => _events.Enqueue(evt);

            public virtual IEnumerable<GameObject> ObjectsWithTag(CustomTagProperty tag)
            {
                List<GameObject> result = new();
                foreach (GameObject obj in Objects) 
                {
                    CustomPropertyList props = obj.GetComponent<CustomPropertyList>();
                    if (props && props.Tags.Contains(tag))
                        result.Add(obj);
                }
                return result;
            }

            public virtual GameObject User { get; set; } = null;
            public virtual float UserLevelOfInterest { get; set; } = 0.0f;

            public virtual List<GameObject> Agents { get; } = new List<GameObject>();
            public virtual List<GameObject> Objects { get; } = new List<GameObject>();

            // private members
            private readonly Queue<InteractionEvent> _events = new();
        }
    }
}