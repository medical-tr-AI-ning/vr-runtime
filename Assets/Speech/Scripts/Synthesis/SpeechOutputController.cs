using System.Collections.Generic;
using UnityEngine;

namespace Speech.Scripts.Synthesis
{
    public abstract class SpeechOutputController : MonoBehaviour
    {
        #region variables
        // Member variables
        //Requested Synthesis Actions
        protected Queue<Utterance> utterances;

        // Events
        public event SpeechOutputEvent SpeechEnded;
        #endregion

        #region unity_functions
        protected virtual void Start()
        {
            utterances = new Queue<Utterance>();
        }
        #endregion

        #region public_functions
        /// <summary>
        /// Function to speech synthesize utterance containing text
        /// </summary>
        /// <param name="utterance">Utterance object containing text to synthesize</param>
        public void Speak(Utterance utterance)
        {
            utterances.Enqueue(utterance);
        }

        /// <summary>
        /// Function to speech synthesize text string
        /// </summary>
        /// <param name="text">string object containing text to synthesize</param>
        public void Speak(string text)
        {
            Speak(
                new Utterance()
                    {
                        Text = text
                    }
             );
        }

        /// <summary>
        /// Function to stop all speech output
        /// </summary>
        public abstract void StopSpeaking();
        #endregion
        

        protected void EmitSpeechEnded()
        {
            SpeechEnded?.Invoke(this);
        }
    }

    public delegate void SpeechOutputEvent(Object sender);
}