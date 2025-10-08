using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public abstract class ArmatureHandle : MonoBehaviour, IArmatureHandle
        {
            // Forward the interface 
            public abstract ArmatureHandleType HandleType { get; }
            public abstract bool Enabled { get; protected set; }
            public abstract bool Transient { get; }
            public abstract void SetTarget(Transform target);
            public abstract void SetPosition(Vector3 position);
            public abstract void SetRotation(Quaternion rotation);

            // default implementations
            protected virtual float UpdateTime { get; set; } = 0.0f;

            public virtual void Enable(float time)
            {
                UpdateTime = time;
                Enabled = true;
            }

            public virtual void Disable(float time)
            {
                UpdateTime = time;
                Enabled = false;
            }

            // <debug
            public virtual bool ArmatureHandlesVisible { get; set; }
            // debug>

            // ...
            protected virtual void Start()
            {
                Renderer = GetComponent<Renderer>();
                UpdateTime = 0.0f;
            }

            protected virtual void LateUpdate()
            {
                if (Renderer)
                {
                    if (Renderer.enabled != ArmatureHandlesVisible)
                        Renderer.enabled = ArmatureHandlesVisible;

                    if (Renderer.enabled)
                    {
                        if (Enabled)
                            Renderer.material.color = new Color(0.0f, 0.7f, 0.2f, 0.5f);
                        else
                            Renderer.material.color = new Color(0.7f, 0.2f, 0.2f, 0.5f); ;
                    }
                }
            }

            protected virtual float adjust(float x, bool increment = true)
            {
                float vt = Mathf.Clamp(UpdateTime, 1e-3f, 120.0f);
                return Mathf.Clamp01(x + (increment ? 1.0f : -1.0f) * Time.deltaTime / vt);
            }

            // ...
            protected Renderer Renderer = null;
        }
    }
}


