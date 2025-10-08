using MedicalTraining.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace MedicalTraining.Configuration.Mapping
{
    public class AppConfig
    {
        public static string DefaultPath {  get
            {
                return PathTools.EscapePath(PathTools.CombinePaths(Directory.GetParent(Application.dataPath).FullName, "CourseLogs"));
            } 
        }

        public Dictionary<string, string> SaveLocations { get; set; } = new Dictionary<string, string>()
        {
            { "default", AppConfig.DefaultPath }
        };

        public Dictionary<string, List<string>> FileDestinations { get; set; } = new Dictionary<string, List<string>>()
        {
            { "participant_data", new List<string>() { "default" } },
            { "motion_data", new List<string>() { "default" } },
            { "event_data", new List<string>() { "default" } },
            { "performance_data", new List<string>() { "default" } },
            { "log_data", new List<string>() { "default" } },
            { "video", new List<string>() { "default" }},
            { "Photo", new List<string>() { "default" } },
            { "ScreenCapture", new List<string>() { "default" } }
        };

        public string DeviceName { get; set; } = SystemInfo.deviceName;

        public string DefaultScenarioDirName { get; set; } = null;
    }
}
