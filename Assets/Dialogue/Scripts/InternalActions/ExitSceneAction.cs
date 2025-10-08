namespace MedicalTraining.Dialogue.InternalActions
{
    public class ExitSceneAction : InternalAction
    {
        public float DelayBeforeExitingInSeconds;

        public override void InvokeAction()
        {
            _actionHandler.Invoke(nameof(_actionHandler.ExitScene), DelayBeforeExitingInSeconds);
        }
    }
}