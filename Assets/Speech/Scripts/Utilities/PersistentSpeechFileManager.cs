using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Speech.Scripts.Utilities
{
    public class PersistentSpeechFileManager
    {
        #region variables
        private static string DataStoragePath;
        #endregion

        #region static_functions
        public static void SetupPersistentDataStoragePath()
        {
            DataStoragePath = Path.Combine(Application.streamingAssetsPath, "Speech");
        }

        public static string GetFilePath(string fileName, string subfolderName = "")
        {
            DirectoryInfo dir = GetDirectoryInfo(subfolderName);

            //Debug.Log($"Trying to create cache path: {dir.FullName}");

            if (!dir.Exists)
            {
                dir.Create();
            }

            string path = Path.Combine(dir.FullName, fileName);
            //Debug.Log($"File path is: {path}");

            return path;
        }

        public static List<string> GetFileNamesInFolder(string subfolderName = "", bool withoutSuffix = false)
        {
            List<string> result = new();

            DirectoryInfo dir = GetDirectoryInfo(subfolderName);

            if (!dir.Exists)
            {
                return result;
            }

            // TODO: Increase performance by omitting DirectoryInfo and directly using Directory
            result.AddRange(dir.GetFiles().Where(fileInfo => fileInfo.Extension != ".meta")
                .Select(fileInfo =>
                {
                    if (withoutSuffix)
                    {
                        return Path.GetFileNameWithoutExtension(fileInfo.FullName);
                    }
                    else
                    {
                        return Path.GetFileName(fileInfo.FullName);
                    }
                })
            );

            return result;
        }

        public static void DeleteSingleFile(string fileName, string subfolderName = "")
        {
            string filePath = GetFilePath(fileName, subfolderName);

            if (!File.Exists(filePath))
            {
                return;
            }

            File.Delete(filePath);
        }

        public static void DeleteAllFiles()
        {
            DirectoryInfo dir = GetDirectoryInfo();

            if (!dir.Exists)
            {
                return;
            }

            dir.Delete(true);
        }

        private static DirectoryInfo GetDirectoryInfo(string subfolderName = "")
        {
            return new DirectoryInfo(Path.Combine(DataStoragePath, subfolderName));
        }
        #endregion
    }
}