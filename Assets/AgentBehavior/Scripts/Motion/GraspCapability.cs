using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        //           or for null in the methods... fix it.
        partial class PatientMotionController : IGraspCapability
        {
            public virtual bool Grasp(Transform target)
            {
                if (!target || !Hand)
                    return false;

                if (GraspingState != GraspingState.Idle && GraspingState != GraspingState.Retracing)
                    return false;

                Hand.SetTarget(target);
                GraspingState = GraspingState.WaitingForTargetReachable;
                return true;
            }

            public virtual void Release()
            {
                if (!Hand)
                    return;

                Hand.Disable(RetracingSpeed);
                Animator.SetBool(GraspParameterName, false);
                GraspTargetReachable = false;
            }

            public virtual bool Grasped
                => GraspingState == GraspingState.Grasped;

            public virtual bool Released
                => GraspingState == GraspingState.Idle; // => GraspingState == GraspingState.Retracing;

            public virtual bool GraspTargetReachable { get; protected set; }

            //public virtual GraspingState GraspingState { get; set; } // TODO: dv: protected set
            //public virtual Transform GraspTarget => Hand.Target;

            protected virtual void UpdateGraspState()
            {
                //if (GraspTarget == null)
                //{
                //    Hand.Disable(RetracingSpeed);
                //    Animator.SetBool(GraspParameterName, false);
                //    GraspTargetReachable = false;
                //    return;
                //}

                // TODO: dv: this only happens at the beginning, when no target is
                // ever set. Should fix this stupid behavior and remove the check here.
                // 
                // TODO: dv: combine the IGraspCapability and IArmtureHandle in a 
                // single, compound functionality block?
                if (!Hand || Hand.Target == null)
                    return;

                if (GraspingState == GraspingState.WaitingForTargetReachable)
                {
                    // check whether the target is reachable
                    Collider[] hitCollidrs = Physics.OverlapSphere(Hand.Target.position, 0.1f);
                    bool graspLeft = hitCollidrs.Contains(Agent.GraspColliders.Left);
                    bool graspRight = hitCollidrs.Contains(Agent.GraspColliders.Right);
                    GraspTargetReachable = graspLeft || graspRight;

                    // reach
                    if (GraspTargetReachable)
                    {
                        Hand.Enable(ReachingSpeed);
                        Animator.SetBool(GraspParameterName, true);
                        Animator.SetFloat(GraspParameterBlendName, graspLeft ? 0.0f : 1.0f);
                    }
                    else
                    {
                        Hand.Disable(RetracingSpeed);
                        Animator.SetBool(GraspParameterName, false);
                    }
                }
                else if (GraspingState != GraspingState.Retracing || GraspingState == GraspingState.Idle)
                {
                    GraspTargetReachable = false;
                }
            }

            protected virtual bool InitializeGraspCapability()
            {
                Hand = Agent.GetArmatureHandle(ArmatureHandleType.RightHand) as AutoAttachHandle;

                GraspTargetReachable = false;
                GraspingState = GraspingState.Idle;

                return (Hand != null && Agent.GraspColliders.Left && Agent.GraspColliders.Right);
            }

            // ...
            protected AutoAttachHandle Hand { get; set; }

            // ...
            private const float ReachingSpeed = 1.0f;
            private const float RetracingSpeed = 1.0f;
            private const string GraspParameterName = "grasping";
            private const string GraspParameterBlendName = "reach-left-right";
        }

    }
}
