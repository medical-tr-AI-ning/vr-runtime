using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        // ...
        partial class PatientMotionController : ITurnCapability
        {
            public virtual void AlignTo(Transform target)
            {
                Quaternion rot = Quaternion.Inverse(Transform.rotation) * target.rotation;
                TurnBy(rot.eulerAngles.y); // TODO: dv: is that what I want?
            }

            public virtual void TurnTo(Transform target, bool invert)
            {
                Vector3 to = (invert ? -1.0f : 1.0f) * (target.position - Transform.position).normalized;
                Vector3 from = Transform.forward;
                TurnBy(Quaternion.FromToRotation(from, to).eulerAngles.y);
            }

            public virtual void TurnBy(float degrees)
            {
                // TODO: dv: Do we need to check the motion 
                //           state here or leave it to the DMS?

                float turnAngle = ClampAngle(degrees);
                float absoluteAngle = Mathf.Abs(turnAngle);
                //Debug.Log($"Turn by {degrees} => {turnAngle}");

                if (absoluteAngle < MinRotationAngle)
                    return;

                if (absoluteAngle < MinAnimationAngle)
                {
                    Transform.Rotate(Vector3.up, degrees);
                    return;
                }

                Animator.SetBool(TurnDirectionAnimation, turnAngle < 0);
                Animator.SetFloat(TurnAnimationBlend, absoluteAngle);
                Animator.SetFloat(TurnAnimaationSpeedMultiplier, 180.0f / absoluteAngle);

                Animator.SetTrigger(TurnAnimation);
                MotionState = MotionState.Turning;
                FixHeight(up: false); // <hack 03.4>
            }

            // ...
            public virtual bool Turning
                => MotionState == MotionState.Turning;

            // ...
            private const float MinRotationAngle = 2.0f;
            private const float MinAnimationAngle = 10.0f;
            private const string TurnDirectionAnimation = "turn-left-right";
            private const string TurnAnimationBlend = "turn-angle";
            private const string TurnAnimaationSpeedMultiplier = "turn-speed-multiplier";
            private const string TurnAnimation = "turn";

            // ...
            private float ClampAngle(float degree)
            {
                while (degree > 180)
                    degree -= 360.0f;

                while (degree < -180)
                    degree += 360.0f;

                return degree;
            }
        }
    }
}
