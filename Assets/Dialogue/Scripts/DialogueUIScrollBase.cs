using Tweens;
using UnityEngine;

namespace MedicalTraining.Dialogue
{
    /// <summary>
    /// Handles the scrolling animations between tabs.
    /// </summary>
    public class DialogueUIScrollBase : MonoBehaviour
    {
        public RectTransform Viewport;
        public RectTransform ContentPane;
        public float ScrollDuration = 0.3f;
        public AnimationCurve AnimationCurve;

        private void Awake()
        {
            if (ContentPane.rect.width % Viewport.rect.width != 0)
                Debug.LogWarning("Dialogue UI content does not evenly fit into the viewport!");
        }

        /// <summary>
        /// Moves the ContentPane, so that targetPage is shown in the viewport and
        /// it resizes the viewport horizontally to fit the targetPage.
        /// </summary>
        public void SwitchToPage(RectTransform targetPage)
        {
            moveContentPaneToPage(targetPage);
            resizeViewportToFitPage(targetPage);
        }

        private void moveContentPaneToPage(RectTransform targetPage)
        {
            var tween = new AnchoredPositionXTween
            {
                to = -targetPage.anchoredPosition.x,
                duration = ScrollDuration,
                animationCurve = AnimationCurve
            };
            ContentPane.gameObject.AddTween(tween);
        }

        private void resizeViewportToFitPage(RectTransform targetPage)
        {
            var tween = new FloatTween()
            {
                from = Viewport.sizeDelta.y,
                to = targetPage.sizeDelta.y,
                duration = ScrollDuration,
                animationCurve = AnimationCurve,
                onUpdate = (_, value) => Viewport.sizeDelta = new Vector2(Viewport.sizeDelta.x, value)
            };
            Viewport.gameObject.AddTween(tween);
        }
    }
}