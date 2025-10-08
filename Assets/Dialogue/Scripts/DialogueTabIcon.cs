using UnityEngine;
using UnityEngine.UI;

namespace MedicalTraining.Dialogue
{
    /// <summary>
    /// Controls the icon for each tab in the top row.
    /// </summary>
    public class DialogueTabIcon : MonoBehaviour
    {
        public Image image;
        public Sprite ActiveSprite;
        public Sprite InactiveSprite;

        public void SetSelectedState(bool active)
        {
            image.sprite = active ? ActiveSprite : InactiveSprite;
        }
    }
}
