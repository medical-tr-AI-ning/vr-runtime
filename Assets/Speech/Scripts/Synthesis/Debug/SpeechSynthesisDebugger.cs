using Microsoft.CognitiveServices.Speech;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Speech.Scripts.Synthesis {
    public class SpeechSynthesisDebugger : MonoBehaviour
    {
        [SerializeField] private SpeechOutputController _controller;
    
        [SerializeField] private string TextToSynthesize = "";
        [SerializeField] private bool Synthesize = false;

        private void OnValidate()
        {
            if (Synthesize && Application.isPlaying)
            {
                Synthesize = false;
                _controller.Speak(new Utterance() { Text = TextToSynthesize });
            }
        }
    }
}