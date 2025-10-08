using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining.Dialogue
{
    /// <summary>
    /// An ActionHandler implements the actions which are triggered by the dialogue options
    /// for a specific scene.
    /// </summary>
    /// <remarks>This is Singleton. The currently active IDialogueActionHandler can be retrieved with
    /// <code>IDialogueUIActionHandler.Instance</code></remarks>
    public abstract class DialogueUIActionHandler : MonoBehaviour
    {
        public static DialogueUIActionHandler Instance { get; private set; }
        internal Dictionary<string, List<DialogueUIButton>> _interactableConditionMap;
        internal DialogueUIController _uiController;

        internal virtual void Awake()
        {
            // Singleton Guard

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            _interactableConditionMap = new Dictionary<string, List<DialogueUIButton>>();
            _uiController = FindObjectOfType<DialogueUIController>();
        }

        /// <summary>
        /// Handles scene-specific actions invoked by the option buttons.
        /// </summary>
        /// <param name="action">A specific command (e.g 'system.back')</param>
        public abstract void InvokeSceneAction(string action);


        /// <summary>
        /// Allows buttons to subscribe to a certain condition string. If the condition state is changed
        /// to true/false, the button will be set to interactable/non-interactable.
        /// </summary>
        public void SubscribeToInteractableCondition(string interactableCondition, DialogueUIButton button)
        {
            if (!_interactableConditionMap.ContainsKey(interactableCondition))
                _interactableConditionMap
                    [interactableCondition] = new List<DialogueUIButton>();
            _interactableConditionMap[interactableCondition].Add(button);
        }

        /// <summary>
        /// Sets the interactable-property of all buttons which are subscribed
        /// to parameter 'condition' to the value of the parameter 'interactable'
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="interactable"></param>
        public void TriggerCondition(string condition, bool interactable)
        {
            if (!_interactableConditionMap.ContainsKey(condition)) return;
            foreach (DialogueUIButton button in _interactableConditionMap[condition])
            {
                button.SetInteractable(interactable);
            }
        }

        #region Common Actions

        public void OpenSubmenue(string submenue)
        {
            _uiController.OpenSubmenue(submenue);
        }

        public void CloseSubmenue()
        {
            _uiController.CloseSubmenue();
        }
        
        public virtual void ExitScene()
        {
          
        }

        #endregion
        
    }
}