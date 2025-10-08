using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class BlendShapeRigConstraintProxy : MonoBehaviour
        {
            // ...
            [Header("Settings")]
            [SerializeField] public bool Enabled = false;
            [SerializeField, Range(0.0f, 1.0f)] public float Weight = 0.0f;

            // ...
            public bool Valid => (Target && AnchorStart && AnchorEnd);

            // ...
            [Header("Agent Setup")]
            [SerializeField] public Transform Target;
            [SerializeField] protected Transform AnchorStart;
            [SerializeField] protected Transform AnchorEnd;
            [SerializeField] protected List<BlendShapeInfo> BlendShapes = new();

            // ...
            void Start()
            {
                foreach (BlendShapeInfo blendShape in BlendShapes)
                    blendShape.Initialize();
            }

            void Update()
            {
                if (!Valid)
                    return;

                if (!Enabled && Vector3.Distance(Target.position, AnchorStart.position) > 1e-4f) 
                {
                    Vector3 p = Weight * Target.position;
                    p += (1.0f - Weight) * AnchorStart.position;
                    Target.SetPositionAndRotation(p, Target.rotation);
                }

                Vector3 direction = (AnchorEnd.position - AnchorStart.position).normalized;
                float invDistance = 1.0f / (Vector3.Distance(AnchorStart.position, AnchorEnd.position));

                Vector3 vd = Target.position - AnchorStart.position;
                float t = Mathf.Clamp01(invDistance * Vector3.Dot(vd, direction));  

                // update 'em all!
                foreach (BlendShapeInfo blendShape in BlendShapes)
                {
                    if (blendShape)
                    {
                        // TODO: dv: update to use the BlendShapeInfo.UniformWeight
                        float w = (1.0f - blendShape.BlendOffset) * t + blendShape.BlendOffset;
                        w = 100.0f * (blendShape.Invert ? 1.0f - w : w);

                        blendShape.Renderer.SetBlendShapeWeight(blendShape.Index, w);
                    }
                }
            }
        }
    }
}

