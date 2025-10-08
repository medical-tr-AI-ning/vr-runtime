using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using MedicalTraining.Configuration;

namespace MedicalTraining.Logger {

    [RequireComponent( typeof(Valve.VR.InteractionSystem.Interactable) )]
    public class InteractableLogger : MonoBehaviour
    {
        [SerializeField]
        protected new string name;
        protected Valve.VR.InteractionSystem.Interactable interactable;
        protected SimulationLogger logger;

        protected virtual void Start()
        {
            logger = ConfigurationContainer.Instance.GetLogger();
            interactable = GetComponent<Valve.VR.InteractionSystem.Interactable>();
        }

        protected virtual void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
        {
            if (logger != null)
            {
                logger.WriteEvent(name, "attached to", hand.handType.ToString());
            }
        }

        protected virtual void OnDetachedFromHand(Valve.VR.InteractionSystem.Hand hand)
        {
            if(logger != null){
                logger.WriteEvent(name, "detached from", hand.handType.ToString());
            }
        }
    }
}
