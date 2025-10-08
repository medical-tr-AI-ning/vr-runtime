using UnityEngine;
using UnityEngine.UI;

using TMPro;
using MedicalTraining.Configuration;
using SFB;
using System.Collections.Generic;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.UI;


[RequireComponent(typeof(AppConfigReader))]
public class AdminMenu : MonoBehaviour
{
    #region structs
    [System.Serializable]
    private struct UI_Config_Settings
    {
        [SerializeField] private TMP_Text _scenarioNameText;
        [SerializeField] private TMP_InputField _deviceNameInputField;
        [SerializeField] private TMP_InputField _savePathInputField;
        [SerializeField] private Button _savePathButton;
        [SerializeField] private ToggleButton _saveVideoToggle;
        [SerializeField] private ToggleButton _saveImagesToggle;
        [SerializeField] private ToggleButton _saveImagesLimitToggle;
        [SerializeField] private TMP_InputField _saveImagesLimitInputField;

        public TMP_Text ScenarioNameText { get => _scenarioNameText; set => _scenarioNameText = value; }
        public TMP_InputField DeviceNameInputField { get => _deviceNameInputField; set => _deviceNameInputField = value; }
        public TMP_InputField SavePathInputField { get => _savePathInputField; set => _savePathInputField = value; }
        public Button SavePathButton { get => _savePathButton; set => _savePathButton = value; }
        public ToggleButton SaveVideoToggle { get => _saveVideoToggle; set => _saveVideoToggle = value; }
        public ToggleButton SaveImagesToggle { get => _saveImagesToggle; set => _saveImagesToggle = value; }
        public ToggleButton SaveImagesLimitToggle { get => _saveImagesLimitToggle; set => _saveImagesLimitToggle = value; }
        public TMP_InputField SaveImagesLimitInputField { get => _saveImagesLimitInputField; set => _saveImagesLimitInputField = value; }
    }
    #endregion

    #region variables
    [SerializeField] private UI_Config_Settings _UI_Config_Settings;

    private AppConfigReader _appConfigReader;
    [SerializeField] public AppConfigReader AppConfigReader 
    { 
        get => this._appConfigReader ?? gameObject.GetComponent<AppConfigReader>();
        private set => this._appConfigReader = value;
    }
    #endregion

    #region unity_functions
    private void Awake()
    {
        this.ResetAppConfig();
    }

    private void Start()
    {
    }
    #endregion

    #region public_functions
    public void OnImportConfigClicked()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("App Konfiguration auswählen", "", "json", false);
        // Import the configuration file
        if (paths.Length > 0)
        {
            this.AppConfigReader.ImportAppConfig(paths[0]);
        }
        this.UpdateUISettings();
    }

    public void OnExportConfigClicked()
    {
        this.UpdateFromUISettings();
        string path = StandaloneFileBrowser.SaveFilePanel("App Konfiguration speichern", "", "AppConfig", "json");
        // Export the configuration file
        if (!string.IsNullOrEmpty(path))
        {
            this.AppConfigReader.ExportAppConfig(path);
        }
    }

    public void OnSavePathButtonClicked()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Speicherort auswählen", "", false);
        if (paths.Length > 0)
        {
            this._UI_Config_Settings.SavePathInputField.text = paths[0];
        }
    }

    public void SaveAppConfig()
    {
        this.UpdateFromUISettings();
        this.SaveLimitSavingImages();
        // Important: Update the AppConfigReader instance in the ConfigurationContainer!
        ConfigurationContainer.Instance.AppConfigReader.SetAppConfig(this.AppConfigReader.GetAppConfig());
        ConfigurationContainer.Instance.AppConfigReader.SaveAppConfig();
    }

    public void ResetAppConfig()
    {
        // Important: No reference to ConfigurationContainer.Instance.AppConfigReader here!
        this.AppConfigReader.SetAppConfig(null);  // Load current AppConfig from file
        this.UpdateUISettings();
    }

    public void UpdateUISettings()
    {
        // Show selected scenario if available
        string scenarioDirName = this.AppConfigReader.GetAppConfig().DefaultScenarioDirName;
        if (!string.IsNullOrEmpty(scenarioDirName))
        {
            VariableScenarioConfig scenarioConfig = SceneLoader.Instance.FindScenarioByDirName(scenarioDirName);
            if (scenarioConfig != null)
            {
                // Show scenario name
                _UI_Config_Settings.ScenarioNameText.text = scenarioConfig.ScenarioName;

                // Show limit for saving images
                string maxPicturesDermatoscope = scenarioConfig.ScenarioSpecificSettings.GetValueOrDefault("maxPicturesDermatoscope", "5");
                _UI_Config_Settings.SaveImagesLimitInputField.text = maxPicturesDermatoscope ?? "5";
            }
        }

        // Update the device name
        string deviceName = this.AppConfigReader.GetAppConfig().DeviceName;
        _UI_Config_Settings.DeviceNameInputField.text = deviceName ?? "";

        // Update the save path
        // TODO: More extensive search for a common save path
        string savePath = this.AppConfigReader.GetAppConfig().SaveLocations.GetValueOrDefault("default", "");
        _UI_Config_Settings.SavePathInputField.text = savePath;

        // Update the save video toggle
        bool saveVideo = this.AppConfigReader.GetAppConfig().FileDestinations["ScreenCapture"].Contains("default");
        _UI_Config_Settings.SaveVideoToggle.SetToggleState(saveVideo);

        // Update the save images toggle
        bool saveImages = this.AppConfigReader.GetAppConfig().FileDestinations["Photo"].Contains("default");
        _UI_Config_Settings.SaveImagesToggle.SetToggleState(saveImages);
    }
    #endregion

    #region helper_functions
    private void UpdateFromUISettings()
    {
        // Update the device name
        string deviceName = _UI_Config_Settings.DeviceNameInputField.text;
        if (deviceName.Length == 0)
        {
            deviceName = null;
        }
        this.AppConfigReader.GetAppConfig().DeviceName = deviceName;

        // Update the default save path
        string savePath = _UI_Config_Settings.SavePathInputField.text;
        if (savePath.Length == 0)
        {
            savePath = null;
        }
        this.AppConfigReader.GetAppConfig().SaveLocations["default"] = savePath;

        // Add default save location to all elements if not already present
        foreach (var fileDestinations in this.AppConfigReader.GetAppConfig().FileDestinations)
        {
            if (!fileDestinations.Value.Contains("default"))
            {
                fileDestinations.Value.Add("default");
            }
        }

        // Remove default save location for disabled elements
        if (_UI_Config_Settings.SaveVideoToggle.buttonState == false)
        {
            this.AppConfigReader.GetAppConfig().FileDestinations["ScreenCapture"].Remove("default");
        }
        if (_UI_Config_Settings.SaveImagesToggle.buttonState == false)
        {
            this.AppConfigReader.GetAppConfig().FileDestinations["Photo"].Remove("default");
        }

        // Limit for saving images not here because it is not in the AppConfig
    }

    private void SaveLimitSavingImages()
    {
        // Limit for saving images
        string maxPicturesDermatoscope = _UI_Config_Settings.SaveImagesLimitInputField.text;
        if (_UI_Config_Settings.SaveImagesToggle.buttonState == false)
        {
            maxPicturesDermatoscope = "0";
        }
        else if (_UI_Config_Settings.SaveImagesLimitToggle.buttonState == false)
        {
            maxPicturesDermatoscope = null;
        }

        // Set value in scenario specific settings
        string scenarioDirName = this.AppConfigReader.GetAppConfig().DefaultScenarioDirName;
        VariableScenarioConfig scenarioConfig = SceneLoader.Instance.FindScenarioByDirName(scenarioDirName);
        if (scenarioConfig != null)
        {
            scenarioConfig.ScenarioSpecificSettings["maxPicturesDermatoscope"] = maxPicturesDermatoscope;
        }

        SceneLoader.Instance.SaveScenarioConfig(scenarioConfig, scenarioDirName);
    }

    #endregion
}
