using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Speech.Scripts.Synthesis;
using Newtonsoft.Json.Linq;
using MedicalTraining.Configuration;
using MedicalTraining.Configuration.Mapping;
using static medicaltraining.assetstore.ScenarioConfiguration.Serialization.PathologyVariant;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;


public class DialogueController : MonoBehaviour
{
    #region variables
    public static DialogueController Instance { get; private set; }

    [SerializeField] private TextAsset _dialogueConfigFallback;
    [SerializeField] private int _bufferLength = 1;
    [SerializeField] private bool _overwriteIfDifferent = true;

    private Dictionary<string, DialogueOption> _dialogueOptions;
    private string _lastExecutedDialogueOption = "";
    private SpeechSynthesizerController _speechSynthesizerController;
    public SpeechSynthesizerController SpeechSynthesizerController { get => _speechSynthesizerController; }

    [Header("Debug")]
    [SerializeField, ReadOnly] GameObject _currentAgent;
    [SerializeField, ReadOnly] string[] _options;
    [SerializeField] bool _updateOptionsList = false;
    [Space(20)]
    [SerializeField] string _dialogueOptionString;
    [SerializeField] bool _execute = false;
    #endregion

    #region unity_functions
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        this.ReadDialogueOptions();
        this._speechSynthesizerController = GetComponentInChildren<SpeechSynthesizerController>();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            _execute = false;
            _updateOptionsList = false;
            return;
        }
        if (_execute)
        {
            _execute = false;
            ExecuteDialogueOption(_dialogueOptionString);
        }
        if (_updateOptionsList)
        {
            _updateOptionsList = false;
            _options = _dialogueOptions.Keys.ToArray();
        }        
    }
    #endregion

    #region public_functions
    /// <summary>
    /// Executes a dialogue option.
    /// </summary>
    /// <param name="optionId">Option to be executed.</param>
    public void ExecuteDialogueOption(string optionId)
    {
        DialogueOption dialogueOption = this._dialogueOptions[optionId];

        if (!_currentAgent)
        {
            Debug.LogError("No Agent specified for output.", this);
            return;
        }
        if (!_currentAgent.activeInHierarchy)
        {
            Debug.LogError("Specified agent is not active in the scene.", this);
            return;
        }

        // synthesize auditory response (and show subtitle)
        if (dialogueOption.HasResponse)
        {
            this.TriggerResponse(dialogueOption);
        }
    }

    public void SetNewAgent(GameObject agentRoot)
    {
        SpeechSystemAgentSetup agentSetup = agentRoot?.GetComponentInChildren<SpeechSystemAgentSetup>(true);
        if (!agentSetup)
        {
            Debug.LogError("Can't switch agent. Specified Agent does not contain a SpeechSystemAgentSetup component.", this);
            return;
        }
        _speechSynthesizerController.AudioOutputConfig.outputPatient = agentSetup.AgentAudioSource;
        _speechSynthesizerController.SubtitleContainer = agentSetup.AgentSubtitleContainer;

        _currentAgent = agentRoot;
    }
    #endregion

    #region helper_functions
    private void ReadDialogueOptions()
    {
        this._dialogueOptions = new();

        // Load dialogue options from configuration
        // TODO: Load from different variant if UseUnifiedAnamnesisData is set to true
        AnamnesisData anamnesesData = ConfigurationContainer.Instance.Scenario.Pathology.Anamnesis;
        if (anamnesesData != null)
        {
            foreach (AnamnesisQuestion anamnesis in anamnesesData.AnamnesisQuestions)
            {
                string optionId = anamnesis.QuestionID;
                string response = anamnesis.AnswerText;
                // Skip if response is empty
                if (string.IsNullOrEmpty(response))
                {
                    Debug.Log("Empty response for dialogue option: " + optionId);
                    continue;
                }

                // Multiple responses are supported by the runtime but not by the VariableScenarioConfiguration specification
                string[] responses = new string[] { response };
                DialogueOption option = new(responses);

                this._dialogueOptions.Add(optionId, option);
                Debug.Log("Added dialogue option: " + optionId);
            }
        }
        else
        {
            Debug.LogError("Anamnesis data not found in ConfigurationContainer. Can't read dialogue options.", this);
            Debug.Log("Using only fallback dialogueOptions instead.");
        }

        // Load fallback dialogue options for all missing options
        // TODO: Convert fallback to the new configuration format

        string dialogueConfigText = this._dialogueConfigFallback.text;
        JObject dialogueOptionsJson = JObject.Parse(dialogueConfigText);

        foreach (KeyValuePair<string, JToken> item in dialogueOptionsJson)
        {
            string optionId = item.Key;

            // Skip if option already exists
            if (this._dialogueOptions.ContainsKey(optionId))
            {
                continue;
            }

            JToken responsesJson = item.Value;
            string[] responses = null;
            if (responsesJson.Type == JTokenType.Array)
            {
                var responsesArray = (JArray)responsesJson;
                int responseCount = responsesArray.Count;
                responses = new string[responseCount];
                int responseIndex = 0;
                IEnumerator<JToken> responseEnumerator = responsesArray.GetEnumerator();
                while (responseEnumerator.MoveNext())
                {
                    responses[responseIndex] = responseEnumerator.Current.Value<string>();
                    responseIndex += 1;
                }

            }
            DialogueOption option = new(responses);
            Debug.Log("Added dialogue option from fallback: " + optionId);

            this._dialogueOptions.Add(optionId, option);
        }
    }

    private void TriggerResponse(DialogueOption dialogueOption)
    {
        string tmpLastExecutedDialogueOption = _lastExecutedDialogueOption;
        _lastExecutedDialogueOption = dialogueOption.Response;

        bool overwrite = false;
        this._speechSynthesizerController.MaxOutputListLength = _bufferLength;

        if (_speechSynthesizerController.IsTalking)
        {
            if (dialogueOption.Response != tmpLastExecutedDialogueOption)
            {
                if(_overwriteIfDifferent) overwrite = true;
            }
            else
            {
                return;
            }
        }
        Debug.Log("Speaking: " + dialogueOption.Response);
        this._speechSynthesizerController.Speak(dialogueOption.Response, overwrite);
    }
    #endregion

}
