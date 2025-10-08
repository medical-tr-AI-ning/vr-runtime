using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public enum CustomTagProperty
        {
            // ...
            None,

            // generic
            InterestingObject,
            Clock,

            // agents
            Agent,
            InteractiveAgent,
            Patient,

            // specialized
            ChangingRoom,
            HomePosition,

            LeftThigAnchor,
            RightThigAnchor
        }

        public class CustomPropertyList : MonoBehaviour
        {
            // Inspector            
            public List<CustomTagProperty> Tags;

            // TODO: dv: these are properties of the agent's perception model
            //           and not of the object itself!
            [Range(0, 1)] public float LevelOfInterest = 0.0f;
            //[Range(0, 1)] public float InterestDecaySpeed = 0.0f;

            // ...
            protected virtual void Start() 
            { 
            }

            protected virtual void Update() 
            { 
            }

            // Utilities
            public virtual void EnsureTag(CustomTagProperty tag) 
            {
                if (!Tags.Contains(tag))
                    Tags.Add(tag);
            }

            public virtual void EnsureTagRemoved(CustomTagProperty tag)
            {
                while (Tags.Contains(tag))
                    Tags.Remove(tag);
            }
        }
    }
}
