using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Speech.Scripts.Synthesis
{
    public class SpeechSystemAgentSetup : MonoBehaviour
    {
        [SerializeField] AudioSource _agentAudioSource;
        [SerializeField] SubtitleContainer _agentSubtitleContainer;

        public AudioSource AgentAudioSource { get => _agentAudioSource; }
        public SubtitleContainer AgentSubtitleContainer { get => _agentSubtitleContainer; }
    }
}
