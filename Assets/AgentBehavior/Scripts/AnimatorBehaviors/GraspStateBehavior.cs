using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class GraspStateBehavior : AnimatorBehavior
        {
            // Inspector properties
            [SerializeField] protected GraspingState TrackedState;
            [SerializeField] protected GraspingState ExitState = GraspingState.Invalid;
            [SerializeField][ReadOnly] private IGraspCapability Capability = null;

            // Interface
            public override MotionController Controller { 
                get { return Capability as MotionController; } 
                set { Capability = value as IGraspCapability; } 
            }

            public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                if (Capability != null)
                    Capability.GraspingState = TrackedState;
            }

            public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                if (Capability != null)
                    Capability.GraspingState = TrackedState;
            }

            public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                if (Capability != null)
                    Capability.GraspingState = ExitState;
            }
        }
    }
}
