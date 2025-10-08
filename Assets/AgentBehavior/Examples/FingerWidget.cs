using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

public class FingerWidget : MonoBehaviour
{
    [System.Serializable]
    public class ConstrainedObject
    {
        [HideInInspector] public ArmatureHandle ArmatureHandle = null;
        public Transform Transform;
        [Range(0f, 1f)] public float Weight;

        public void Initialize()
        {
            if (Transform)
                ArmatureHandle = Transform.GetComponent<ArmatureHandle>();
        }
    }

    // Inspector Settings
    [SerializeField] public bool Enabled = false;
    [SerializeField] public Transform SourceA;
    [SerializeField] public Transform SourceB;
    [SerializeField] public ConstrainedObject[] ConstrainedObjects;

    // ...
    protected virtual void Start() 
    { 
        foreach (var constrainedObject in ConstrainedObjects)
            constrainedObject.Initialize();
    }

    // ...
    protected virtual void FixedUpdate()
    {
        if (!SourceA || !SourceB) 
            return;

        if (ConstrainedObjects.Length == 0)
            return;

        Vector3 dir = SourceB.position - SourceA.position;
        foreach (var obj in ConstrainedObjects)
        {
            if (obj.ArmatureHandle != null && obj.ArmatureHandle.Enabled != Enabled)
            {
                if (Enabled)
                    obj.ArmatureHandle.Enable(0.0f);
                else
                    obj.ArmatureHandle.Disable(0.3f);
            }

            if (Enabled && obj.Transform)
            {
                Vector3 p = obj.Transform.position;
                obj.Transform.position = SourceA.position + obj.Weight * dir;
            }
        }
    }
}
