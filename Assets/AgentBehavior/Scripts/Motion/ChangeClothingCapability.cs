using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        partial class PatientMotionController : IChangeClothingCapability
        {
            public virtual void ChangeClothing(ClothingState clothingState)
            {
                if (Clothing == clothingState)
                    return;

                Agent.ChangeClothing(clothingState);
                Clothing = clothingState;
            }
            public virtual bool ClothingChanged { get; protected set; } = true;
        }
    }
}
