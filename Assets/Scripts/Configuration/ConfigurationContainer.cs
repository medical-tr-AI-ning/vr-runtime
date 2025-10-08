using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.Configuration.Mapping;
using MedicalTraining.Logger;
using MedicalTraining.Utils;
using static medicaltraining.assetstore.ScenarioConfiguration.Serialization.VariableScenarioConfig;
using UnityEngine.Rendering;
using MedicalTraining.Skin;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;


namespace MedicalTraining.Configuration
{
    public class ConfigurationContainer : MonoBehaviour
    {
        private static ConfigurationContainer m_instance;
        public static ConfigurationContainer Instance
        {
            get
            {
                if (ConfigurationContainer.m_instance == null)
                {
                    GameObject configurationContainer = new GameObject("ConfigurationContainer");
                    DontDestroyOnLoad(configurationContainer);
                    ConfigurationContainer.m_instance = configurationContainer.AddComponent<ConfigurationContainer>();
                }
                return ConfigurationContainer.m_instance;
            }
        }

        private SimulationLogger m_logger;

        [Header("Fallback References")]
        [SerializeField] private TextAsset m_scenariosListFallback;
        [SerializeField] private TextAsset m_scenarioConfigFallback;
        [SerializeField] private TextAsset m_assetsConfig;

        private string m_studentName;
        private string m_studentId;

        public AppConfigReader AppConfigReader { get; private set; }

        public Dictionary<string, List<string>> FileDestinations { get; private set; }

        public VariableScenarioConfig VariableScenario { get; private set; }
        public ScenarioVariant Scenario { get; private set; }
        public PathologyVariant Pathology { get; private set; }
        private SerializedDictionary<string, string> ScenarioSpecificSettings;
        
        public string ScenarioConfigDir { get; private set; }
        public string AgentPath { get => PathTools.CombinePaths(this.ScenarioConfigDir, this.Scenario.Agent.AgentID); }
        public string PathologyPath { get => PathTools.CombinePaths(this.ScenarioConfigDir, this.Scenario.Pathology.Pathology.PathologyID); }

        public GameObject Agent { get; private set; }

        private void Awake()
        {
            // make sure there is always exactly one instance of the ConfigurationContainer
            if (ConfigurationContainer.m_instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                ConfigurationContainer.m_instance = this;

                this.AppConfigReader = gameObject.AddComponent<AppConfigReader>();
                LoadFileDestinations();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public SimulationLogger GetLogger()
        {
            if (m_logger == null)
            {
                GameObject sceneConfig = GameObject.Find("/SceneConfig");
                if (sceneConfig != null)
                {
                    m_logger = sceneConfig.GetComponent<SimulationLogger>();
                }
                else
                {
                    m_logger = FindObjectOfType<SimulationLogger>();
                }
            }
            return m_logger;
        }

        private void LoadFileDestinations()
        {
            this.FileDestinations = new Dictionary<string, List<string>>();
            foreach (var fileDestinations in AppConfigReader.GetAppConfig().FileDestinations)
            {
                FileDestinations.Add(fileDestinations.Key, new List<string>());
                foreach (var destination in fileDestinations.Value)
                {
                    string path = AppConfigReader.GetAppConfig().SaveLocations.GetValueOrDefault(destination, null);
                    path ??= AppConfig.DefaultPath;  // Replace unset path (null) with default local path
                    path = PathTools.EscapePath(PathTools.CombinePaths(path, AppConfigReader.GetDeviceName()));
                    this.FileDestinations[fileDestinations.Key].Add(path);
                }
            }
        }

        public TextAsset GetScenarioListFallback()
        {
            return this.m_scenariosListFallback;
        }

        public TextAsset GetScenarioConfigFallback()
        {
            return this.m_scenarioConfigFallback;
        }

        public TextAsset GetAssetsConfigFallback()
        {
            return this.m_assetsConfig;
        }

        public void GetStudentData(out string studentName, out string studentId)
        {
            studentName = this.m_studentName;
            studentId = this.m_studentId;
        }

        public void SetStudentData(string studentName, string studentId)
        {
            this.m_studentName = studentName;
            this.m_studentId = studentId;
        }

        public void ClearStudentData()
        {
            this.m_studentName = "";
            this.m_studentId = "";
        }

        public void SetAgent(GameObject agent)
        {
            this.Agent = agent;
        }

        public void SetVariableScenario(VariableScenarioConfig variableScenario)
        {
            this.VariableScenario = variableScenario;
        }

        public void SetScenario(ScenarioVariant scenario)
        {
            this.Scenario = scenario;
        }

        public void SetPathology(PathologyVariant pathology)
        {
            this.Pathology = pathology;
        }

        public string GetScenarioSpecificSetting(string key, string defaultValue = null)
        {
            string value;

            if (ScenarioSpecificSettings == null) return defaultValue;

            this.ScenarioSpecificSettings.TryGetValue(key, out value);

            if (value == null)
            {
                value = defaultValue;
            }
            return value;
        }

        public void SetScenarioSpecificSettings(SerializedDictionary<string, string> scenarioSpecificSettings)
        {
            this.ScenarioSpecificSettings = scenarioSpecificSettings;
        }

        public void SetScenarioConfigDir(string scenarioPath)
        {
            this.ScenarioConfigDir = scenarioPath;
        }

        public void ClearScenarioData()
        {
            this.VariableScenario = null;
            this.ScenarioConfigDir = null;
            this.Scenario = null;
            this.ScenarioSpecificSettings = null;
            this.Agent = null;
        }
    }
}
