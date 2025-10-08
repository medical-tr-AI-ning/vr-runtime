using MedicalTraining.Dialogue.Utilities;

namespace MedicalTraining.Dialogue
{
    /// <summary>
    /// A DialogueSubmenue is a DialogueOptionList with additional capabilities for fading in and out.
    /// </summary>
    public class DialogueSubmenue : DialogueOptionList
    {
        public string SubmenueName;
        private Fadeable _fadeable;

        private new void Awake()
        {
            _fadeable = GetComponent<Fadeable>();
            base.Awake();
        }

        public void Show()
        {
            _fadeable.FadeIn();
            OnFocus();
        }


        public void Hide()
        {
            _fadeable.FadeOut();
        }
    }
}