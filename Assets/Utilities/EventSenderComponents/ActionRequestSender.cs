using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    /// <summary>
    /// Base <code>ActionRequest</code> sender class. 
    /// The class is intended to provide a way to decouple the implementations of a particular event trigger, 
    /// e.g., a menu button, and an <code>ActionRequest</code>.
    /// 
    /// If the event trigger (or some other user class) implements the <code>IActionRequestStateObserver</code>,
    /// it may simply add the required ActionRequestSender derivate as component and use its <code>Send(observer)</code> 
    /// method to dispatch the request.
    /// 
    /// To fully decouple the implementations, one can use, e.g., an UnityEvent, to invoke the <code>Send()</code> 
    /// method and register the own callbacks to the OnRequestAccepted, OnRequestDeclined, etc. delegates.
    /// </summary>
    public abstract class ActionRequestSender : MonoBehaviour, IActionRequestStateObserver
    {
        // Inspector
        /// <summary>
        /// Inspector delegates to register own callback methods instead of implementing 
        /// the IActionRequestStateObserver interface. 
        /// </summary>
        [SerializeField] private UnityEvent OnRequestAccepted;
        [SerializeField] private UnityEvent<Reason> OnRequestDeclined;
        [SerializeField] private UnityEvent OnRequestStarted;
        [SerializeField] private UnityEvent<Reason> OnRequestAborted;
        [SerializeField] private UnityEvent OnRequestFinished;

        // interface
        protected abstract ActionRequest CreateActionRequest(IActionRequestStateObserver observer);

        /// <summary>
        /// Create and send an ActionRequest with external observer 
        /// </summary>
        /// <param name="observer">Observer to be notified about the state of the request.</param>
        public virtual void Send(IActionRequestStateObserver observer)
        {
            if (!_controller)
            {
                Debug.LogError("Cannot send ActionRequest. AgentController not initialized!");
                return;
            }

            ActionRequest actionRequest = CreateActionRequest(observer);
            if (actionRequest == null)
            {
                Debug.LogError("Cannot send ActionRequest. CreateActionRequest returned null!");
            }

            actionRequest.AddObserver(this);
            actionRequest.AddObserver(ActionAdapter.Instance);
            _controller.notify(actionRequest);
        }

        /// <summary>
        /// Convenience extension of Send(observer). 
        /// This makes the function visible for the UnityEvent. 
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
        public virtual void OnAccepted(ActionRequest action) 
            => OnRequestAccepted?.Invoke(); 
        
        public virtual void OnDeclined(ActionRequest action, Reason reason) 
            => OnRequestDeclined?.Invoke(reason); 
        
        public virtual void OnStarted(ActionRequest action) 
            => OnRequestStarted?.Invoke(); 
        
        public virtual void OnAborted(ActionRequest action, Reason reason) 
            => OnRequestAborted?.Invoke(reason); 
        
        public virtual void OnFinished(ActionRequest action) 
            => OnRequestFinished?.Invoke(); 

        // ...
        private AgentController _controller;
    }
}
