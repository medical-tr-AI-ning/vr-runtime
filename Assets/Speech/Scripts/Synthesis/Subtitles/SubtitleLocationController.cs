using UnityEngine;

namespace Speech.Scripts.Synthesis {
    public class SubtitleLocationController : MonoBehaviour
    {
        #region variables
        [SerializeField] private string _agentHeadGameObjectName = "CC_Base_Head";
        [SerializeField] private float _yOffset = 0.4f;

        private Transform _agentHeadTransform;
        private Transform _cameraTransform;
        private Transform _subtitleContainerTransform;
        #endregion

        #region unity_functions
        private void Start()
        {
            // TODO: Make this run faster
            _agentHeadTransform = transform.parent.gameObject.FindObject(_agentHeadGameObjectName)?.transform;
            if (!_agentHeadTransform)
            {
                Debug.LogWarning("No agent head transform found.", this);
            }

            _cameraTransform = Camera.main?.transform;
            _subtitleContainerTransform = GetComponentInChildren<SubtitleContainer>(true).transform;
        }

        private void Update()
        {
            if (!_agentHeadTransform){
                return;
            }

            // Set location
            Vector3 offset = new Vector3(0, _yOffset, 0);
            _subtitleContainerTransform.position = _agentHeadTransform.position;
            _subtitleContainerTransform.transform.localPosition += offset;

            // Set rotation
            if (!_cameraTransform) return;
            _subtitleContainerTransform.LookAt(_cameraTransform);
        }
        #endregion
    }
}
