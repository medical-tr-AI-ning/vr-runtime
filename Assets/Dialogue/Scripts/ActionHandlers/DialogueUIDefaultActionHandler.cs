using UnityEngine;

namespace MedicalTraining.Dialogue.ActionHandlers
{
    /// <summary>
    /// A default action handler which has no scene-specific actions.
    /// </summary>
    public class DialogueUIDefaultActionHandler : DialogueUIActionHandler
    {
        public override void InvokeSceneAction(string action)
        {
            Debug.Log($"{action} was called!");
        }
    }
}