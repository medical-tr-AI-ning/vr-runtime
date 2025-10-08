using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class PoseTransitionBehavior : AnimatorBehavior
        {

            // Inspector properties
            [SerializeField] protected bool IsTransitionState = false;
            [SerializeField][ReadOnly] private IPoseCapability Capability = null;

            // Interface
            public override MotionController Controller
            {
                get { return Capability as MotionController; }
                set { Capability = value as IPoseCapability; }
            }

            override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                if (Capability != null)
                    Capability.SetInTransientPose(layerIndex, IsTransitionState);
            }
        }
    }
}


