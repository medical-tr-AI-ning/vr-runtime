namespace MedicalTraining
{
    namespace AgentBehavior
    {
        /// <summary>
        /// The IActionRequestStateObserver is used from the event system to 
        /// inform the requesting unit about the current state of the request.
        /// </summary>
        /// <remarks>
        /// Do not put long or demanding routines in the interface
        /// implementations, as this may block the event processing.
        /// </remarks>
        public interface IActionRequestStateObserver
        {
            /// <summary>
            /// The ActionRequest was accepted. The actual execution
            /// will be scheduled for execution (maybe later in time).
            /// </summary>
            /// <param name="action">The ActionRequest served.</param>
            public virtual void OnAccepted(ActionRequest action) { }

            /// <summary>
            /// The ActionRequest was declined by the Agent.
            /// </summary>
            /// <param name="action">The ActionRequest served.</param>
            public virtual void OnDeclined(ActionRequest action, Reason reason) { }

            /// <summary>
            /// The execution of the request has started.
            /// </summary>
            /// <param name="action">The ActionRequest served.</param>
            public virtual void OnStarted(ActionRequest action) { }

            /// <summary>
            /// The execution of the request was aborted. This can happen before
            /// the request's execution starts (pruned) or after this (aborted) 
            /// </summary>
            /// <param name="action">The ActionRequest served.</param>
            public virtual void OnAborted(ActionRequest action, Reason reason) { }

            /// <summary>
            /// The execution of the request has finished.
            /// </summary>
            /// <param name="action">The ActionRequest served.</param>
            /// <remarks>
            /// OnFinished is not guaranteed to be called directly after the 
            /// ActionRequest was served. It only guarantees it has been completed.
            /// </remarks>
            public virtual void OnFinished(ActionRequest action) { }
        }
    }
}
