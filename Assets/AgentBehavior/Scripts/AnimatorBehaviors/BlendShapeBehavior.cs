using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class BlendShapeBehavior : AnimatorBehavior
        {
            // Inspector
            [SerializeField] protected List<BlendShapeInfo> BlendShapes = new();
            
            // Interface
            public override MotionController Controller { get; set; }

            // ...
            override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                Initialize();
            }

            override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                float t = stateInfo.normalizedTime;
                if (t > 1.0f)
                    return;


                foreach (BlendShapeInfo blendShape in BlendShapes)
                {
                    if (blendShape && t >= blendShape.AnimationOffset)
                    {
                        float m = (1.0f - blendShape.BlendOffset) / (1.0f - blendShape.AnimationOffset);                      
                        blendShape.UniformWeight = m * t + (1.0f - m);
                    }
                }
            }

            // ...
            private bool _initialized = false;

            private void Initialize()
            {
                if (!_initialized)
                {
                    foreach (BlendShapeInfo blendShape in BlendShapes)
                        blendShape.Initialize(Controller.Animator.transform);

                    _initialized = true;
                }
            }
        }
    }
}
