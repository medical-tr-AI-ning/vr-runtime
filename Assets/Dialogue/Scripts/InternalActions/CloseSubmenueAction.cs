namespace MedicalTraining.Dialogue.InternalActions
{
    public class CloseSubmenueAction : InternalAction
    {

        public override void InvokeAction()
        {
            _actionHandler.CloseSubmenue();
        }
    }
}