using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Speech.Scripts
{
    // TODO: Move api keys to seperate file so it can get injected during build process
    public static class SpeechCloudSettings
    {
        public const string luisApiKey = "5a50c8d78c4347ba9ffb8d5191649218";

        public const string cognitiveServicesApiKey = "31eb64aa0cf7492fa12200ff2959dd6f";

        public const string azureRegion = "westeurope";

        public const string languageModelAppId = "5450f5a0-7a30-4617-8889-dd1ea0664bc0";

        public const string language = "de-DE";

        public const string synthesizerApiKey = "31eb64aa0cf7492fa12200ff2959dd6f";

        public static SpeechConfig Config
        {
            get
            {
                var config = SpeechConfig.FromSubscription(luisApiKey, azureRegion);
                config.SpeechRecognitionLanguage = language;
                config.SpeechSynthesisLanguage = language;

                return config;
            }
        }

        public static AudioConfig InputAudioConfig => AudioConfig.FromDefaultMicrophoneInput();
    }
}
