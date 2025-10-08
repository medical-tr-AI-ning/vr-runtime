using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        // ...
        partial class PatientMotionController : ILookAtCapability
        {
            // target == null resets the look-at
            public virtual void SetLookAtTarget(Transform target, float speed)
            {
                if (!LookAtHandle)
                    return;

                if (target)
                {
                    LookAtHandle.SetTarget(target);
                    LookAtHandle.Enable(speed);
                }
                else
                {
                    LookAtHandle.Disable(speed);
                }
            }

            public virtual void SetLookAtTarget(Transform target)
                => SetLookAtTarget(target, target ? SwitchSpeed : OffSpeed);

            protected virtual bool InitializeLookAtCapability()
            {
                LookAtHandle = Agent.GetComponentInChildren<LookAtHandle>();
                return (LookAtHandle != null);
            }

            // ...
            protected LookAtHandle LookAtHandle { get; set; }
            public Transform LookAtTarget => LookAtHandle.Target; // <hack>

            // ...
            private const float SwitchSpeed = 1.0f;
            private const float OffSpeed = 1.0f;
        }

    }
}
