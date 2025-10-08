using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
        public class PreviewImageLoader : MonoBehaviour
    {
        [SerializeField] private Image _previewImage;
        [SerializeField] private Sprite _missingPreviewSprite;
        [SerializeField] private List<PreviewImageMapping> _mappings = new List<PreviewImageMapping>();
        

        public void SetPreviewImage(string agentID)
        {
            try
            {
                _previewImage.sprite = _mappings.Find(mapping => mapping.AgentID == agentID).Sprite;
            }
            catch (Exception)
            {
                Debug.LogError($"Error finding preview image for {agentID}");
                _previewImage.sprite = _missingPreviewSprite;
            }
            
        }

        [Serializable]
        public class PreviewImageMapping
        {
            public string AgentID;
            public Sprite Sprite;
        }
    }
    
}