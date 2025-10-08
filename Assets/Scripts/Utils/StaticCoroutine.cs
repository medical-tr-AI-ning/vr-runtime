using System.Collections;
using UnityEngine;

/// <summary>
/// https://forum.unity.com/threads/passing-in-a-monobehaviour-to-run-a-coroutine.588049/#post-3958183
/// This is also useful when dealing with UniTasks (converted from a Coroutine) that need a MonoBehavior to run on.
/// </summary>
public class StaticCoroutine
{
    private class StaticCoroutineRunner : MonoBehaviour { }

    private static StaticCoroutineRunner runner;

    public static Coroutine Start(IEnumerator coroutine, bool dontDestroy = false)
    {
        EnsureRunner(dontDestroy);
        return runner.StartCoroutine(coroutine);
    }

    private static void EnsureRunner(bool dontDestory)
    {
        if (runner == null)
        {
            runner = new GameObject("[Static Coroutine Runner]").AddComponent<StaticCoroutineRunner>();
            if (dontDestory)
            {
                Object.DontDestroyOnLoad(runner.gameObject);
            }
        }
    }

}