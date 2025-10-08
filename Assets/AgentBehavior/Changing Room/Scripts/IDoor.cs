using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public interface IDoor
        {
            public abstract void Open();
            public abstract void Close();

            public abstract float RelativePosition { get; set; } // amount in [0, 1]
            public virtual bool Opened { get => RelativePosition >= 0.9f; } // TODO: dv: settable threshold?
            public virtual bool Closed { get => RelativePosition <= 0.1f; } // TODO: dv: settable threshold?
            public abstract Transform GraspTargetOut { get; }
            public abstract Transform GraspTargetIn { get; }
            public abstract Transform WalkTargetOut { get; }
            public abstract Transform WalkTargetIn { get; }
        }
    }
}
