using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public abstract class PerceptionEvent : InteractionEvent
        {
            /// <summary>
            /// The constructor of the PerceptionEvent
            /// </summary>
            /// <param name="observer">Optional observer for this PerceptionEvent.</param>
            public PerceptionEvent(IPerceptionEventStateObserver observer = null)
            {
                _observers = new List<IPerceptionEventStateObserver>();
                if (observer != null)
                    AddObserver(observer);
            }

            /// <summary>
            /// Adds an IPerceptionEventStateObserver to be informed about 
            /// the state of this request
            /// </summary>
            /// <param name="observer">The object to be informed about status changes</param>
            public virtual void AddObserver(IPerceptionEventStateObserver observer)
            {
                // dv: assert(observer)
                if (observer != null)
                    _observers.Add(observer);
            }

            /// <summary>
            /// Removes a particular observer from the list for this request
            /// </summary>
            /// <param name="observer">The object to be removed from the list</param>
            public virtual void RemoveObserver(IPerceptionEventStateObserver observer)
            {
                // dv: assert(observer)
                if (observer != null)
                    _observers.Remove(observer);
            }

            // ...
            // Internal use only!
            public IEnumerable<IPerceptionEventStateObserver> Observers { get => _observers; }
            private List<IPerceptionEventStateObserver> _observers;
        }
    }
}
