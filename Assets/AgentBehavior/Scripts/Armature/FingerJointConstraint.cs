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
        public class FingerJointConstraint : CustomJointConstraint
        {
            [SerializeField] protected bool Enabled = true;
            [SerializeField] protected bool FingertipAutoRotation = true;

            // <debug
            [ReadOnly, SerializeField] private bool IsValid = false;
            [ReadOnly, SerializeField] private bool IsActivated = false;
            [ReadOnly, SerializeField] private bool IsFingertipAutoRotationEnabled = false;
            // debug>

            public override bool Activated
            {
                get => Enabled && Valid && _activated;
                set => _activated = value;
            }

            protected override void Start()
            {
                dip = transform;
                pip = transform.parent;
                mcp = pip.parent;

                Valid = !(!dip || !pip || !mcp);
            }

            protected override void LateUpdate()
            {
                // <debug
                IsValid = Valid;
                IsActivated = Activated;
                IsFingertipAutoRotationEnabled = FingertipAutoRotation;
                // debug>

                base.LateUpdate();

                if (!Activated)
                    return;

                // mcp 
                Vector3 mcpRotationEuler = mcp.localRotation.eulerAngles;
                mcpRotationEuler.x = MathUtilities.ClampAngle(mcpRotationEuler.x, MinMCPAbduction, MaxMCPAbduction);
                mcpRotationEuler.y = 0.0f;
                mcpRotationEuler.z = MathUtilities.ClampAngle(mcpRotationEuler.z, MinMCPFlexion, MaxMCPFlexion);
                mcp.SetLocalPositionAndRotation(mcp.localPosition, Quaternion.Euler(mcpRotationEuler));

                // pip
                Vector3 pipRotationEuler = pip.localRotation.eulerAngles;
                pipRotationEuler.x = 0.0f;
                pipRotationEuler.y = 0.0f;
                pipRotationEuler.z = MathUtilities.ClampAngle(pipRotationEuler.z, MinFlexion, MaxFlexion);
                pip.SetLocalPositionAndRotation(pip.localPosition, Quaternion.Euler(pipRotationEuler));

                // dip
                Vector3 dipRotationEuler = dip.localRotation.eulerAngles;
                dipRotationEuler.x = 0.0f;
                dipRotationEuler.y = 0.0f;
                if (FingertipAutoRotation)
                    dipRotationEuler.z = Ratio * pipRotationEuler.z;
                else
                    dipRotationEuler.z = MathUtilities.ClampAngle(dipRotationEuler.z, MinFlexion, MaxFlexion);

                dip.SetLocalPositionAndRotation(dip.localPosition, Quaternion.Euler(dipRotationEuler));
            }

            // ...
            protected Transform mcp;
            protected Transform pip;
            protected Transform dip;

            protected bool Valid = false;
            private bool _activated = false;

            // ...
            [HideInInspector] public float Ratio = 2.0f / 3.0f;

            [HideInInspector] public float MaxFlexion =  0.0f;
            [HideInInspector] public float MinFlexion = -80.0f;

            [HideInInspector] public float MaxMCPFlexion = 45.0f;
            [HideInInspector] public float MinMCPFlexion = -80.0f;

            [HideInInspector] public float MinMCPAbduction = -30.0f;
            [HideInInspector] public float MaxMCPAbduction = 30.0f;
        }

        // ...
        #region EditorScript
#if UNITY_EDITOR

        [CustomEditor(typeof(FingerJointConstraint))]
        public class FingerJointConstraintEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                FingerJointConstraint tc = (FingerJointConstraint)target;

                DrawDefaultInspector();
                EditorGUILayout.Space();

                EditorUtil.RangeSlider("MCP Abduction", ref tc.MinMCPAbduction, ref tc.MaxMCPAbduction, -45.0f, 45.0f);
                EditorUtil.RangeSlider("MCP Flexion", ref tc.MinMCPFlexion, ref tc.MaxMCPFlexion, -90.0f, 90.0f);
                EditorUtil.RangeSlider("xIP Flexion", ref tc.MinFlexion, ref tc.MaxFlexion, -90.0f, 10.0f);
            }
        }
#endif
        #endregion
    }
}
