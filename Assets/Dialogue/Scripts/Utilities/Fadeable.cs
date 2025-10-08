using Tweens;
using UnityEngine;

namespace MedicalTraining.Dialogue.Utilities
{
    /// <summary>
    /// Utility behavior to handle animations for fading elements in and out.
    /// </summary>
   public class Fadeable : MonoBehaviour
   {
      private CanvasGroup _canvasGroup;
   
      public float FadeDuration = 2f;
      public AnimationCurve FadeAnimation;
      public bool ChangeActiveState;
   
      public void Awake()
      {
         _canvasGroup = GetComponent<CanvasGroup>();
      }

      public void Fade(bool targetVisibleState)
      {
         if (IsVisible() == targetVisibleState) return;
         if(targetVisibleState) FadeIn();
         else FadeOut();
      }

      public void FadeIn()
      {
         if(ChangeActiveState) gameObject.SetActive(true);
         var tween = _alphaTween(1);
         _canvasGroup.blocksRaycasts = true;
         _canvasGroup.gameObject.AddTween(tween);
      }
   
      public void FadeOut()
      {
         var tween = _alphaTween(0);
         tween.onEnd = (instance) =>
         {
            _canvasGroup.blocksRaycasts = false;
            if(ChangeActiveState) gameObject.SetActive(false);
         };
         _canvasGroup.gameObject.AddTween(tween);
      }

      public bool IsVisible() => _canvasGroup.alpha > 0;

      public FloatTween _alphaTween(float target)
      {
         var tween = new FloatTween
         {
            from = _canvasGroup.alpha,
            to = target,
            duration = FadeDuration,
            animationCurve = FadeAnimation,
            onUpdate = (instance, value) => {
               _canvasGroup.alpha = value;
            }
         };
         return tween;
      }
   }
}
