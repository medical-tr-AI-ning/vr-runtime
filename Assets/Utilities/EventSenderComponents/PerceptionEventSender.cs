using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    /// <summary>
    /// Base <code>PerceptionEvent</code> sender class. 
    /// The class is intended to provide a way to decouple the implementations of a particular event trigger, 
    /// e.g., a menu button, and a <code>PerceptionEvent</code>.
    /// 
    /// If the event trigger (or some other user class) implements the <code>IPerceptionEventStateObserver</code>,
    /// it may simply add the required PerceptionEventSender derivate as component and use its <code>Send(observer)</code> 
    /// method to dispatch the request.
    /// 
    /// To fully decouple the implementations, one can use, e.g., an UnityEvent, to invoke the <code>Send()</code> 
    /// method and register the own delegates to OnEventPerceived, OnEventIgnored, etc.
    /// </summary>
    public abstract class PerceptionEventSender : MonoBehaviour, IPerceptionEventStateObserver
    {
        // Inspector
        /// <summary>
        /// Inspector delegates to register own callback methods instead of implementing 
        /// the IPerceptionEventStateObserver interface. 
        /// </summary>
        [SerializeField] private UnityEvent OnEventPerceived;
        [SerializeField] private UnityEvent<Reason> OnEventIgnored;
        [SerializeField] private UnityEvent OnEventAccepted;
        [SerializeField] private UnityEvent<Reason> OnEventDeclined;

        // interface
        protected abstract PerceptionEvent CreatePerceptionEvent(IPerceptionEventStateObserver observer);

        /// <summary>
        /// Create and send an PerceptionEvent using external observer 
        /// </summary>
        /// <param name="observer">Observer to be notified about the state of the event.</param>
        public virtual void Send(IPerceptionEventStateObserver observer)
        {
            if (!_controller)
            {
                Debug.LogError("Cannot send PerceptionEvent. AgentController not initialized!");
                return;
            }

            PerceptionEvent perceptionEvent = CreatePerceptionEvent(observer);
            if (perceptionEvent == null)
            {
                Debug.LogError("Cannot send PerceptionEvent. CreatePerceptionEvent returned null!");
            }

            perceptionEvent.AddObserver(this);
            _controller.notify(perceptionEvent);
        }

        /// <summary>
        /// Convenience extension of Send(observer). 
        /// This makes the function visible for an UnityEvent. 
        /// </summary>
        public virtual void Send() => Send(null);

        // ...
        protected virtual void Start()
        {
            // TODO: dv: if there are other agents in the scene and we want to be able to send
            //           messages to them, this will not work... don't care about it right now.

            // TODO: dv: Don't like searching for nodes by name but there might be other
            //           agents in the scene and each would have it's own AgentController.

            GameObject patient = GameObject.Find("/SceneRoot/Agents/Patient");
            if (!patient)
            {
                Debug.LogError("No Patient Node found. Are you in the right scene?");
                return;
            }
            _controller = patient.GetComponent<AgentController>();
        }

        // ...
        protected virtual void Update()
        {
        }

        // implementation of the observer interface.
        public virtual void OnPerceived(PerceptionEvent action)
            => OnEventPerceived?.Invoke();

        public virtual void OnIgnored(PerceptionEvent action, Reason reason)
            => OnEventIgnored?.Invoke(reason);
        
        public virtual void OnAccepted(PerceptionEvent action)
            => OnEventAccepted?.Invoke();

        public virtual void OnDeclined(PerceptionEvent action, Reason reason)
            => OnEventDeclined?.Invoke(reason);

        // ...
        protected AgentController _controller;
    }
}
