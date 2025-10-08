using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public partial class PatientMotionController : IWalkCapability
        {
            public virtual void WalkTo(Transform target)
            {
                WalkTarget = target;
                WalkTargetReached = WalkTarget != null;

                if (WalkTarget != null)
                    StartWalking();
                else
                    StopWalking(); // if the target is null we're fine...
                                   // but don't want to walk any more
            }
            public virtual bool Walking
                => MotionState == MotionState.Walking;

            public virtual bool WalkTargetReached { get; protected set; } = true;

            // ...
            protected virtual void UpdateWalkingState() 
            {
                if (Walking && AnimatorMotionState == MotionState.Walking)
                {
                    Vector3 direction = WalkTarget.position - Transform.position;
                    direction.y = 0.0f;

                    if (direction.magnitude > Agent.MovementSettings.TargetReachedDelta)
                    {
                        Vector3 d = direction.normalized * Time.deltaTime;
                        d.y = -Agent.MovementSettings.Gravity;

                        CharacterController.Move(d * Agent.MovementSettings.BaseSpeed);
                        Transform.rotation = Quaternion.LookRotation(direction.normalized);
                    }
                    else
                    {
                        WalkTarget = null;
                        WalkTargetReached = true;
                        StopWalking();
                    }
                }
            }

            protected virtual void HandleWalkCollision()
            {
                StopWalking();
                Debug.Log("OnControllerColliderHit && !Ground");
            }

            protected virtual void StartWalking()
            {
                if (!Walking)
                {
                    MotionState = MotionState.Walking;
                    Animator.SetBool(WalkAnimation, true);
                    FixHeight(up:false); // <hack 03.2>
                }
            }

            protected virtual void StopWalking()
            {
                if (Walking)
                {
                    MotionState = MotionState.Standing;
                    Animator.SetBool(WalkAnimation, false);
                    FixHeight(up: true);  // <hack 03.3>
                }
            }

            // ...
            private const string WalkAnimation = "walking";
            private Transform WalkTarget = null;
        }
    }
}
