using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace Speech.Scripts.Synthesis
{
    public class AzureSpeechSynthesizer : BaseSpeechSynthesizer
    {
        #region variables
        [SerializeField] private string _voiceIDDoctor = "de-DE-KatjaNeural";
        [SerializeField] private string _voiceIDPatient = "de-DE-ConradNeural";

        // Microsoft Azure
        private SpeechSynthesizer _synthesizer;
        private Connection _connection;
        private SpeechConfig _synthesizerConfig;

        protected override string CacheDirName => Path.Combine("AzureSpeechCache"); 
        #endregion

        #region unity_functions
        protected override void Start()
        {
            base.Start();

            _synthesizerConfig = SpeechConfig.FromSubscription(SpeechCloudSettings.synthesizerApiKey, SpeechCloudSettings.azureRegion);
            _synthesizerConfig.SpeechSynthesisLanguage = SpeechCloudSettings.language;
            _synthesizerConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio24Khz48KBitRateMonoMp3);

            _synthesizer = new SpeechSynthesizer(_synthesizerConfig, null);
            _connection = Connection.FromSpeechSynthesizer(_synthesizer);
        }

        private void OnDestroy()
        {
            _connection.Dispose();
            _synthesizer.Dispose();
        }
        #endregion

        protected override async Task SynthesizeImpl(Utterance utterance)
        {
            // Set VoiceID for hash calculation
            utterance.VoiceID = utterance.VoiceType == Voice.patient ? _voiceIDPatient : _voiceIDDoctor;

            if (isCached(utterance))
            {
                Debug.Log("Utterance already cached. Using local file.");
                HandleSpeechSynthesisCached(utterance);
            }
            else
            {
            Debug.Log("Utterance not cached. Requesting online synthesis.");
            var document = utterance.ToSSML(utterance.VoiceType == Voice.patient ? _voiceIDPatient : _voiceIDDoctor);
                
                using (var result = await _synthesizer.SpeakSsmlAsync(document))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        HandleSpeechSynthesisChunk(result.AudioData, utterance);
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Debug.LogWarning($"Speech Synthetization canceled: Reason={cancellation.Reason}", this);

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Debug.LogError($"Speech Synthetization canceled: ErrorCode={cancellation.ErrorCode}", this);
                            Debug.LogError($"Speech Synthetization canceled: ErrorDetails=[{cancellation.ErrorDetails}]", this);
                        }
                    }
                }
            }
        }
    }
}
