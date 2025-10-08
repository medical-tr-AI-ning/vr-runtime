using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using MedicalTraining.Configuration;
using MedicalTraining.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static medicaltraining.assetstore.ScenarioConfiguration.Serialization.VariableScenarioConfig;


namespace MedicalTraining.Logger
{
    public class SimulationLogger : MonoBehaviour
    {
        // public settings
        // Objects for which we want to log the motion
        public List<Transform> TrackedObjects = new List<Transform>();

        // Time in milliseconds to log the objects' motion
        public float DeltaTime = 100.0f;

        // properties
        private string logDirectoryName;

        // settings
        private const string ParticipantDataFileName = "participant_data";
        private const string MotionDataFileName = "motion_data";
        private const string PerformanceDataFileName = "performance_data";
        private const string EventDataFileName = "event_data";

        // our writer
        private List<StreamWriter> motionLogs = new List<StreamWriter>();
        private List<StreamWriter> performanceLogs = new List<StreamWriter>();
        private List<StreamWriter> eventLogs = new List<StreamWriter>();

        // and some control variables
        private float timeSinceLastCapture = 0.0f;
        private float timeSinceLastFPSTick = 0.0f;
        private int currentFps = 0;

        // helper and utils
        public static string LoggerTimeStampUnix()
        {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();
        }
        public static string LoggerTimeStampUTC()
        {
            return ((DateTimeOffset)DateTime.UtcNow).ToString("yyyy-MM-ddTHH-mm-ss");
        }

        // unity interface
        private void Start()
        {
            InitializeLogDirectoryName();
            WriteParticipantData();
            CreateMotionLogs();
            CreatePerformanceLogs();
            CreateEventLogs();
        }

        private void Update()
        {
            float dt = Time.deltaTime * 1000.0f; // ms
            timeSinceLastCapture += dt;
            if (timeSinceLastCapture > DeltaTime)
            {
                WriteMotion();
                timeSinceLastCapture = 0.0f;
            }

            currentFps++;
            timeSinceLastFPSTick += dt;
            if (timeSinceLastFPSTick > 1000.0f) // write fps every second
            {
                WriteFPS();
                timeSinceLastFPSTick = 0.0f;
                currentFps = 0;
            }
        }

        private void OnDestroy()
        {
            // close and dispose our StreamWriter-s
            foreach (StreamWriter motionLog in motionLogs)
            {
                motionLog?.Dispose();
            }
            foreach (StreamWriter performanceLog in performanceLogs)
            {
                performanceLog?.Dispose();
            }
            foreach (StreamWriter eventLog in eventLogs)
            {
                eventLog?.Dispose();
            }
        }

        //  public interface for the event sources
        public void WriteEvent(string callerID, string action, string tag = "")   // TODO: dv: update this to use MedicalTraining.InteractionSystem.Event
                                                                                  //           when (if ever) it is ready
        {
            // Warning: Change the header in CreateEventLogs() if you change the format here!
            WriteLineToLogs($"{LoggerTimeStampUnix()};{callerID};{action};{tag}", eventLogs);
        }

        // public function to generate all file paths for a given file name and type
        public List<string> GetFilePaths(string fileName, string fileType = null, bool readableTimestamp = false)
        {
            List<string> paths = new List<string>();

            ConfigurationContainer configuration = ConfigurationContainer.Instance;
            if (!configuration)
            {
                Debug.LogWarning($"ConfigurationContainer missing! '{fileName}' not saved!");
                return paths;
            }
                
            foreach (string rootDirectory in configuration.FileDestinations.GetValueOrDefault(fileName, new List<string>()))
            {
                try
                {
                    string logDirectory = CreateLogDirectory(rootDirectory);
                    string path = PathTools.EscapePath(PathTools.CombinePaths(logDirectory, fileName + GetFilePostfix(readableTimestamp)));
                    if (fileType != null)
                    {
                        path += "." + fileType;
                    }
                    paths.Add(path);
                }
                catch (Exception ex)
                {
                    if (ex is DirectoryNotFoundException)
                    {
                        Debug.LogError($"DirectoryNotFoundException: Could not find '{rootDirectory}'. Location for '{fileName}' skipped!");
                    }
                }
            }
                
            return paths;
        }

        // private helper function to generate file postfix for all logging related files
        // starts with "_" and adds studentId (if available) followed by Unix timestamp
        private string GetFilePostfix(bool readableTimestamp = false)
        {
            string postfix = "_";

            if (readableTimestamp)
            {
                postfix += LoggerTimeStampUTC();
            }
            else
            {
                postfix += LoggerTimeStampUnix();
            }

            ConfigurationContainer configuration = ConfigurationContainer.Instance;
            if (configuration)
            {
                configuration.GetStudentData(out string _, out string studentId);
                postfix = "_" + studentId + postfix;
            }
            return postfix;
        }

        // helper
        private void WriteMotion()
        {
            foreach (StreamWriter motionLog in motionLogs)
            {
                Debug.Assert(motionLog != null, "MotionLog StreamWriter is null!");

                try
                {
                    // Warning: Change the header in CreateMotionLogs() if you change the format here!
                    motionLog.Write(LoggerTimeStampUnix());
                    foreach (Transform t in TrackedObjects)
                    {
                        motionLog.Write($";{t.name};{t.position.x};{t.position.y};{t.position.z};" +
                            $"{t.rotation.x};{t.rotation.y};{t.rotation.z};{t.rotation.w}");
                    }
                    motionLog.Write("\n");
                    motionLog.Flush();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception: {ex.Message}");
                }
            }
        }

        private void WriteFPS()
        {
            // Warning: Change the header in CreatePerformanceLogs() if you change the format here!
            WriteLineToLogs($"{LoggerTimeStampUnix()};{currentFps}", performanceLogs);
        }

        private void InitializeLogDirectoryName()
        {
            this.logDirectoryName = LoggerTimeStampUTC();
            ConfigurationContainer configuration = ConfigurationContainer.Instance;
            if (configuration)
            {
                configuration.GetStudentData(out string _, out string studentId);
                this.logDirectoryName = studentId + "_" + this.logDirectoryName;
            } else
            {
                Debug.LogWarning("Configuration Container missing! No stundentId found!");
            }
        }

        private string CreateLogDirectory(string rootDirectory)
        {
            string logDirectory = PathTools.EscapePath(PathTools.CombinePaths(rootDirectory, logDirectoryName));

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            return logDirectory;
        }

        private void WriteParticipantData()
        {
            ConfigurationContainer configuration = ConfigurationContainer.Instance;
            Debug.Assert(configuration != null, "ConfigurationContainer not set properly!");

            configuration.GetStudentData(out string studentName, out string studentId);
            VariableScenarioConfig variableScenario = configuration.VariableScenario;
            ScenarioVariant scenario = configuration.Scenario;
            List<SerializableMelanoma> melanomas = configuration.Scenario.Pathology.Pathology.Melanoma;

            string logContent = "";
            logContent += $"Student            [id] [name]: '{studentId}' '{studentName}'\n";
            logContent += $"Scenario        [Name] [Title]: '{variableScenario?.ScenarioName}' '{variableScenario?.DisplayTitle}'\n";
            logContent += $"Scenario         [Description]: '{variableScenario?.Description}'\n";
            logContent += $"Scenario    [ModificationDate]: '{variableScenario?.ModificationDate}'\n";
            logContent += $"Agent         [AgentID] [Name]: '{scenario.Agent.AgentID}' '{scenario.Agent.Name}'\n";
            logContent += $"Environment    [EnvironmentID]: '{scenario.Environment.EnvironmentID}'\n";
            logContent += $"Timestamp           [UnixTime]: '{LoggerTimeStampUnix()}'\n";
            logContent += $"Timestamp                [UTC]: '{LoggerTimeStampUTC()}'\n";

            if (melanomas != null && melanomas.Count > 0)
            {
                foreach (SerializableMelanoma pathologyDescription in melanomas)
                {
                    logContent += $"Pathology [Type] [Description]: 'Melanoma' '{pathologyDescription.Placement}'\n";
                }
            }
            else
            {
                logContent += "No pathology description found.\n";
            }


            foreach (string filePath in GetFilePaths(ParticipantDataFileName, "txt"))
            {
                try
                {
                    using StreamWriter sw = File.CreateText(filePath);
                    sw.WriteLine(logContent);
                }
                catch (Exception)
                {
                    // TODO: check the exception and prompt some info ...
                    Debug.LogWarning($"Wasn't able to write {ParticipantDataFileName} at '{filePath}'.");
                }
            }
        }

        private List<StreamWriter> CreateLogFiles(string fileName, string fileType = "csv")
        {
            List<StreamWriter> logs = new List<StreamWriter>();
            foreach (string filePath in GetFilePaths(fileName, fileType))
            {
                try
                {
                    logs.Add(File.CreateText(filePath));
                }
                catch (Exception)
                {
                    // TODO: check the exception and prompt some info ...
                    Debug.LogWarning($"Wasn't able to create {fileName} at '{filePath}'.");
                }
            }
            return logs;
        }

        private void WriteLineToLogs(string line, List<StreamWriter> logs)
        {
            foreach (StreamWriter log in logs)
            {
                Debug.Assert(log != null, "Log StreamWriter is null!");

                try
                {
                    log.WriteLine(line);
                    log.Flush();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception: {ex.Message}");
                }
            }
        }

        private void CreateMotionLogs()
        {
            motionLogs = CreateLogFiles(MotionDataFileName);

            // Write header
            foreach (StreamWriter motionLog in motionLogs)
            {
                Debug.Assert(motionLog != null, "MotionLog StreamWriter is null!");

                try
                {
                    motionLog.Write("timestamp");
                    foreach (Transform t in TrackedObjects)
                    {
                        motionLog.Write($";{t.name}_name;{t.name}_posX;{t.name}_posY;{t.name}_posZ;" +
                            $"{t.name}_rotX;{t.name}_rotY;{t.name}_rotZ;{t.name}_rotW");
                    }
                    motionLog.Write("\n");
                    motionLog.Flush();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception: {ex.Message}");
                }
            }
        }

        private void CreatePerformanceLogs() 
        {
            performanceLogs = CreateLogFiles(PerformanceDataFileName);

            // Write header
            WriteLineToLogs("timestamp;FPS", performanceLogs);
        }

        private void CreateEventLogs()
        {
            eventLogs = CreateLogFiles(EventDataFileName);

            // Write header
            WriteLineToLogs("timestamp;callerID;action;tag", eventLogs);
        }
    }
}
