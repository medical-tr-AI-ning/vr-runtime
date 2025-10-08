using System.Text.RegularExpressions;
using UnityEngine;


namespace MedicalTraining.Utils
{
    public class PathTools
    {
        static public string CombinePaths(params string[] paths)
        {
            string result = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                string path = paths[i];

                // Remove long path prefix if it exists
                path = path.Replace(@"\\?\", "");
                
                // Check operating system
                if (PathTools.IsWindows())
                {
                    // Combine paths with backslashes
                    result = result + @"\" + path;
                }
                else
                {
                    // Combine paths with forward slashes
                    result = result + @"/" + path;
                }
            }
            return result;
        }

        static public string EscapePath(string path, bool posix = false)
        {
            // Escape invalid characters
            string newPath = Regex.Replace(path, new string(System.IO.Path.GetInvalidPathChars()), "");

            // Remove long path prefix if it exists
            newPath = Regex.Replace(newPath, @"^\\\\\?\\", "");

            // Remove double slashes
            newPath = Regex.Replace(newPath, @"[\\/]+", "/");

            // Check operating system
            if (PathTools.IsWindows() && !posix)
            {
                // Replace forward slashes with backslashes 
                newPath = newPath.Replace(@"/", @"\");

                // Add prefix for long paths
                newPath = @"\\?\" + newPath;
            }
            else
            {
                // Replace backslashes with forward slashes
                newPath = newPath.Replace(@"\", @"/");
            }

            // Return the escaped path
            return newPath;
        }

        static public bool IsWindows()
        {
            return Application.platform == RuntimePlatform.WindowsPlayer   // Windows standalone
                || Application.platform == RuntimePlatform.WindowsEditor   // Windows editor
                || Application.platform == RuntimePlatform.WSAPlayerX86    // Windows Store x86
                || Application.platform == RuntimePlatform.WSAPlayerX64    // Windows Store x64
                || Application.platform == RuntimePlatform.WSAPlayerARM;   // Windows Store ARM
        }
    }
}
