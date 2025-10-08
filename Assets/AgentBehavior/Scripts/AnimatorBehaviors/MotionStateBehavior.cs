using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        class MotionStateBehavior : AnimatorBehavior
        {
            // Inspector
            [SerializeField] protected MotionState MotionState = MotionState.Invalid;

            [SerializeField][ReadOnly] private PatientMotionController PatientMotion;
            //[SerializeField][ReadOnly][Range(0, 1)] private float BoostThresholdA = 0.3f;
            //[SerializeField][ReadOnly][Range(0, 1)] private float BoostThresholdB = 0.4f;
            //[SerializeField][ReadOnly][Range(0, 10)] private float BoostAmount = 3.0f;

            // Interface
            public override MotionController Controller { 
                get { return PatientMotion; }
                set { PatientMotion = value as PatientMotionController; } 
            }

            public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                if (PatientMotion == null || layerIndex != 0)
                    return;

                bool transient = animator.IsInTransition(0);
                PatientMotion.AnimatorMotionState = (transient ? MotionState.Transient : MotionState);
                
                //if (PatientMotion.AnimatorMotionState == MotionState.Turning)
                //{
                //    float t = stateInfo.normalizedTime;
                //    float boost;

                //    if (t < BoostThresholdA)
                //        boost = BoostAmount;
                //    else if (t < BoostThresholdB)
                //        boost = Mathf.SmoothStep(BoostAmount, 0, (t - BoostThresholdA) / (BoostThresholdB - BoostThresholdA));
                //    else
                //        boost = 0.0f;

                //    float angle = animator.GetFloat("turn-angle");
                //    float tsm = Mathf.Clamp(135.0f / angle, 1.0f, 25.0f);
                //    animator.SetFloat("turn-speed-multiplier", tsm + boost);
                //}
            }

            public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                if (PatientMotion != null && MotionState == MotionState.Turning)
                {
                    PatientMotion.MotionState = MotionState.Standing;
                    PatientMotion.FixHeight(up: true); // <hack 03.5>
                }
            }
        }
    }
}
