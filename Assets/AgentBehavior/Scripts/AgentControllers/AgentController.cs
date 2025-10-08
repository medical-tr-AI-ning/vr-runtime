using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        /// <summary>
        /// The entry point for interaction with the Agent. Each Agent has an
        /// AgentController that receives InteractionEvents from the user, the 
        /// environment, the other agents, etc. 
        /// 
        /// The current clothing and high-level states of the agents are visible 
        /// through the respective properties. 
        /// 
        /// The access to the Agent's Armature, is achieved through the IArmatureHandle 
        /// interface. Use GetArmatureHandle to get a handle for the respective body part. 
        /// </summary>
        public abstract class AgentController : MonoBehaviour
        {            
            /// <summary>
            /// The current clothing state of this agent. 
            /// </summary>
            public abstract ClothingState ClothingState { get; }

            /// <summary>
            /// The current high-level state of this agent. 
            /// </summary>
            public abstract HLState HLState { get; }

            /// <summary>
            /// Notify the Agent about something. 
            /// Use ActionRequests to instruct the Agent to do something, Notifications 
            /// to inform it about something, or SpeachEvents to inform it about questions, 
            /// being asked, instructions being issued, etc.  
            /// The Agent needs to be informed about speech input so that it can react 
            /// appropriately (e.g., look at the user when a question is asked.)
            /// </summary>
            /// <remarks>
            /// The AgentController is the front end of the Non-Verbal Behavior System.
            /// The SpeachEvent-s will be neither processed nor forwarded here.
            /// </remarks>
            /// <param name="interactionEvent">The event to be processed.</param>
            public abstract void notify(InteractionEvent interactionEvent);

            /// <summary>
            /// Get an armature handle that control some part of the agent's body,e.g. 
            /// left/right hand, some finger, foot,genitals, etc.
            /// </summary>
            /// <param name="type">Type of the handle we wan to access.</param>
            /// <returns>The function returns the armature handle for the respective body part
            /// or null if the handle type is not supported.</returns>
            public abstract IArmatureHandle GetArmatureHandle(ArmatureHandleType type);
        }
    }
}

