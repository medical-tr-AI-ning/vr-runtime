using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class JointConstraint : CustomJointConstraint
        {
            [SerializeField] protected bool Enabled = true;
            [HideInInspector] public ValueRange xRange = new ValueRange(-180.0f, 180.0f);
            [HideInInspector] public ValueRange yRange = new ValueRange(-180.0f, 180.0f);
            [HideInInspector] public ValueRange zRange = new ValueRange(-180.0f, 180.0f);

            //<debug
            [ReadOnly, SerializeField] private bool IsActivated = false;
            [ReadOnly, SerializeField] private bool xClamped;
            [ReadOnly, SerializeField] private bool yClamped;
            [ReadOnly, SerializeField] private bool zClamped;
            // debug>

            private bool _activated = false;
            public override bool Activated 
            { 
                get => Enabled && _activated; 
                set => _activated = value; 
            }

            protected override void LateUpdate() 
            {
                // <debug
                IsActivated = Activated;
                // debug>

                base.LateUpdate();
                if (!Activated)
                    return;

                Vector3 euler = transform.localEulerAngles;
                euler.x = MathUtilities.ClampAngle(euler.x, xRange, ref xClamped); 
                euler.y = MathUtilities.ClampAngle(euler.y, yRange, ref yClamped); 
                euler.z = MathUtilities.ClampAngle(euler.z, zRange, ref zClamped);
                transform.localEulerAngles = euler;
            }
        }

        // ...
        #region EditorScript
#if UNITY_EDITOR

        [CustomEditor(typeof(JointConstraint))]
        public class JointConstraintEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                JointConstraint tc = (JointConstraint)target;

                DrawDefaultInspector();
                EditorGUILayout.Space();

                EditorUtil.RangeSlider("Euler X", ref tc.xRange, new ValueRange(-180.0f, 180.0f));
                EditorUtil.RangeSlider("Euler Y", ref tc.yRange, new ValueRange(-180.0f, 180.0f));
                EditorUtil.RangeSlider("Euler Z", ref tc.zRange, new ValueRange(-180.0f, 180.0f));
            }
        }
#endif
        #endregion
    }
}