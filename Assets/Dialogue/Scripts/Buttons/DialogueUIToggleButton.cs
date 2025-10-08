using MedicalTraining.AgentBehavior;
using MedicalTraining.Utilities;
using TMPro;

namespace MedicalTraining.Dialogue.Buttons
{
    /// <summary>
    /// An option button with two alternating actions. (Toggle)
    /// </summary>
    public class DialogueUIToggleButton : DialogueUIButton
    {
        public InternalAction ToggledInternalAction;
        public InternalAction DefaultInternalAction;
        private InternalAction _activeInternalAction;

        public ActionRequestSender DefaultActionRequestSender;
        public ActionRequestSender ToggledActionRequestSender;
        private ActionRequestSender _activeActionRequestSender;

        public PerceptionEventSender DefaultPerceptionEventSender;
        public PerceptionEventSender ToggledPerceptionEventSender;
        private PerceptionEventSender _activePerceptionEventSender;

        public string DefaultLabel;
        public string ToggledLabel;
        private bool _toggled;

        private TextMeshProUGUI _label;

        public override void Start()
        {
            base.Start();
            _label = GetComponentInChildren<TextMeshProUGUI>();
            _activeInternalAction = DefaultInternalAction;
            _activeActionRequestSender = DefaultActionRequestSender;
            _activePerceptionEventSender = DefaultPerceptionEventSender;
        }

        public override void InvokeAction()
        {
            _activeInternalAction?.InvokeAction();
            _activeActionRequestSender?.Send(this);
            _activePerceptionEventSender?.Send(this);
            logButtonPress(_label, _activeActionRequestSender, _activePerceptionEventSender, _activeInternalAction);

            // If there are no attached actions (except Internal Actions),
            // don't wait for callbacks to change toggle state
            if (_activeActionRequestSender == null && _activePerceptionEventSender == null)
                toggleState();
        }

        private void toggleState()
        {
            setToggleState(!_toggled);
        }

        internal void setToggleState(bool toggled)
        {
            _toggled = toggled;
            _activeInternalAction = toggled ? ToggledInternalAction : DefaultInternalAction;
            _activeActionRequestSender = toggled ? ToggledActionRequestSender : DefaultActionRequestSender;
            _activePerceptionEventSender = toggled ? ToggledPerceptionEventSender : DefaultPerceptionEventSender;
            _label.text = toggled ? ToggledLabel : DefaultLabel;
        }

        public override void OnFinished(ActionRequest action)
        {
            toggleState();
        }
        /*
        public override void OnPerceived(PerceptionEvent perceptionEvent)
        {
            toggleState();
        }
        */
    }
}