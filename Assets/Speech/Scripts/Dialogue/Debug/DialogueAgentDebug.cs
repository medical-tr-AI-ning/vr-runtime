using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAgentDebug : MonoBehaviour
{
    [SerializeField] GameObject _agent;
    [SerializeField] bool _setAgent = false;

    private void Start()
    {
        if (_agent)
        {
            DialogueController.Instance.SetNewAgent(_agent);
        }
    }
    private void OnValidate()
    {
        if(_setAgent && Application.isPlaying)
        {
            DialogueController.Instance?.SetNewAgent(_agent);
        }
    }
}
