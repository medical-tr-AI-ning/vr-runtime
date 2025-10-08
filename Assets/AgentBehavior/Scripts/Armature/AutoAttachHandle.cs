using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class AutoAttachHandle : ArmatureHandle
        {
            // Inspector attribute
            [SerializeField] protected GameObject IKConstraintsNode;
            [SerializeField] protected ArmatureHandleType Type = ArmatureHandleType.Invalid;
            [SerializeField] protected List<CustomJointConstraint> CustomJointConstraints = new();

            // <debug
            [Header("Debug")]
            [ReadOnly, SerializeField] private bool IsValid = false;
            [ReadOnly, SerializeField] private bool IsEnabled = false;
            [ReadOnly, SerializeField] private bool IsTransient = false; 
            // debug>

            public override ArmatureHandleType HandleType => Type;
            public override bool Transient => Weight != TargetWeight;
            public override bool Enabled
            {
                get => Weight > 0.0f;
                protected set
                {
                    if (!Valid || !value)
                    {
                        TargetWeight = 0.0f;
                        Target = null;
                    }
                    else
                    {
                        TargetWeight = 1.0f;
                    }
                }
            }
            public override void SetTarget(Transform target)
            {
                if (Valid && target)
                    Target = target;
            }

            protected override void Start()
            {
                base.Start();
                if (!InitializeConstraints())
                    return;

                Valid = true;
                ResetConstraints();
            }
            protected override void LateUpdate()
            {
                if (!Valid)
                    return;

                base.LateUpdate();
                UpdateConstraintsWeights();

                if (Target)
                {
                    SetPosition(Target.position);
                    SetRotation(Target.rotation);
                }

                // <debug
                IsValid = Valid;
                IsEnabled = Enabled;
                IsTransient = Transient;
                // debug>
            }

            public override void SetPosition(Vector3 position)
                => transform.position = position;
            public override void SetRotation(Quaternion rotation)
                => transform.rotation = rotation;

            // ...
            protected virtual bool InitializeConstraints()
            {
                // handle call-twice!
                Constraints.Clear();
                InverseConstraints.Clear();

                // find 'em all
                IRigConstraint[] rigConstraints = IKConstraintsNode?.GetComponents<IRigConstraint>();
                foreach (IRigConstraint constraint in rigConstraints)
                {
                    if (constraint.IsValid())
                    {
                        if (constraint is MultiParentConstraint)
                        {
                            InverseConstraints.Add(constraint);
                        }
                        else
                        {
                            Constraints.Add(constraint);
                        }
                    }
                }

                // TODO: dv: (low priority) check the constraints and auto-extract the CustomJointConstraint
                // from each controlled skeleton node.

                return Constraints.Count > 0 || InverseConstraints.Count > 0;
            }

            protected virtual void UpdateConstraintsWeights()
            {
                if (!Transient || !Valid)
                    return;

                Weight = adjust(Weight, TargetWeight == 1.0f);
                foreach (var constraint in Constraints)
                    constraint.weight = Weight;

                foreach (var constraint in InverseConstraints)
                    constraint.weight = 1.0f - Weight;

                // update the custom joint constraints
                foreach (var cjc in CustomJointConstraints)
                    cjc.Activated = (Weight > 0.0f);
            }

            protected virtual void ResetConstraints()
            {
                if (!Valid)
                    return;

                foreach (var constraint in Constraints)
                    constraint.weight = 0.0f;

                foreach (var constraint in InverseConstraints)
                    constraint.weight = 1.0f;

                Weight = 0.0f;
                TargetWeight = 0.0f;
            }

            // ...
            protected List<IRigConstraint> Constraints = new();
            protected List<IRigConstraint> InverseConstraints = new();

            // ...
            protected virtual bool Valid { get; set; } = false;

            // ...
            private float Weight = 0.0f;
            private float TargetWeight = 0.0f;

            // hack: dv: 02.1: make private!
            [ReadOnly][SerializeField] public Transform Target = null;
        }
    }
}
