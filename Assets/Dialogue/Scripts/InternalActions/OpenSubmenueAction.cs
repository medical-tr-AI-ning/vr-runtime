namespace MedicalTraining.Dialogue.InternalActions
{
    public class OpenSubmenueAction : InternalAction
    {
        public string Submenue;

        public override void InvokeAction()
        {
            _actionHandler.OpenSubmenue(Submenue);
        }
    }
}