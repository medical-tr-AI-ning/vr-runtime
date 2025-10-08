using UnityEngine;
using UnityEngine.UI;

using TMPro;
using MedicalTraining.Configuration;
using System.Collections.Generic;
using Microsoft.CognitiveServices.Speech.Speaker;


public class StartMenu : MonoBehaviour
{
    #region structs
    [System.Serializable]
    private struct UI_Input
    {
        [SerializeField] private TMP_InputField _idInput;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private InputField_Validator _idInputValidator;

        public TMP_InputField IdInput { get => _idInput; set => _idInput = value; }
        public TMP_InputField NameInput { get => _nameInput; set => _nameInput = value; }
        public InputField_Validator IdInputValidator { get => _idInputValidator; set => _idInputValidator = value; }
    }

    [System.Serializable]
    private struct UI_Start
    {
        [SerializeField] private Button _startTutorialButton;
        [SerializeField] private Button _startScenarioButton;
        [SerializeField] private TMP_Text _scenarioDisplayTitle;

        public Button StartTutorialButton { get => _startTutorialButton; set => _startTutorialButton = value; }
        public Button StartScenarioButton { get => _startScenarioButton; set => _startScenarioButton = value; }
        public TMP_Text ScenarioDisplayTitle { get => _scenarioDisplayTitle; set => _scenarioDisplayTitle = value; }
    }
    #endregion

    #region variables
    [SerializeField] private UI_Input _UI_Input;
    [SerializeField] private UI_Start _UI_Start;
    [SerializeField] private GameObject _errorOutputFrame;
    [SerializeField] private TMP_Text _errorOutput;

    [SerializeField] private List<string> _sceneNamesForValueRestore; // TODO: replace with SceneReferences e.g. https://github.com/starikcetin/Eflatun.SceneReference
    #endregion

    #region unity_functions
    private void Awake()
    {
    }

    private void Start()
    {
        // Update display title of selected scenario and enable start button if scenario is selected
        UpdateSelectedScenario();

        // Check if returning from specific previous scene
        if (SceneTracker.PreviousSceneName != null && _sceneNamesForValueRestore.Contains(SceneTracker.PreviousSceneName))
        {
            // prefill input fields
            string studentName;
            string studentID;
            ConfigurationContainer.Instance.GetStudentData(out studentName, out studentID);

            this._UI_Input.NameInput.text = studentName;
            this._UI_Input.IdInput.text = studentID;
        }

        // Clear Student Data from Configuration Container
        ConfigurationContainer.Instance.ClearStudentData();
    }
    #endregion

    #region public_functions
    public void UpdateSelectedScenario()
    {
        // Update the scenario display title
        if (SceneLoader.Instance.IsScenariosDictLoaded() && SceneLoader.Instance.IsScenarioSelected())
        {
            string displayTitle = SceneLoader.Instance.GetSelectedScenario().DisplayTitle;
            _UI_Start.ScenarioDisplayTitle.text = displayTitle;
            _UI_Start.StartScenarioButton.gameObject.SetActive(true);
        }
        else
        {
            _UI_Start.ScenarioDisplayTitle.text = "";
            _UI_Start.StartScenarioButton.gameObject.SetActive(false);
        }

        // Enable Start Tutorial Button
        _UI_Start.StartTutorialButton.gameObject.SetActive(true);
    }

    public void OnStartScenarioClicked()
    {
        if (ValidateAndStoreInput(allowEmptyInputs: false))
        {
            SceneLoader.Instance.LoadSelectedScenario();
            this.DisableStartButtons();
        }
    }

    public void OnStartTutorialClicked()
    {
        if (ValidateAndStoreInput(allowEmptyInputs: false))
        {
            SceneLoader.Instance.LoadTutorialScene();
            this.DisableStartButtons();
        }
    }
    #endregion

    #region helper_functions
    private void DisableStartButtons()
    {
        _UI_Start.StartTutorialButton.gameObject.SetActive(false);
        _UI_Start.StartScenarioButton.gameObject.SetActive(false);
    }

    private bool ValidateAndStoreInput(bool allowEmptyInputs = false)
    {
        // read student name and id
        string studentName = this._UI_Input.NameInput.text;
        string studentId = this._UI_Input.IdInput.text;

        // Empty Input allowed
        if (allowEmptyInputs && studentName == "" && studentId == "")
        {
            this._errorOutput.text = "";
            return true;
        }
        // Empty Input not allowed
        else
        {
            if (studentName == "" || studentId == "")
            {
                this._errorOutput.text = "Bitte Name und ID eingeben";
                _errorOutputFrame.SetActive(true);
                return false;
            }
            else if (this._UI_Input.IdInputValidator && !this._UI_Input.IdInputValidator.isValid)
            {
                this._errorOutput.text = "Bitte ID überprüfen";
                _errorOutputFrame.SetActive(true);
                return false;
            }
            else
            {
                this._errorOutput.text = "";
                ConfigurationContainer.Instance.SetStudentData(studentName, studentId);
                return true;
            }
        }
    }
    #endregion
}
