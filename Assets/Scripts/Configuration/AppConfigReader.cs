using UnityEngine;

using MedicalTraining.Utils;
using MedicalTraining.Configuration.Mapping;
using System.IO;
using Newtonsoft.Json;


namespace MedicalTraining.Configuration
{
    public class AppConfigReader : MonoBehaviour
    {
        private AppConfig m_appConfig;
        private readonly string m_appConfigName = PathTools.EscapePath("AppConfig.json");


        private void Start()
        {
            LoadAppConfig();
        }

        private void LoadAppConfig()
        {
            string appConfigPath = PathTools.EscapePath(PathTools.CombinePaths(Application.streamingAssetsPath, m_appConfigName));

            if (File.Exists(appConfigPath))
            {
                this.ImportAppConfig(appConfigPath);
            }
            else
            {
                Debug.LogWarning($"AppConfig file ({appConfigPath}) not found. Using default configuration.");
                this.m_appConfig = new AppConfig();
            }
        }

        public AppConfig GetAppConfig()
        {
            if (this.m_appConfig == null)
            {
                this.LoadAppConfig();
            }
            return this.m_appConfig;
        }

        public void SetAppConfig(AppConfig appConfig)
        {
            this.m_appConfig = appConfig;
        }

        public void SaveAppConfig()
        {
            string appConfigPath = PathTools.CombinePaths(Application.streamingAssetsPath, m_appConfigName);
            this.ExportAppConfig(appConfigPath);
        }

        public void ImportAppConfig(string path)
        {
            string appConfigPath = PathTools.EscapePath(path);
            this.m_appConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(appConfigPath));
        }

        public void ExportAppConfig(string path)
        {
            string appConfigPath = PathTools.EscapePath(path);
            File.WriteAllText(appConfigPath, JsonConvert.SerializeObject(this.GetAppConfig()));
        }

        public string GetDeviceName()
        {
            AppConfig appConfig = this.GetAppConfig();
            return appConfig.DeviceName != null ? appConfig.DeviceName : SystemInfo.deviceName;
        }
    }
}
