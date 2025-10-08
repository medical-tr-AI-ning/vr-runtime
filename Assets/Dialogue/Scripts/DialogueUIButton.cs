using System.Text;
using MedicalTraining.AgentBehavior;
using MedicalTraining.Configuration;
using MedicalTraining.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MedicalTraining.Dialogue
{
    /// <summary>
    /// Handles the input events and styling for a particular type of button.
    /// </summary>
    public abstract class DialogueUIButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler,
        IActionRequestStateObserver, IPerceptionEventStateObserver
    {
        //Game Objects References
        private Button _button;
        private TextMeshProUGUI[] _textElements;
        private GameObject _shadow;
        private CanvasGroup _canvasGroup;
        private Sprite _defaultSprite;
        private ConfigurationContainer _configurationContainer;

        //Object State
        private bool selected;
        public bool Disabled;

        //Style Properties
        public Color DefaultTextColor;
        public Color SelectedTextColor;
        public float DisabledOpacity;

        //Functional Properties
        public string InteractableCondition;

        public virtual void Start()
        {
            //Component Reference Setup
            _configurationContainer = ConfigurationContainer.Instance;
            _button = GetComponent<Button>();
            _shadow = transform.Find("Shadow").gameObject;
            _textElements = GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true);
            _canvasGroup = GetComponent<CanvasGroup>();
            _defaultSprite = ((Image)_button.targetGraphic).sprite;

            _button.interactable = !Disabled;
            _button.onClick.AddListener(InvokeAction);
            if (!string.IsNullOrEmpty(InteractableCondition))
                DialogueUIActionHandler.Instance.SubscribeToInteractableCondition(InteractableCondition, this);
            updateStyling();
        }

        public void SetInteractable(bool interactable)
        {
            bool wasSelected = gameObject.Equals(EventSystem.current.currentSelectedGameObject);
            Disabled = !interactable;
            _button.interactable = interactable;
            //making the currently selected button non-interactable loses the selection, so we reselect afterwards
            if (wasSelected) _button.Select();
            updateStyling();
        }

        public void ShowShadow()
        {
            _shadow.SetActive(true);
        }

        internal void logButtonPress(TMP_Text label, ActionRequestSender actionRequest,
            PerceptionEventSender perceptionEvent, InternalAction internalAction)
        {
            var logger = _configurationContainer.GetLogger();
            if (logger == null) return;
            StringBuilder actionSb = new StringBuilder();
            if (actionRequest) actionSb.Append(actionRequest.GetType().Name);
            if (perceptionEvent) actionSb.Append(perceptionEvent.GetType().Name);
            if (internalAction) actionSb.Append(internalAction.GetType().Name);
            if(label) logger.WriteEvent("DialogueUI", actionSb.ToString(), label.text);
            else logger.WriteEvent("DialogueUI", actionSb.ToString());
        }

        private void updateStyling()
        {
            Color textColor = selected ? SelectedTextColor : DefaultTextColor;
            foreach (TextMeshProUGUI textElement in _textElements)
                textElement.color = textColor;

            //Unity doesn't appropriately update the sprites of non-interactable buttons
            //so we have to do it manually.

            ((Image)_button.targetGraphic).sprite =
                selected ? _button.spriteState.selectedSprite : _defaultSprite;

            _canvasGroup.alpha = Disabled ? DisabledOpacity : 1;
        }

        public abstract void InvokeAction();

        #region Unity Input Event Handlers

        public void OnSubmit(BaseEventData eventData)
        {
            if (Disabled) return;
            //There no way to get a SubmitRelease Event, so we'll just reactivate the shadow after x time
            _shadow.SetActive(false);
            Invoke(nameof(ShowShadow), 0.1f);
        }

        public void OnSelect(BaseEventData eventData)
        {
            selected = true;

            updateStyling();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            selected = false;
            updateStyling();
        }

        #endregion

        #region Action Request Callbacks

        public virtual void OnAccepted(ActionRequest action)
        {
        }

        public virtual void OnDeclined(ActionRequest action, Reason reason)
        {
        }

        public virtual void OnStarted(ActionRequest action)
        {
        }

        public virtual void OnAborted(ActionRequest action, Reason reason)
        {
        }

        public virtual void OnFinished(ActionRequest action)
        {
        }

        #endregion

        #region Perception Event Callbacks

        public virtual void OnPerceived(PerceptionEvent perceptionEvent)
        {
        }

        public virtual void OnIgnored(PerceptionEvent perceptionEvent, Reason reason)
        {
        }

        public virtual void OnAccepted(PerceptionEvent perceptionEvent)
        {
        }

        public virtual void OnDeclined(PerceptionEvent perceptionEvent, Reason reason)
        {
        }

        #endregion
    }
}