using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public abstract class CustomJointConstraint : MonoBehaviour
        {
            public abstract bool Activated { get; set; }

            protected virtual void LateUpdate() { }
            protected virtual void Start() { }
        }
    }
}
