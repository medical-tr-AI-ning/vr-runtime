using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using MedicalTraining.Dialogue.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MedicalTraining.Dialogue
{
    /// <summary>
    /// Handles Navigation between Tabs and Submenues.
    /// Receives Navigation and Submit Events (from DialogueUIInputObserver)
    /// and forwards them to Native Unity Navigation. 
    /// </summary>
    public class DialogueUIController : MonoBehaviour
    {
        [SerializeField] private int _initialTab;
        [FormerlySerializedAs("_navigationButtonsContainer")] [SerializeField] private GameObject _tabIconsContainer;
        [SerializeField] private GameObject _tabsContainer;
        [SerializeField] private GameObject _submenuesContainer;
        [SerializeField] private Fadeable _indicatorLeft;
        [SerializeField] private Fadeable _indicatorRight;
        [SerializeField] private Fadeable _submenuesBackground;
        [SerializeField] private DialogueUIScrollBase _scrollBase;
        [SerializeField] private DialogueOptionList _specialOptions;
        private CanvasGroup _canvasGroup;
        private bool _visible = false;
        private int _activeTabIndex = 0;
        private DialogueSubmenue _activeSubmenue;
        private DialogueOptionList[] _tabs;
        private DialogueSubmenue[] _submenues;
        private DialogueTabIcon[] _tabIcons;
        private GameObject _currentSelection;

        void Start()
        {
            _tabs = _tabsContainer.GetComponentsInChildren<DialogueOptionList>();
            _submenues = _submenuesContainer.GetComponentsInChildren<DialogueSubmenue>();
            _tabIcons = _tabIconsContainer.GetComponentsInChildren<DialogueTabIcon>();
            _canvasGroup = GetComponent<CanvasGroup>();
            updateNavigationIndicators();
            if (DialogueUIActionHandler.Instance == null)
                Debug.LogWarning("A DialogueUIActionHandler is missing in this scene! \n" +
                                 "If unsure, add a 'DialogueUIDefaultActionHandler' Component to the scene!");
            
            StartCoroutine(_initializeAfterFirstFrame());
        }

        /// <summary>
        /// Unity needs a frame to sort out its auto-resizing mechanisms after starting the application.
        /// We want to continue with our initialization after that.
        /// </summary>
        private IEnumerator _initializeAfterFirstFrame()
        {
            yield return 0;
            switchToTab(_initialTab);
        }

        private void updateNavigationButtons()
        {
            foreach (var button in _tabIcons)
            {
                button.SetSelectedState(false);
            }

            _tabIcons[_activeTabIndex].SetSelectedState(true);
        }

        private void tryCycleTabs(bool forwardDirection)
        {
            if (_activeSubmenue is not null) return;

            if (forwardDirection)
            {
                if (_activeTabIndex == _tabs.Length - 1) return;
                switchToTab(_activeTabIndex + 1);
            }
            else
            {
                if (_activeTabIndex == 0) return;
                switchToTab(_activeTabIndex - 1);
            }
        }

        private void switchToTab(int index)
        {
            _activeTabIndex = index;
            _scrollBase.SwitchToPage(_tabs[_activeTabIndex].GetComponent<RectTransform>());
            _tabs[_activeTabIndex].OnFocus();
            updateNavigationButtons();
            _specialOptions.SetNeighboringOptionLists(_tabs[_activeTabIndex], _tabs[_activeTabIndex]);
            updateNavigationIndicators();
        }

        public void OpenSubmenue(string submenueName)
        {
            DialogueSubmenue submenue = getSubmenueByName(submenueName);
            if (submenue is null)
            {
                Debug.LogWarning($"Submenue with name {submenueName} does not exist");
                return;
            }

            _tabs[_activeTabIndex].SaveSelection();
            submenue.Show();
            _submenuesBackground.FadeIn();
            _activeSubmenue = submenue;
            updateNavigationIndicators();
        }

        public void CloseSubmenue()
        {
            if (_activeSubmenue == null) return;
            _activeSubmenue.Hide();
            _activeSubmenue = null;
            focusActiveTab();
            _submenuesBackground.FadeOut();
            updateNavigationIndicators();
        }

        private void updateNavigationIndicators()
        {
            bool leftVisible = true;
            bool rightVisible = true;
            if (_activeSubmenue != null)
            {
                leftVisible = false;
                rightVisible = false;
            }
            else
            {
                if (_activeTabIndex == 0) leftVisible = false;
                if (_activeTabIndex == _tabs.Length - 1) rightVisible = false;
            }

            _indicatorLeft.Fade(leftVisible);
            _indicatorRight.Fade(rightVisible);
        }

        private void focusActiveTab() => _tabs[_activeTabIndex].OnFocus();

        private DialogueSubmenue getSubmenueByName(string submenueName) =>
            _submenues.First(submenue => submenue.SubmenueName.Equals(submenueName));
        
        /// <summary>
        /// Selects the most recently selected button to restore the selection if it's lost.
        /// </summary>
        public void RestoreSelection()
        {
            Selectable currentSelectable = _currentSelection?.GetComponent<Selectable>();
            if(currentSelectable) currentSelectable.Select();
        }

        #region Input Event Actions

        public void OnNavigationEvent(MoveDirection direction)
        {
            if (direction == MoveDirection.Down || direction == MoveDirection.Up)
                InputUtils.SimulateInputSystemNavigationEvent(direction);

            if (direction == MoveDirection.Right)
            {
                tryCycleTabs(true);
            }

            if (direction == MoveDirection.Left)
            {
                tryCycleTabs(false);
            }

            _currentSelection = EventSystem.current.currentSelectedGameObject;
        }

        public void OnSubmitEvent()
        {
            InputUtils.SimulateInputSystemSelectEvent();
        }

        public void OnToggleVisibleEvent()
        {
            _visible = !_visible;
            _canvasGroup.alpha = _visible ? 1 : 0;
        }

        #endregion
    }
}