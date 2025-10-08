using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using MedicalTraining.Utils;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using System;


namespace MedicalTraining.Configuration
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader m_instance;
        public static SceneLoader Instance { get {
                if (SceneLoader.m_instance == null)
                {
                    GameObject sceneLoader = new GameObject("SceneLoader");
                    DontDestroyOnLoad(sceneLoader);
                    SceneLoader.m_instance = sceneLoader.AddComponent<SceneLoader>();
                }
                return SceneLoader.m_instance;
            } }

        private const string m_menuSceneName = "Menu";
        private const string m_loadingSceneName = "Loading";

        [Header("Unity Scene Dirs")]
        [SerializeField] private string m_generalScenesPath = PathTools.CombinePaths("Scenes", "General");
        [SerializeField] private string m_scenarioScenesPath = PathTools.CombinePaths("Scenes", "Scenarios");

        [Header("Unity Scene Names")]
        [SerializeField] private string m_tutorialSceneName = "Scenario_Tutorial";

        [Header("Streaming Assets Paths")]
        [SerializeField] private string m_scenarioConfigName = "scenario.json";
        [SerializeField] private string m_scenariosDirPath = "Scenarios";

        private ConfigurationContainer m_config;
        private Dictionary<string, VariableScenarioConfig> m_availableScenariosDict;

        public string ScenariosConfigDir { get {
                return PathTools.EscapePath(PathTools.CombinePaths(Application.streamingAssetsPath, m_scenariosDirPath));
            } }


        private void Awake()
        {
            // Make sure there is always exactly one instance of the SceneLoader
            if (SceneLoader.m_instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                SceneLoader.m_instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            // Load the configuration container
            this.m_config = ConfigurationContainer.Instance;

            // Load the current scenario if the SceneLoader is created in a scenario scene
            this.DirectlyLoadCurrentScenario();
        }

        // Start is called before the first frame update
        private void Start()
        {
            LoadScenariosList();
        }

        /// <summary>
        /// Load the current scenario if the SceneLoader is created in a scenario scene
        /// </summary>
        private void DirectlyLoadCurrentScenario()
        {
            Scene currentScene = SceneManager.GetActiveScene();

            // If the current scene is the main menu, do nothing
            if (currentScene.name == m_menuSceneName)
            {
                return;
            } 
            // If the current scene is the loading scene, the expected behaviour is unknown
            else if (currentScene.name == m_loadingSceneName)
            {
                Debug.LogError("Loading scene cannot be loaded directly.");
                return;
            }

            // If the current scene is a scenario, try to load it
            // TODO: Set default student name and ID!
            Debug.Log("Directly loading scenario: " + currentScene.name);
            this.LoadScenariosList();
            // TODO: Load the scenario by the directory name instead of the scene name
            this.LoadScenarioByDirName(currentScene.name);
        }

        /// <summary>
        /// Import a scenario from a .mtscn file by extracting the scenario config file and assets to the streaming assets folder
        /// </summary>
        /// <param name="scenarioPackagePath"></param>
        public string ImportScenarioPackage(string scenarioPackagePath)
        {
            scenarioPackagePath = PathTools.EscapePath(scenarioPackagePath);

            // Get the scenario directory path and unused destination dir by counting up
            string scenarioDir = PathTools.CombinePaths(this.ScenariosConfigDir, Path.GetFileNameWithoutExtension(scenarioPackagePath));
            int i = 1;
            while (Directory.Exists(scenarioDir))
            {
                scenarioDir = PathTools.CombinePaths(this.ScenariosConfigDir, Path.GetFileNameWithoutExtension(scenarioPackagePath) + "_" + i);
                i++;
            }

            // Unpack the scenario package
            System.IO.Compression.ZipFile.ExtractToDirectory(scenarioPackagePath, scenarioDir);

            // Add the scenario to the list of available scenarios
            this.ImportScenario(scenarioDir);

            return Path.GetFileNameWithoutExtension(scenarioDir);
        }

        public void SaveScenarioConfig(VariableScenarioConfig scenarioConfig, string scenarioDirName)
        {
            string scenarioConfigPath = PathTools.EscapePath(PathTools.CombinePaths(this.ScenariosConfigDir, scenarioDirName, m_scenarioConfigName));
            File.WriteAllText(scenarioConfigPath, scenarioConfig.Serialize());
        }

        public void DeleteScenario(string scenarioDirName)
        {
            string scenarioDir = PathTools.CombinePaths(this.ScenariosConfigDir, scenarioDirName);
            if (Directory.Exists(scenarioDir))
            {
                Directory.Delete(scenarioDir, true);
            }
            else
            {
                Debug.LogWarning("Scenario directory not found: " + scenarioDir);
            }
            this.m_availableScenariosDict.Remove(scenarioDirName);
        }

        public void LoadScenariosList()
        {
            if (m_availableScenariosDict == null)
            {
                m_availableScenariosDict = new Dictionary<string, VariableScenarioConfig>();

                // Iterate through all subdirectories in the scenarios directory and load the scenario config files
                foreach (string scenarioDir in Directory.GetDirectories(this.ScenariosConfigDir))
                {
                    this.ImportScenario(scenarioDir);
                }
            }
        }

        public IEnumerator LoadSceneAsync(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }

        private IEnumerator LoadSceneAsyncAfterAssetLoader(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);

            // Configure the scenario after the scene is loaded (has to be done after AgentAssets are loaded)
            ScenarioConfigurator scenarioConfigurator = FindObjectOfType<ScenarioConfigurator>();
            if (scenarioConfigurator != null)
            {
                scenarioConfigurator.ConfigureScenario();
            }
        }

        public void LoadMainMenuScene()
        {
            // Escape path for Unix systems so Unity finds the scene
            string menuScenePath = PathTools.EscapePath(PathTools.CombinePaths(m_generalScenesPath, m_menuSceneName), posix: true);
            StartCoroutine(this.LoadSceneAsync(menuScenePath));
        }

        public void LoadLoadingScene()
        {
            // Escape path for Unix systems so Unity finds the scene
            string loadingScenePath = PathTools.EscapePath(PathTools.CombinePaths(m_generalScenesPath, m_loadingSceneName), posix: true);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadingScenePath);
            asyncLoad.allowSceneActivation = true;
        }

        public void LoadTutorialScene()
        {
            this.LoadLoadingScene();

            // Escape path for Unix systems so Unity finds the scene
            string scenePath = PathTools.EscapePath(PathTools.CombinePaths(m_scenarioScenesPath, this.m_tutorialSceneName), posix: true);
            StartCoroutine(this.LoadSceneAsync(scenePath));
        }

        public VariableScenarioConfig FindScenarioByDirName(string scenarioDirName)
        {
            return m_availableScenariosDict.GetValueOrDefault(scenarioDirName, null);
        }

        public VariableScenarioConfig ImportScenario(string scenarioPath)
        {
            string scenarioConfigPath = PathTools.EscapePath(PathTools.CombinePaths(scenarioPath, m_scenarioConfigName));

            if (File.Exists(scenarioConfigPath))
            {
                try {
                    VariableScenarioConfig variableScenarioConfig = VariableScenarioConfig.TryDeserialize(File.ReadAllText(scenarioConfigPath));

                    string scenarioDirName = Path.GetFileName(Path.GetDirectoryName(scenarioConfigPath));
                    if (string.IsNullOrEmpty(variableScenarioConfig.ScenarioName))
                    {
                        variableScenarioConfig.ScenarioName = scenarioDirName;
                    }

                    m_availableScenariosDict[scenarioDirName] = variableScenarioConfig;
                    return variableScenarioConfig;
                } catch (Exception e) {
                    Debug.LogError($"Failed to load scenario config file ({scenarioConfigPath}): {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Scenario config file ({scenarioConfigPath}) not found. Skipping scenario.");
            }
            return null;
        }

        public bool IsScenarioSelected()
        {
            return !string.IsNullOrEmpty(this.GetSelectedScenarioDirName());
        }

        public string GetSelectedScenarioDirName()
        {
            return this.m_config.AppConfigReader.GetAppConfig().DefaultScenarioDirName;
        }

        public VariableScenarioConfig GetSelectedScenario()
        {
            return this.FindScenarioByDirName(this.GetSelectedScenarioDirName());
        }

        public void SelectScenarioByDirName(string scenarioDirName)
        {
            Debug.LogWarning("Selecting scenario by dir name (but not saved to disk!): " + scenarioDirName);
            this.m_config.AppConfigReader.GetAppConfig().DefaultScenarioDirName = scenarioDirName;
        }

        public bool IsScenariosDictLoaded()
        {
            return m_availableScenariosDict != null;
        }

        public List<string> GetAllScenarioDirNames()
        {
            return new List<string>(m_availableScenariosDict.Keys);
        }

        public List<VariableScenarioConfig> GetAllScenarioItems()
        {
            return new List<VariableScenarioConfig>(m_availableScenariosDict.Values);
        }

        public List<VariableScenarioConfig> GetVisibleScenarioItems()
        {
            return this.GetAllScenarioItems();  //.FindAll(scenario => scenario.visible);
        }

        public List<string> GetVisibleScenarioNames()
        {
            List<string> scenarioNames = new List<string>();
            foreach (VariableScenarioConfig scenario in GetVisibleScenarioItems())
            {
                scenarioNames.Add(scenario.DisplayTitle);
            }
            return scenarioNames;
        }

        public void LoadSelectedScenario()
        {
            string selectedScenarioDirName = this.GetSelectedScenarioDirName();
            if (!string.IsNullOrEmpty(selectedScenarioDirName))
            {
                this.LoadScenarioByDirName(selectedScenarioDirName);
            }
            else
            {
                Debug.LogError("No scenario selected.");
            }
        }

        public void LoadScenarioByDirName(string scenarioDirName)
        {
            this.LoadLoadingScene();

            string scenarioConfigDir = PathTools.EscapePath(PathTools.CombinePaths(ScenariosConfigDir, scenarioDirName));
            VariableScenarioConfig scenario = this.FindScenarioByDirName(scenarioDirName);

            ScenarioConfigReader scenarioConfigReader = gameObject.AddComponent<ScenarioConfigReader>();
            scenarioConfigReader.PrepareScenarioConfig(scenario, scenarioConfigDir);

            string sceneName = this.m_config.Scenario.Environment.EnvironmentID;  // TODO: Exception handling if scene not found?
            StartCoroutine(this.LoadSceneAsyncAfterAssetLoader(sceneName));
        }
    }
}
