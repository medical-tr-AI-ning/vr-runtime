using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace Speech.Scripts.Synthesis
{
    [RequireComponent(typeof(AudioOutputConfig))]
    public class SpeechSynthesizerController : SpeechOutputController
    {
        #region variables
        [Header("Component References")]
        [SerializeField] private BaseSpeechSynthesizer _speechSynthesizer;
        [SerializeField] private SubtitleContainer _subtitleContainer;
        public SubtitleContainer SubtitleContainer { get => _subtitleContainer; set => _subtitleContainer = value; }
        private AudioOutputConfig _audioOutputConfig;
        public AudioOutputConfig AudioOutputConfig { 
            get { 
                _audioOutputConfig = _audioOutputConfig != null ? _audioOutputConfig : GetComponent<AudioOutputConfig>(); 
                return _audioOutputConfig; } 
            set => _audioOutputConfig = value; }
        // ---
        // ---
        [Header("Settings")]
        [SerializeField] private bool _withSubtitle = true;
        public bool WithSubtitle { get => _withSubtitle; set => _withSubtitle = value; }
        [SerializeField] private bool _mute = false;
        public bool Mute { get => _mute; set => _mute = value; }        
        // ---
        // ---
        [Header("Status Variables")]
        [ReadOnly, SerializeField] private bool _isPatientTalking;
        public bool IsPatientTalking { get => _isPatientTalking; }
        [ReadOnly, SerializeField] private bool _isDoctorTalking;
        public bool IsDoctorTalking1 { get => _isDoctorTalking; }
        public bool IsDoctorTalking { get; }
        [ReadOnly, SerializeField] private bool _isBuffering;
        public bool IsBuffering { get; }
        public bool IsTalking => _isPatientTalking || _isDoctorTalking;

        // ---
        // ---
        private int _maxConcurrentSynthesis = 5;
        private int _currentConcurrentSynthesis = 0;

        private Queue<SpeechSynthesisEventArgs> _pendingSynthesisOutputs;
        private bool _outputStarted;
        public int pendingOutputs => _pendingSynthesisOutputs.Count + utterances.Count + (IsTalking ? 1 : 0);

        private int _maxOutputListLength = 0;
        public int MaxOutputListLength { get => _maxOutputListLength; set => _maxOutputListLength = value; }
        #endregion

        #region unity_functions
        private void Awake()
        {
            _audioOutputConfig = GetComponent<AudioOutputConfig>();
        }

        protected override void Start()
        {
            base.Start();
            _pendingSynthesisOutputs = new Queue<SpeechSynthesisEventArgs>();
            HideSubtitle();

            Debug.Assert(_speechSynthesizer != null, "Assertion Error: No SpeechSynthesizer set.", this);

            // speech is synthesized in chunks for faster start.
            _speechSynthesizer.SpeechSynthesisChunkCompleted += (s, e) => { 
                _pendingSynthesisOutputs.Enqueue(e);
                _currentConcurrentSynthesis--;
            };
        }

        private void Update()
        {
            // Get current status
            _isPatientTalking = _audioOutputConfig.outputPatient && _audioOutputConfig.outputPatient.isPlaying;
            _isDoctorTalking = _audioOutputConfig.outputDoctor && _audioOutputConfig.outputDoctor.isPlaying;

            if (IsTalking)
            {
                return;
            }
            else
            {
                if (_outputStarted)
                {
                    HideSubtitle();

                    _outputStarted = false;
                    EmitSpeechEnded();
                }
            }

            // Dequeue requests and start synthesis
            if (utterances.Count > 0 && _currentConcurrentSynthesis < _maxConcurrentSynthesis)
            {
                _currentConcurrentSynthesis++;
                var utterance = utterances.Dequeue();

                if (utterance != null)
                {
                    Synthesize(utterance).ConfigureAwait(false);
                }
            }

            // Playback already synthesized requests
            if (!_isBuffering && !IsTalking && _pendingSynthesisOutputs.Count > 0)
            {
                var pendingOutput = _pendingSynthesisOutputs.Dequeue();

                if (pendingOutput != null)
                {
                    if (_withSubtitle)
                    {
                        if (!_subtitleContainer)
                        {
                            Debug.LogWarning("SubtitleContainer reference missing. Can't display subtitles.", this);
                        }
                        _subtitleContainer?.ShowSubtitle(pendingOutput.UtteranceText);
                    }

                    if (!_mute)
                    {
                        PlaybackOutput(pendingOutput);
                    }
                }
            }
        }
        #endregion

        #region public_functions
        public void Speak(Utterance utterance, bool overwrite = false)
        {
            if (overwrite)
            {
                utterances.Clear();
                StopSpeaking();
            }
            Debug.Log("PendingOutputs=" + pendingOutputs);
            if (pendingOutputs < _maxOutputListLength || _maxOutputListLength <= 0)
            {
                base.Speak(utterance);
            }
        }

        public void Speak(string text, bool overwrite = false)
        {
            Speak(
                new Utterance()
                {
                    Text = text
                },
                overwrite
             );
        }

        public void HideSubtitle()
        {
            _subtitleContainer?.HideSubtitle();
        }

        public override void StopSpeaking()
        {
            if (_audioOutputConfig.outputPatient)
            {
                _audioOutputConfig.outputPatient.Stop();
                _isPatientTalking = false;
            }
            if (_audioOutputConfig.outputDoctor)
            {
                _audioOutputConfig.outputDoctor.Stop();
                _isDoctorTalking = false;
            }
            if (SubtitleContainer) _subtitleContainer.HideSubtitle();
            _outputStarted = false;
            StopAllCoroutines();

            EmitSpeechEnded();
        }
        #endregion

        #region helper_functions
        private async Task Synthesize(Utterance utterance)
        {
            await _speechSynthesizer.Synthesize(utterance);
        }

        private void PlaybackOutput(SpeechSynthesisEventArgs arg)
        {
            StartCoroutine(SetAudioClip(arg.FilePath, arg.Voice));
        }

        private IEnumerator SetAudioClip(string filePath, Voice voice, AudioType fileType = AudioType.MPEG)
        {
            _outputStarted = false;
            _isBuffering = true;

            //Debug.Log($"Trying to get audio clip... {filePath}");
            using var www = UnityWebRequestMultimedia.GetAudioClip(filePath, fileType);
            
            //Debug.Log($"Sending web-request!");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Loading did not succeed: {www.error}");
            }

            var content = DownloadHandlerAudioClip.GetContent(www);

            var output = voice == Voice.patient ? _audioOutputConfig.outputPatient : _audioOutputConfig.outputDoctor;

            output.clip = content;
            output.Play();

            _isBuffering = false;
            _outputStarted = true;
        }
        #endregion
    }
}