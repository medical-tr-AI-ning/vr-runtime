using UnityEngine;

namespace MedicalTraining.Dialogue
{
    public abstract class InternalAction : MonoBehaviour
    {
        internal DialogueUIActionHandler _actionHandler;

        private void Start()
        {
            _actionHandler = DialogueUIActionHandler.Instance;
        }
        public abstract void InvokeAction();
    }
}