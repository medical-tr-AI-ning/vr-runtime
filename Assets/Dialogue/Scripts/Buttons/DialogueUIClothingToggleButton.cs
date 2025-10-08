using System.Linq;
using MedicalTraining.AgentBehavior;
using MedicalTraining.Dialogue.ActionHandlers;
using UnityEngine;

namespace MedicalTraining.Dialogue.Buttons
{
    public class DialogueUIClothingToggleButton : DialogueUIToggleButton
    {
        [SerializeField] private ClothingState[] _clothingStatesWhereButtonsIsToggled;
        private DialogueUISkinCancerScreeningActionHandler _actionHandler;

        public override void Start()
        {
            base.Start();
            if (DialogueUIActionHandler.Instance is DialogueUISkinCancerScreeningActionHandler
                skinCancerScreeningActionHandler)
            {
                _actionHandler = skinCancerScreeningActionHandler;
                _actionHandler.ClothingStateChanged += UpdateToggledStateToMatchClothing;
            }
        }

        private void UpdateToggledStateToMatchClothing()
        {
            bool shouldToggle = _clothingStatesWhereButtonsIsToggled.Contains(_actionHandler.GetClothingState());
            setToggleState(shouldToggle);
        }

        public override void OnFinished(ActionRequest action)
        {
            if (_actionHandler != null)
            {
                _actionHandler.OnClothingChangeFinished();
            }
            else
            {
                base.OnFinished(action);
            }
        }
    }
}