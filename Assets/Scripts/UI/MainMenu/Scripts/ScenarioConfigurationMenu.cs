using UnityEngine;
using UnityEngine.UI;

using TMPro;
using MedicalTraining.Configuration;
using SFB;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.UI;


public class ScenarioConfigurationMenu : MonoBehaviour
{
    #region structs
    [System.Serializable]
    private struct UI_Buttons
    {
        [SerializeField] private Button _scenarioCancelButton;
        [SerializeField] private Button _scenarioSaveButton;

        public Button ScenarioCancelButton { get => _scenarioCancelButton; set => _scenarioCancelButton = value; }
        public Button ScenarioSaveButton { get => _scenarioSaveButton; set => _scenarioSaveButton = value; }
    }

    [System.Serializable]
    private struct UI_ScenarioOverview
    {
        [SerializeField] private TMP_Text _scenarioDisplayTitle;
        [SerializeField] private TMP_Text _scenarioName;
        [SerializeField] private TMP_Text _scenarioDescription;
        [SerializeField] private Button _scenarioDisplayTitleEditButton;

        public TMP_Text ScenarioDisplayTitle { get => _scenarioDisplayTitle; set => _scenarioDisplayTitle = value; }
        public TMP_Text ScenarioName { get => _scenarioName; set => _scenarioName = value; }
        public TMP_Text ScenarioDescription { get => _scenarioDescription; set => _scenarioDescription = value; }
        public Button ScenarioDisplayTitleEditButton { get => _scenarioDisplayTitleEditButton; set => _scenarioDisplayTitleEditButton = value; }
    }

    [System.Serializable]
    private struct UI_ScenarioOverview_Title
    {
        [SerializeField] private TMP_InputField _scenarioName;
        [SerializeField] private TMP_InputField _scenarioNewDisplayTitle;
        [SerializeField] private Button _scenarioCancelButton;
        [SerializeField] private Button _scenarioSaveButton;

        public TMP_InputField ScenarioName { get => _scenarioName; set => _scenarioName = value; }
        public TMP_InputField ScenarioNewDisplayTitle { get => _scenarioNewDisplayTitle; set => _scenarioNewDisplayTitle = value; }
        public Button ScenarioCancelButton { get => _scenarioCancelButton; set => _scenarioCancelButton = value; }
        public Button ScenarioSaveButton { get => _scenarioSaveButton; set => _scenarioSaveButton = value; }
    }

    [System.Serializable]
    private struct UI_ScenarioSelection
    {
        [SerializeField] private Button _scenarioImportButton;
        [SerializeField] private GameObject _scenarioListContent;
        [SerializeField] private GameObject _scenarioListItemPrefab;

        public Button ScenarioImportButton { get => _scenarioImportButton; set => _scenarioImportButton = value; }
        public GameObject ScenarioListContent { get => _scenarioListContent; set => _scenarioListContent = value; }
        public GameObject ScenarioListItemPrefab { get => _scenarioListItemPrefab; set => _scenarioListItemPrefab = value; }
    }
    #endregion

    #region variables
    [SerializeField] private UI_Buttons _UI_Buttons;
    [SerializeField] private UI_ScenarioOverview _UI_ScenarioOverview;
    [SerializeField] private UI_ScenarioOverview_Title _UI_ScenarioOverview_Title;
    [SerializeField] private UI_ScenarioSelection _UI_ScenarioSelection;
    [SerializeField] private PreviewImageLoader _previewImageLoader;

    [SerializeField] private AdminMenu _adminMenu;

    private string _selectedScenarioDirName
    {
        get => this._adminMenu.AppConfigReader.GetAppConfig().DefaultScenarioDirName;
        set => this._adminMenu.AppConfigReader.GetAppConfig().DefaultScenarioDirName = value;
    }
    #endregion

    #region unity_functions
    private void Awake()
    {
    }

    private void Start()
    {
        // Fill Scroll Area with loaded scenarios in StreamingAssets
        // Delete all children of the content object
        foreach (Transform child in _UI_ScenarioSelection.ScenarioListContent.transform)
        {
            Destroy(child.gameObject);
        }
        // Add all scenarios to the content object
        Debug.Assert(SceneLoader.Instance.IsScenariosDictLoaded(), "Scenarios dict not loaded yet!");
        foreach (string scenarioDirName in SceneLoader.Instance.GetAllScenarioDirNames())
        {
            this.AddToScenarioList(scenarioDirName);
        }

        // Show selected scenario if available
        this.UpdateScenarioCard();
    }
    #endregion

    #region public_functions
    public void OnImportScenarioPackageClicked()
    {
        // Open file dialog to select scenario package
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Szenario auswählen", "", "mtscn", false);

        // Extract package to StreamingAssets
        foreach (string path in paths)
        {
            string scenarioDirName = SceneLoader.Instance.ImportScenarioPackage(path);
            Debug.Log("Imported scenario package: " + path + " to " + scenarioDirName);
            this._selectedScenarioDirName = scenarioDirName;
        }

        // Update scenario card
        this.UpdateScenarioCard();

        // Add scenario to list
        this.AddToScenarioList(this._selectedScenarioDirName);
    }

    public void OnSaveChangesClicked()
    {
        this._adminMenu.SaveAppConfig();
    }

    public void OnEditDisplayTitleClicked()
    {
        _UI_ScenarioOverview_Title.ScenarioName.text = _UI_ScenarioOverview.ScenarioName.text;
        _UI_ScenarioOverview_Title.ScenarioNewDisplayTitle.text = _UI_ScenarioOverview.ScenarioDisplayTitle.text;
    }

    public void OnSaveDisplayTitleClicked()
    {
        VariableScenarioConfig scenarioConfig = SceneLoader.Instance.FindScenarioByDirName(this._selectedScenarioDirName);
        
        scenarioConfig.DisplayTitle = _UI_ScenarioOverview_Title.ScenarioNewDisplayTitle.text;
        SceneLoader.Instance.SaveScenarioConfig(scenarioConfig, this._selectedScenarioDirName);
        _UI_ScenarioOverview.ScenarioDisplayTitle.text = scenarioConfig.DisplayTitle;
        TMP_Text scenarioListItemText = _UI_ScenarioSelection.ScenarioListContent.transform.Find(this._selectedScenarioDirName).GetComponentInChildren<TMP_Text>();
        scenarioListItemText.text = scenarioConfig.DisplayTitle;
    }

    public void OnScenarioListItemClicked(string scenarioDirName)
    {
        this._selectedScenarioDirName = scenarioDirName;
        this.UpdateScenarioCard();
    }

    public void UpdateScenarioCard()
    {
        VariableScenarioConfig scenarioConfig = SceneLoader.Instance.FindScenarioByDirName(this._selectedScenarioDirName);
        if (scenarioConfig == null)
        {
            _UI_ScenarioOverview.ScenarioDisplayTitle.text = "Kein Szenario ausgewählt";
            _UI_ScenarioOverview.ScenarioName.text = "";
            _UI_ScenarioOverview.ScenarioDescription.text = "";
            // Disable edit button
            _UI_ScenarioOverview.ScenarioDisplayTitleEditButton.interactable = false;
            _previewImageLoader.SetPreviewImage("unknown");
        }
        else
        {
            _UI_ScenarioOverview.ScenarioDisplayTitle.text = scenarioConfig.DisplayTitle;
            _UI_ScenarioOverview.ScenarioName.text = scenarioConfig.ScenarioName;
            _UI_ScenarioOverview.ScenarioDescription.text = scenarioConfig.Description;
            _previewImageLoader.SetPreviewImage(scenarioConfig.GetDefaultAgent().AgentID);
            // Enable edit button
            _UI_ScenarioOverview.ScenarioDisplayTitleEditButton.interactable = true;
        }
    }
    #endregion

    #region helper_functions
    private void AddToScenarioList(string scenarioDirName)
    { 
        Debug.Log("Adding scenario to list: " + scenarioDirName);
        VariableScenarioConfig scenarioConfig = SceneLoader.Instance.FindScenarioByDirName(scenarioDirName);
        GameObject scenarioListItem = Instantiate(_UI_ScenarioSelection.ScenarioListItemPrefab, _UI_ScenarioSelection.ScenarioListContent.transform);
        scenarioListItem.name = scenarioDirName;
        scenarioListItem.GetComponentInChildren<TMP_Text>().text = scenarioConfig.DisplayTitle;
        scenarioListItem.GetComponent<Button>().onClick.AddListener(() => OnScenarioListItemClicked(scenarioDirName));
        // Add delete button
        Button deleteButton = scenarioListItem.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => RemoveFromScenarioList(scenarioDirName));
    }

    private void RemoveFromScenarioList(string scenarioDirName)
    {
        Debug.Log("Delete scenario: " + scenarioDirName);
        SceneLoader.Instance.DeleteScenario(scenarioDirName);
        // Update selected scenario
        if (this._selectedScenarioDirName == scenarioDirName)
        {
            this._selectedScenarioDirName = "";
            this.UpdateScenarioCard();
        }
    }
    #endregion
}
