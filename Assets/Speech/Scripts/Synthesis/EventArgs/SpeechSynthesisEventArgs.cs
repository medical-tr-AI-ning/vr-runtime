namespace Speech.Scripts.Synthesis
{
    public class SpeechSynthesisEventArgs
    {
        public string UtteranceText { get; set; }

        public Voice Voice { get; set; }

        public string FilePath { get; set; }
    }
}
