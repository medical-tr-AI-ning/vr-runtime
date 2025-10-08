using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class EarHandle : ArmatureHandle
        {
            //...
            public enum Ear
            {
                LeftEar, 
                RightEar
            }

            [SerializeField] protected BlendShapeRigConstraintProxy IKConstraint;
            [SerializeField] protected Ear Type;

            // <debug
            [Header("Debug")]
            [ReadOnly, SerializeField] private bool IsValid = false;
            [ReadOnly, SerializeField] private bool IsEnabled = false;
            [ReadOnly, SerializeField] private bool IsTransient = false;
            // debug>

            //...
            [ReadOnly, SerializeField] protected Transform Target;

            //...
            public override ArmatureHandleType HandleType 
                => Type == Ear.LeftEar ? ArmatureHandleType.LeftEar : ArmatureHandleType.RightEar;

            public override bool Enabled {
                get
                {
                    return IKConstraint && IKConstraint.Valid && IKConstraint.Enabled;
                }
                
                protected set
                {
                    if (IKConstraint)
                        IKConstraint.Enabled = value;
                }
            }

            public override bool Transient 
                => IKConstraint && IKConstraint.Valid && IKConstraint.Weight > 0.0f;

            public override void SetTarget(Transform target)
                => Target = target;
            
            public override void SetPosition(Vector3 position)
            {
                if (!Enabled)
                    return;

                IKConstraint.Target.position = position;
            }

            public override void SetRotation(Quaternion rotation)
            {
                if (!Enabled)
                    return;

                IKConstraint.Target.rotation = rotation;
            }

            // ...
            protected override void Start()
            {
                base.Start();

                // reset the target
                if (IKConstraint)
                {
                    IKConstraint.Enabled = false;
                    IKConstraint.Weight = 0.0f;
                }
                UpdateTime = 2.0f;
            }

            // ...
            protected override void LateUpdate()
            {
                base.LateUpdate();

                if (Target)
                {
                    SetPosition(Target.position);
                    SetRotation(Target.rotation);
                }

                if (Transient)
                    IKConstraint.Weight = adjust(IKConstraint.Weight, false);

                // <debug
                IsValid = IKConstraint && IKConstraint.Valid;
                IsEnabled = Enabled;
                IsTransient = Transient;
                // debug>
            }
        }
    }
}
