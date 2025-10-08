using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTutorial : MonoBehaviour
{
    public void LoadMenu()
    {
        StartCoroutine(LoadMenuAsync());
    }

    private IEnumerator LoadMenuAsync()
    {
        yield return new WaitForSeconds(10);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scenes/General/Menu");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
