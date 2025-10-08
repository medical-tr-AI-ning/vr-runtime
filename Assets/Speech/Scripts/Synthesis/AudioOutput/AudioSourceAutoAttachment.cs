using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Speech.Scripts.Synthesis
{
    public class AudioSourceAutoAttachment : MonoBehaviour
    {
        #region variables
        [SerializeField] private string _agentHeadGameObjectName = "CC_Base_Head";
        [SerializeField] private bool _attachToAgentHead = true;
        [SerializeField] private Transform _transformToAttachTo;
        
        private Transform _audioSourceTransform;
        #endregion

        #region unity_functions
        private void Awake()
        {
            // Audio Source Transform
            _audioSourceTransform = GetComponentInChildren<AudioSource>().transform;

            // Destination Transform
            if (_attachToAgentHead)
            {
                // TODO: Make this run faster
                _transformToAttachTo = transform.parent.gameObject.FindObject(_agentHeadGameObjectName)?.transform;
            }
        }

        private void Start()
        {
            // Audio Source Transform
            if (!_audioSourceTransform)
            {
                Debug.LogError("No child transform containing Audio Source component found.", this);
            }

            // Destination Transform
            if (!_transformToAttachTo)
            {
                Debug.LogWarning("No transform reference to attach AudioSource to.", this);
                return;
            }

            // Attach to destination transform
            _audioSourceTransform.transform.SetParent(_transformToAttachTo, false);

        }
        #endregion
    }
}
