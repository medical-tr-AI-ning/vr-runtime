using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

namespace MedicalTraining.Utilities
{
    public class SendSpeechEvent : PerceptionEventSender
    {
        // Inspector
        [SerializeField] private SpeechEventType Type;
        [SerializeField] private int Identifier;
        [SerializeField] private string Text;

        // interface
        protected override PerceptionEvent CreatePerceptionEvent(IPerceptionEventStateObserver observer)
        {
            var speechEvent = new SpeechEvent(observer, Type, Identifier, Text);
            speechEvent.AddObserver(SpeechAdapter.Instance);
            return speechEvent;
        }

        public override void Send(IPerceptionEventStateObserver observer)
        {
            //hack sk: Close mouth before speaking
            var closeMouthEvent = new CloseMouth(null);
            _controller.notify(closeMouthEvent);
            base.Send(observer);
        }
    }
}
