using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Globalization;

namespace Speech.Scripts.Synthesis
{
    public class ElevenLabsSpeechSynthesizer : BaseSpeechSynthesizer
    {
        #region variables
        [SerializeField] private string _ApiKey = "<API-KEY>";
        [SerializeField] private string _voiceIDDoctor = "SAz9YHcvj6GT2YYXdXww";   // Voice: River
        [SerializeField] private string _voiceIDPatient = "SMQKgThKQvXmuAzjIP2x";  // Voice: Flauschi
        [SerializeField] private string _modelID = "eleven_multilingual_v2";       // eleven_multilingual_v2
        [SerializeField] private double _stability = 0.5;                          // Default: 0.5
        [SerializeField] private double _similarityBoost = 0.75;                   // Default: 0.75
        [SerializeField] private double _style = 0.0;                                // Default: 0
        [SerializeField] private bool _use_speaker_boost = true;                   // Default: true

        protected override string CacheDirName => Path.Combine("ElevenLabsSpeechCache");
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
                Debug.Log("Synthesizing utterance: " + utterance.Text);

                var url = $"https://api.elevenlabs.io/v1/text-to-speech/{utterance.VoiceID}";

                // Convert float/double to string with dot as decimal separator (instead of comma)
                string stability = _stability.ToString("F2", CultureInfo.InvariantCulture);
                string similarityBoost = _similarityBoost.ToString("F2", CultureInfo.InvariantCulture);
                string style = _style.ToString("F2", CultureInfo.InvariantCulture);
                string use_speaker_boost = _use_speaker_boost ? "true" : "false";
                var data = $"{{\"text\": \"{utterance.Text}\", \"model_id\": \"{_modelID}\", \"voice_settings\": " +
                    $"{{\"stability\": {stability}, \"similarity_boost\": {similarityBoost}, \"style\": {style}, \"use_speaker_boost\": {use_speaker_boost}}}}}";
                Debug.Log("Data: " + data);

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Meditrain");

                httpClient.DefaultRequestHeaders.Add("xi-api-key", _ApiKey);
                Debug.Log("Headers: " + httpClient.DefaultRequestHeaders);

                var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
                Debug.Log("Content: " + httpContent);

                var response = await httpClient.PostAsync(url, httpContent);
                Debug.Log("Response: " + response);

                using (var result = await httpClient.PostAsync(url, httpContent))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        HandleSpeechSynthesisChunk(await response.Content.ReadAsByteArrayAsync(), utterance);
                    }
                    else
                    {
                        Debug.LogError($"Speech Response failed {response.StatusCode}");
                    }
                }
            }
        }
        #endregion
    }
}
