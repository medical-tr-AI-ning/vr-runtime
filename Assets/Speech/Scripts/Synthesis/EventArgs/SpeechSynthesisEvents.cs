namespace Speech.Scripts.Synthesis
{
    public class SpeechSynthesisSuccessEventArgs
    {
        public string UtteranceText { get; set; }

        public Voice Voice { get; set; }

        public string FilePath { get; set; }
    }
    
    public class SpeechSynthesisFailedEventArgs
    {
        public string UtteranceText { get; set; }
        public string ErrorMessage { get; set; }
    }
}
