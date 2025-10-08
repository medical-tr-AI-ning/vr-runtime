using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        /// <summary>
        /// The IPerceptionEventStateObserver is used from the event system to 
        /// inform the sending unit about the current state of a PerceptionEvent.
        /// </summary>
        /// <remarks>
        /// Do not put long or demanding routines in the interface
        /// implementations, as this may block the event processing.
        /// </remarks>
        public interface IPerceptionEventStateObserver
        {
            /// <summary>
            /// The PerceptionEvent was accepted and may be processed by the perception system.
            /// </summary>
            /// <param name="perceptionEvent">The PerceptionEvent served.</param>
            public virtual void OnPerceived(PerceptionEvent perceptionEvent) { }

            /// <summary>
            /// The PerceptionEvent was ignored by the Agent.
            /// </summary>
            /// <param name="perceptionEvent">The PerceptionEvent served.</param>
            /// <param name="reason">The reason for ignoring this event.</param>
            public virtual void OnIgnored(PerceptionEvent perceptionEvent, Reason reason) { }

            /// <summary>
            /// The PerceptionEvent was processed and accepted by the perception system.
            /// </summary>
            /// <param name="perceptionEvent">The PerceptionEvent served.</param>
            public virtual void OnAccepted(PerceptionEvent perceptionEvent) { }

            /// <summary>
            /// The PerceptionEvent was perceived but then declined by the perception system.
            /// </summary>
            /// <param name="perceptionEvent">The PerceptionEvent served.</param>
            /// <param name="reason">The reason for declining this event.</param>
            public virtual void OnDeclined(PerceptionEvent perceptionEvent, Reason reason) { }
        }
    }
}
