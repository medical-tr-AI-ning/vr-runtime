using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class RotatePalmsAnimatorBehavior : StateMachineBehaviour
        {
            [SerializeField] bool UpDown = false;
            [SerializeField] BodySide BodySide;

            override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                if (BodySide == BodySide.Left)
                {
                    animator.SetFloat("left palm position", UpDown ? 1.0f : 0.0f);
                }
                else if (BodySide == BodySide.Right)
                {
                    animator.SetFloat("right palm position", UpDown ? 1.0f : 0.0f);
                }
            }
        }
    }
}
