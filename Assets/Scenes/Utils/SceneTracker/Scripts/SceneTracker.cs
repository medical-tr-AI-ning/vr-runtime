using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Tracks the previous loaded Scenes this script is placed in
/// TODO: Make it more robust by tracking SceneReferences instead of Scene names:
/// https://github.com/starikcetin/Eflatun.SceneReference
/// </summary>
public class SceneTracker : MonoBehaviour
{
    public static string PreviousSceneName { get; private set; }

    private static readonly List<string> _listOfPreviousSceneNames;

    public static ReadOnlyCollection<string> ListOfPreviousScenes { get; private set; }

    static SceneTracker()
    {
        _listOfPreviousSceneNames = new List<string>();
        ListOfPreviousScenes = _listOfPreviousSceneNames.AsReadOnly();
    }

    private void OnDestroy()
    {
        PreviousSceneName = SceneManager.GetActiveScene().name;
        _listOfPreviousSceneNames.Add(PreviousSceneName);
    }
}
