using MedicalTraining.Utilities;
using TMPro;

namespace MedicalTraining.Dialogue.Buttons
{
    /// <summary>
    ///  A simple option button with a single action.
    /// </summary>
    public class DialogueUISimpleButton : DialogueUIButton
    {
        private InternalAction _internalAction;
        private ActionRequestSender _actionRequest;
        private PerceptionEventSender _perceptionEvent;
        private TMP_Text _label;

        public override void Start()
        {
            base.Start();
            _internalAction = GetComponent<InternalAction>();
            _actionRequest = GetComponent<ActionRequestSender>();
            _perceptionEvent = GetComponent<PerceptionEventSender>();
            _label = GetComponentInChildren<TMP_Text>();
        }

        public override void InvokeAction()
        {
            logButtonPress(_label, _actionRequest, _perceptionEvent, _internalAction);
            _internalAction?.InvokeAction();
            _actionRequest?.Send(this);
            _perceptionEvent?.Send(this);
        }
    }
}