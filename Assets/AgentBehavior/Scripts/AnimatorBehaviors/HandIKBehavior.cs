using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class HandIKBehavior : AnimatorBehavior
        {
            protected enum IKHandle
            {
                LeftHand = ArmatureHandleType.LeftHand,
                RightHand = ArmatureHandleType.RightHand,
            }

            protected enum ActivationMode
            {
                EnableOnEnter,
                EnableOnLeave,

                DisableOnEnter,
                DisableOnLeave,

                EnableOnEnterDisableOnLeave,
                DisableOnEnterEnableOnLeave,
            }

            [SerializeField] protected IKHandle Handle;
            [SerializeField] protected ActivationMode Mode;
            [SerializeField] protected float TimeScale = 1.0f;

            //[SerializeField] protected float ActivationTime = 0.5f;   // seconds
            //[SerializeField] protected float DeactivationTime = 0.5f; // seconds

            protected AutoAttachHandle Hand = null;
            protected PatientAgentComponent Agent = null;
            protected Transform Target = null;

            public override MotionController Controller { get; set; }

            public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                // TODO: dv: not sure if we are always called w/ the same animator :/
                //if (animator != Controller.Animator)
                //    return;

                Initialize();

                if (!Hand || !Target)
                    return;

                if (Mode == ActivationMode.EnableOnEnter || Mode == ActivationMode.EnableOnEnterDisableOnLeave)
                {
                    Hand.SetTarget(Target);
                    Hand.Enable(TimeScale * stateInfo.length);
                }
                else if (Mode == ActivationMode.DisableOnEnter || Mode == ActivationMode.DisableOnEnterEnableOnLeave)
                {
                    Hand.Disable(TimeScale * stateInfo.length);
                }
            }

            public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                if (!Hand || !Target)
                    return;

                if (Mode == ActivationMode.EnableOnLeave || Mode == ActivationMode.DisableOnEnterEnableOnLeave)
                {
                    Hand.SetTarget(Target);
                    Hand.Enable(TimeScale * stateInfo.length);
                }
                else if (Mode == ActivationMode.DisableOnLeave || Mode == ActivationMode.EnableOnEnterDisableOnLeave)
                {
                    Hand.Disable(TimeScale * stateInfo.length);
                }
            }

            // ...
            private bool _initialized = false;

            private void Initialize()
            {
                if (!_initialized)
                {
                    _initialized = true;

                    Agent = Controller.Animator.GetComponent<PatientAgentComponent>();
                    if (!Agent)
                        return;

                    Hand = Agent.GetArmatureHandle((ArmatureHandleType)Handle) as AutoAttachHandle;

                    CustomPropertyList[] targets = Agent.GetComponentsInChildren<CustomPropertyList>();
                    CustomTagProperty tag = Handle == IKHandle.LeftHand ? CustomTagProperty.LeftThigAnchor : CustomTagProperty.RightThigAnchor;
                    foreach (CustomPropertyList target in targets)
                    {
                        if (target.Tags.Contains(tag))
                        {
                            Target = target.transform;
                            break;
                        }
                    }
                }
            }
        }
    }
}
