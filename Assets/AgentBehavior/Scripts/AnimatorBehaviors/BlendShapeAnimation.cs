using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        // TODO: dv: re-factor the blend shape handling
        // BlendShapeInfo, BlendShapeAnimation, BlendShapeBehavior, BlendShapeRigConstraintProxy
        // all do almost the same with slight differences... that's stupid

        public class BlendShapeAnimation : MonoBehaviour
        {
            protected enum AnimationType
            {
                Linear,
                Triangle,
                SmoothStep,
                DualSmoothStep,
            }

            // Inspector
            [SerializeField] protected bool Playing = false;
            [SerializeField] protected bool Continuous = false;
            [ReadOnly, SerializeField] protected bool Interruptible = false;
            [SerializeField] protected AnimationType Type = AnimationType.Linear;
            [SerializeField] protected float Duration = 0.5f;

            [SerializeField] protected List<BlendShapeInfo> BlendShapes = new();

            // ...
            private bool _playing = false;
            private float _startTime = 0;

            public void Play(bool play = true)
                => Playing = play;

            // ...
            protected virtual void Start()
            {
                foreach (BlendShapeInfo blendShape in BlendShapes)
                    blendShape.Initialize();

                // remove all invalid blend shapes
                //BlendShapes.RemoveAll(shape => !shape);
            }

            // ...
            protected virtual void Update()
            {
                // if play state changed
                if (_playing != Playing)
                {
                    if (Playing) // just switched on
                    {
                        _startTime = Time.time;
                        _playing = true;
                    }
                    else // switch off. If interruptible - immediately, else at the end of the playback.
                    {
                        Continuous = false;
                        if (Interruptible)
                            _playing = false;
                    }                     
                }

                if (!_playing)
                    return;

                // ...
                int frameFactor = ((int)Type) % 2 == 0 ? 1 : 2;
                int duration = (int)(Duration * 1000) / frameFactor;

                int time = (int)((Time.time - _startTime) * 1000);
                int frame = time / duration;
                float frameTime = (time % duration) / (float)duration;

                if (!Continuous && frame >= frameFactor)
                {
                    Playing = _playing = false;
                    return;
                }

                float weight = 0.0f;
                switch (Type)
                {
                    case AnimationType.Linear:
                        weight = frameTime;
                        break;

                    case AnimationType.Triangle:
                        if (frame % 2 == 0)
                            weight = frameTime;
                        else
                            weight = 1.0f - frameTime;
                        break;

                    case AnimationType.SmoothStep:
                        weight = Mathf.SmoothStep(0.0f, 1.0f, frameTime);
                        break;

                    case AnimationType.DualSmoothStep:
                        if (frame % 2 == 0)
                            weight = Mathf.SmoothStep(0.0f, 1.0f, frameTime);
                        else
                            weight = Mathf.SmoothStep(1.0f, 0.0f, frameTime);
                        break;
                }

                // setup the frame
                foreach (BlendShapeInfo blendShape in BlendShapes)
                    blendShape.UniformWeight = weight;
            }
        }
    }
}
