using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        // TODO: dv: re-factor me
        [System.Serializable]
        public class BlendShapeInfo
        {
            [SerializeField] protected string NodeName;
            [SerializeField] protected string Name;
            [SerializeField, Range(0.0f, 1.0f)] public float AnimationOffset = 0.0f;
            [Range(0.0f, 1.0f)] public float BlendOffset = 0.0f;
            public bool Invert = false;

            [SerializeField] public SkinnedMeshRenderer Renderer = null;
            [ReadOnly] public int Index = -1;

            //...
            public static implicit operator bool(BlendShapeInfo info)
            {
                return info is not null && info.Renderer && info.Index >= 0;
            }

            //...
            public bool Initialize(Transform root = null)
            {
                // try to find a renderer
                if (!Renderer && root)
                {
                    Transform node = root.Find(NodeName);
                    if (!node)
                        return false;

                    Renderer = node.GetComponent<SkinnedMeshRenderer>();
                }

                // shit!
                if (!Renderer)
                    return false;

                // set the node name
                if (NodeName == "")
                    NodeName = Renderer.gameObject.name;

                // get the index of the blend shape
                int index = Renderer.sharedMesh.GetBlendShapeIndex(Name);
                if (index < 0)
                    return false;

                // we are fine :)
                Index = index;
                return true;
            }

            // TODO: dv: The blend shape and the animation offset are still handled externally
            // TODO: dv: Animation offset has nothing to do with the blend shape.
            public virtual float Weight {
                get
                {
                    if (Renderer == null || Index < 0)
                        return 0.0f;

                    return Renderer.GetBlendShapeWeight(Index);
                }
                set
                {
                    if (Renderer != null && Index >= 0)
                    {
                        float w = Mathf.Clamp(value, 0.0f, 100.0f);
                        w = Invert ? 100.0f - w : w;
                        Renderer.SetBlendShapeWeight(Index, w);
                    }
                }
            }

            public virtual float UniformWeight
            {
                get => Weight / 100.0f;
                set => Weight = value * 100.0f;
            }
        }
    }
}
