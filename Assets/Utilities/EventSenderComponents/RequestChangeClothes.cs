using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    public class RequestChangeClothes : ActionRequestSender
    {
        // Inspector properties
        [SerializeField] private ClothingState Clothing = ClothingState.Dressed;

        // interface
        protected override ActionRequest CreateActionRequest(IActionRequestStateObserver observer)
            => new ChangeClothes(observer, Clothing);
    }
}
