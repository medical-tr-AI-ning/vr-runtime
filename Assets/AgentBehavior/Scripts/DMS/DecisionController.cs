using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public abstract class DecisionController
        {
            public abstract void Think(IPerceptionModel model, ActionList actionList);

            public virtual void LateUpdate(IPerceptionModel model) { }
        }
    }
}