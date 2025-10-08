using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    /// <summary>
    /// This class provides the OnActionStarted and OnActionStopped events
    /// in this singleton class to react to started and stopped actions of the agent.
    /// </summary>
    public class ActionAdapter : IActionRequestStateObserver
    {
        private static ActionAdapter instance;
        public static ActionAdapter Instance {
            get { return instance ??= new ActionAdapter(); }
        }

        public void OnStarted(ActionRequest action)
        {
            OnActionStarted?.Invoke();
        }

        public void OnAborted(ActionRequest action, Reason reason)
        {
            OnActionStopped?.Invoke();
        }

        public void OnFinished(ActionRequest action)
        {
            OnActionStopped?.Invoke();
        }
        
        public delegate void OnActionStartedEvent();

        public event OnActionStartedEvent OnActionStarted;
        
        public delegate void OnActionStoppedEvent();

        public event OnActionStoppedEvent OnActionStopped;
    }
}