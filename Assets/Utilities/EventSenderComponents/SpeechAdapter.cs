using MedicalTraining.AgentBehavior;
using UnityEngine;

namespace MedicalTraining.Utilities
{
    /// <summary>
    /// Can listen to SpeechEvents and forward the triggered speech-keyword to a synthesizer.
    /// </summary>
    public class SpeechAdapter : IPerceptionEventStateObserver
    {
        private static SpeechAdapter instance;
        public static SpeechAdapter Instance {
            get { return instance ??= new SpeechAdapter(); }
        }
        
        public void OnAccepted(PerceptionEvent perceptionEvent)
        {
            if (perceptionEvent is SpeechEvent speechEvent)
            {
                string keystring = speechEvent.Info.RawText;
                DialogueController.Instance.ExecuteDialogueOption(keystring);
                Debug.Log($"Triggered speech with key: {keystring}");
            }
        }
    }
}