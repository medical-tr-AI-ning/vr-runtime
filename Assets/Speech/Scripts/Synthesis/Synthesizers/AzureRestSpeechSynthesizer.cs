using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Speech.Scripts.Synthesis
{
    public class AzureRestSpeechSynthesizer : BaseSpeechSynthesizer
    {
        #region variables
        [SerializeField] private string _voiceIDDoctor = "de-DE-KatjaNeural";
        [SerializeField] private string _voiceIDPatient = "de-DE-ConradNeural";

        protected override string CacheDirName => "AzureRestSpeechCache";
        #endregion

        #region unity_functions
        protected override void Start()
        {
            base.Start();
        }
        #endregion

        #region public_functions
        protected override async Task SynthesizeImpl(Utterance utterance)
        {
            var document = utterance.ToSSML(utterance.VoiceType == Voice.patient ? _voiceIDPatient : _voiceIDDoctor);

            var url = $"https://{SpeechCloudSettings.azureRegion}.tts.speech.microsoft.com/cognitiveservices/v1";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Meditrain");

            var httpContent = new StringContent(document, Encoding.UTF8, "application/ssml+xml");
            httpContent.Headers.Add("Ocp-Apim-Subscription-Key", SpeechCloudSettings.synthesizerApiKey);
            httpContent.Headers.Add("X-Microsoft-OutputFormat", "audio-24khz-48kbitrate-mono-mp3");

            var response = await httpClient.PostAsync(url, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                // TODO: More resilient error handling
                Debug.LogError($"Speech Response failed {response.StatusCode}");

                return;
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();

            HandleSpeechSynthesisChunk(bytes, utterance);
        }
        #endregion
    }
}