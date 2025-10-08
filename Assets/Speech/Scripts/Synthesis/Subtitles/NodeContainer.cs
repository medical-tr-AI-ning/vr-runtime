using System.Collections.Generic;
using UnityEngine;

namespace Speech.Scripts.Synthesis
{
    /// <summary>
    /// Base Renderer class
    /// </summary>
    public class NodeContainer : MonoBehaviour
    {
        #region variables
        protected GameObject _NodeContainer;
        protected List<GameObject> _NodeGameObjects = new List<GameObject>();
        #endregion

        #region unity_functions
        protected virtual void Start()
        {
            _NodeContainer = new GameObject("NodeContainer", typeof(Canvas));
            _NodeContainer.transform.SetParent(transform, false);
        }
        #endregion

        protected void ResetDialogueGameObjectPosition(GameObject gObj)
        {
            RectTransform rectTransform = gObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
            rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        }

        protected void DestroyExistingNodeGameObjects()
        {
            foreach (GameObject gObj in _NodeGameObjects)
            {
                Destroy(gObj);
            }
        }
    }
}