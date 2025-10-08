using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.Utilities;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class ThumbJointConstraint : FingerJointConstraint
        {
            enum ThumbJointType { LeftThumb, RightThumb };
            [SerializeField] private ThumbJointType Type;

            private static readonly ValueRange cmcRangeX = new ValueRange(25.0f, 75.0f);
            private static readonly ValueRange cmcRangeY = new ValueRange(-25.0f, 32.0f);
            private static readonly ValueRange cmcRangeZ = new ValueRange(-75.0f, -14.0f);
            private static readonly ValueRange mcpRange = new ValueRange(-30.0f, 0.0f);
            private static readonly ValueRange ipRange = new ValueRange(-85.0f, 0.0f);

           
            protected override void LateUpdate()
            {
                if (!Activated)
                    return;

                // the thumb joints have slightly different names, but we don't care that 
                // much here and simply use 'mcp' for the CMC joint, 'pip' for MCP and 'dip' for IP 

                // CMC/TMC joint
                Vector3 mcpRotationEuler = mcp.localRotation.eulerAngles;
                mcpRotationEuler.x = MathUtilities.ClampAngle(mcpRotationEuler.x, cmcRangeX);
                mcpRotationEuler.y = MathUtilities.ClampAngle(mcpRotationEuler.y, cmcRangeY);
                if (Type == ThumbJointType.LeftThumb)
                    mcpRotationEuler.z = -MathUtilities.ClampAngle(-mcpRotationEuler.z, cmcRangeZ);
                else
                    mcpRotationEuler.z = MathUtilities.ClampAngle(mcpRotationEuler.z, cmcRangeZ);
                mcp.SetLocalPositionAndRotation(mcp.localPosition, Quaternion.Euler(mcpRotationEuler));

                // MCP joint
                Vector3 pipRotationEuler = pip.localRotation.eulerAngles;
                pipRotationEuler.x = MathUtilities.ClampAngle(pipRotationEuler.x, mcpRange);
                pipRotationEuler.y = 0.0f;
                pipRotationEuler.z = 0.0f;
                pip.SetLocalPositionAndRotation(pip.localPosition, Quaternion.Euler(pipRotationEuler));

                // IP joint
                Vector3 dipRotationEuler = dip.localRotation.eulerAngles;
                if (FingertipAutoRotation)
                    dipRotationEuler.x = 2.0f * pipRotationEuler.x;
                else
                    dipRotationEuler.x = MathUtilities.ClampAngle(dipRotationEuler.x, ipRange);

                dipRotationEuler.y = 0.0f;
                dipRotationEuler.z = 0.0f;
                dip.SetLocalPositionAndRotation(dip.localPosition, Quaternion.Euler(dipRotationEuler));
            }
        }
    }
}

