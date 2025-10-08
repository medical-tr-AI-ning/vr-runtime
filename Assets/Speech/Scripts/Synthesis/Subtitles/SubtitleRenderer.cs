using UnityEngine;
using UnityEngine.UI;

namespace Speech.Scripts.Synthesis
{
    public class SubtitleRenderer : MonoBehaviour
    {
        #region variables
        private string _lastSubtitleText;

        [SerializeField] private TMPro.TMP_Text _subtitleText;
        #endregion

        #region unity_functions
        private void Start()
        {
            if (!_subtitleText)
            {
                throw new UnassignedReferenceException("Reference for Subtitle Text Object not found.");
            }
        }
        #endregion

        #region public_functions
        /// <summary>
        /// Returns last displayed subtitle text
        /// </summary>
        public string LastSubtitleText { get => _lastSubtitleText;}

        /// <summary>
        /// Function to update subtitle text
        /// </summary>
        /// <param name="responseText">text to display as subtitle</param>
        public void UpdateSubtitleText(string responseText)
        {
            _lastSubtitleText = responseText;
            _subtitleText.text = _lastSubtitleText;
        }
        #endregion
    }
}
