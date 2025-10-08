using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class LookAtHandle : MonoBehaviour, IArmatureHandle
        {
            // Inspector Attributes
            [SerializeField] protected GameObject IKConstraintsNode;

            // interface functions
            public virtual ArmatureHandleType HandleType
                => ArmatureHandleType.LookAt;

            public virtual bool Enabled { get; protected set; } = false;

            public virtual bool Transient
                => Valid && (MultiAimConstraint.weight != (Enabled ? 1.0f : 0.0f));

            public virtual bool TargetTransient
                => MultiAimConstraint.data.sourceObjects[CurrentSourceID].weight < 1.0f;

            public virtual void SetTarget(Transform target)
            {
                if (target == null || target == Target)
                    return;

                CurrentSourceID = (CurrentSourceID + 1) % NumSources;
                Target = target;
            }

            public virtual void SetPosition(Vector3 position)
            {
                if (Target)
                    Target.position = position;
            }

            public virtual void SetRotation(Quaternion rotation) { }

            public virtual void Enable(float time)
            {
                if (CurrentSourceID == -1 || Target == null)
                    return;

                UpdateTime = time;
                if (!Enabled) // TODO: dv: don't like this!
                    ResetSourceWeights();

                Enabled = true;
            }

            public virtual void Disable(float time)
            {
                UpdateTime = time;
                Enabled = false;
            }

            // ...
            protected virtual void Start()
            {
                if (!IKConstraintsNode)
                    return;

                MultiAimConstraint = IKConstraintsNode.GetComponent<MultiAimConstraint>();

                if (!MultiAimConstraint)
                    return;

                Enabled = false;
                Target = null;
                CurrentSourceID = -1;

                UpdateTime = 0.0f;
                MultiAimConstraint.weight = 0.0f;
            }

            protected virtual void LateUpdate()
            {
                if (!Valid)
                    return;

                UpdateContraintsWeights();
                UpdateSourceWeights();

                //
                if (Target && Enabled)
                {
                    var cso = MultiAimConstraint.data.sourceObjects[CurrentSourceID];
                    cso.transform.SetPositionAndRotation(Target.position, Target.rotation);
                }
            }

            // ...
            private MultiAimConstraint MultiAimConstraint;

            // ...
            public Transform Target { get; private set; } = null; // <hack>
            private int CurrentSourceID = -1;
            private float UpdateTime;

            // ...
            private int NumSources
                => !Valid ? 0 : MultiAimConstraint.data.sourceObjects.Count;

            private bool Valid
                => MultiAimConstraint != null;

            private void ResetSourceWeights()
                => UpdateSourceWeights(0.0f);

            private void UpdateSourceWeights()
                => UpdateSourceWeights(UpdateTime);

            private void UpdateSourceWeights(float time)
            {
                if (!Valid)
                    return;

                WeightedTransformArray sourceObjects = MultiAimConstraint.data.sourceObjects;
                if (Enabled && sourceObjects[CurrentSourceID].weight < 1.0f)
                {
                    for (int i = 0; i < sourceObjects.Count; ++i)
                        sourceObjects.SetWeight(i, adjust(sourceObjects.GetWeight(i), time, i == CurrentSourceID));

                    MultiAimConstraint.data.sourceObjects = sourceObjects;
                }
            }

            private void UpdateContraintsWeights()
            {
                if (!Valid)
                    return;

                float ConstraintWeight = MultiAimConstraint.weight;
                if (Enabled && ConstraintWeight < 1.0f)
                    MultiAimConstraint.weight = adjust(ConstraintWeight, UpdateTime);

                if (!Enabled && ConstraintWeight > 0.0f)
                    MultiAimConstraint.weight = adjust(ConstraintWeight, UpdateTime, false);
            }

            private float adjust(float x, float time, bool increment = true)
            {
                float vt = Mathf.Clamp(time, 1e-3f, 120.0f);
                return Mathf.Clamp01(x + (increment ? 1.0f : -1.0f) * Time.deltaTime / vt);
            }
        }
    }
}
