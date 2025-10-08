using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public abstract class AnimatorBehavior : StateMachineBehaviour
        {
            public abstract MotionController Controller { get; set; }

            public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
            }

            public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
            }

            public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
            }
        }
    }
}
