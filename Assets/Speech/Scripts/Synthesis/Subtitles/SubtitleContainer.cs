using UnityEngine;

namespace Speech.Scripts.Synthesis
{
    public class SubtitleContainer : NodeContainer
    {
        #region variables        
        [SerializeField] private GameObject _subtitlePrefab;

        [Header("Container RectTransform Setup")]
        [SerializeField] private float _rectWidth = 1000;
        [SerializeField] private float _rectHeight = 180;

        [SerializeField, Range(0, 1)] private float _rectScaleX = 0.001f;
        [SerializeField, Range(0, 1)] private float _rectScaleY = 0.001f;
        #endregion

        #region unity_functions
        protected override void Start()
        {
            base.Start();
            RectTransform t = _NodeContainer.GetComponent<RectTransform>();
            t.sizeDelta = new Vector2(_rectWidth, _rectHeight);
            t.localScale = new Vector3(_rectScaleX, _rectScaleY, 1);
        }
        #endregion

        #region public_functions
        /// <summary>
        /// Hide Subtitle
        /// </summary>
        public void HideSubtitle()
        {
            DestroyExistingNodeGameObjects();
        }

        /// <summary>
        /// Show Subtitle containing text.
        /// </summary>
        /// <param name="subtitle">Text inside subtitle box</param>
        public void ShowSubtitle(string subtitle)
        {
            DestroyExistingNodeGameObjects();

            var obj = Instantiate(_subtitlePrefab, new Vector3(0, 0, 0), Quaternion.identity, _NodeContainer.transform);
            ResetDialogueGameObjectPosition(obj);

            SubtitleRenderer renderer = obj.GetComponent<SubtitleRenderer>();
            renderer.UpdateSubtitleText(subtitle);

            _NodeGameObjects.Add(obj);
        }
        #endregion
    }
}
