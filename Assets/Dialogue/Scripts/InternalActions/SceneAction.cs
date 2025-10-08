namespace MedicalTraining.Dialogue.InternalActions
{
    public class SceneAction : InternalAction
    {
        public string Action;
        public override void InvokeAction()
        {
            _actionHandler.InvokeSceneAction(Action);
        }
    }
}