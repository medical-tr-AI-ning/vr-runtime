using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Ignore Spelling: Collider
namespace MedicalTraining
{
    namespace AgentBehavior
    {
        // Base interface for all MotionController-s.
        // The actual motions are implemented as mix-ins by the I-xxx-Capability interfaces
        
        // TODO: dv: (low-priority) It may worth considering to include the motion controller
        // (or the capabilities) in the AgentComponent. Thus the AgentComponent will encapsulate
        // all low level actions and the DMS would only care for high-level states and sequencing...
        // Don't care about it right now. 
        public abstract class MotionController
        {
            public MotionController(Animator animator)
                => Animator = animator;

            public virtual Animator Animator { get; private set; }
            public virtual Transform Transform => Animator.transform;
            public virtual MotionState MotionState { get; set; } = MotionState.Invalid;
            
            public abstract void Update();

            // TODO: dv: still not sure about this part of the interface...
            // subject to change :)
            public virtual void OnControllerColliderHit(ControllerColliderHit hit) { }
            
            public virtual void OnTriggerEnter(Collider collider) { }

            public virtual void OnTriggerExit(Collider collider) { }

            public virtual void OnAnimatorIK() { }
        }
    }
}
