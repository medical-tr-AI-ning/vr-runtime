using System;
using System.Collections;
using MedicalTraining.AgentBehavior;
using MedicalTraining.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MedicalTraining.Dialogue.ActionHandlers
{
    /// <summary>
    /// A default action handler which has no scene-specific actions.
    /// </summary>
    public class DialogueUISkinCancerScreeningActionHandler : DialogueUIActionHandler
    {
        [SerializeField] private TakePhoto _dermatoscope;
        [SerializeField] private AgentController _agentController;
        [SerializeField] private HelpOverlay _helpOverlay;

        //Conditions
        private static readonly string SAVE_PHOTO_AVAILABLE_CONDITION = "save_photo_available";
        private static readonly string AGENT_NAKED_CONDITION = "agent_naked";

        public override void InvokeSceneAction(string action)
        {
            switch (action)
            {
                case "save-photo":
                    _dermatoscope.SavePhoto();
                    break;
                case "request-help":
                    _helpOverlay.SetOverlayVisible(true);
                    break;
            }
        }

        public void Start()
        {
            setupSubscriptions();
            _agentController = getActiveAgentController();
        }

        public void SetDermatoscope(TakePhoto takePhoto)
        {
            _dermatoscope = takePhoto;
        }

        private void setupSubscriptions()
        {
            _dermatoscope.SavePhotoAvailable += () => TriggerCondition(SAVE_PHOTO_AVAILABLE_CONDITION, true);
            _dermatoscope.SavePhotoUnavailable += () => TriggerCondition(SAVE_PHOTO_AVAILABLE_CONDITION, false);
            AgentNaked += () => TriggerCondition(AGENT_NAKED_CONDITION, true);
            AgentClothed += () => TriggerCondition(AGENT_NAKED_CONDITION, false);

        }

        private AgentController getActiveAgentController()
        {
            GameObject patient = GameObject.Find("/SceneRoot/Agents/Patient");
            if (!patient)
            {
                Debug.LogError("No Patient Node found. Are you in the right scene?");
                return null;
            }

            return patient.GetComponent<AgentController>();
        }

        public void OnClothingChangeFinished()
        {
            ClothingStateChanged?.Invoke();
            if (GetClothingState() == ClothingState.Naked)
                AgentNaked?.Invoke();
            else
                AgentClothed?.Invoke();
        }

        public delegate void ClothingStateChangedEvent();

        public event ClothingStateChangedEvent ClothingStateChanged;

        public delegate void AgentClothedEvent();

        public event AgentClothedEvent AgentClothed;

        public delegate void AgentNakedEvent();

        public event AgentNakedEvent AgentNaked;

        public ClothingState GetClothingState() => _agentController.ClothingState;


        public override void ExitScene()
        {
            SceneLoader.Instance.LoadMainMenuScene();
        }
    }
}