using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MedicalTraining.Dialogue
{
    /// <summary>
    /// Container of menue options. Sets up the navigation settings for the contained buttons (options).
    /// </summary>
    public class DialogueOptionList : MonoBehaviour
    {
        [SerializeField] public Transform ButtonsContainer;
        private List<Button> instantiatedButtons;

        [SerializeField] private DialogueOptionList _previousOptions;
        [SerializeField] private DialogueOptionList _nextOptions;
        private GameObject _savedSelection;

        public void Start()
        {
            UpdateNavigation();
        }

        public void Awake()
        {
            instantiatedButtons = ButtonsContainer.GetComponentsInChildren<Button>().ToList();
        }

        /// <summary>
        /// Sets up navigation for each button, using the native Unity Navigation System.
        ///This is equal to opening the button in the inspector, setting the Navigation to explicit
        ///and selecting the previous and next button as targets for SelectOnUp and SelectOnDown.
        /// </summary>
        public void UpdateNavigation()
        {
            for (int i = 0; i < instantiatedButtons.Count; i++)
            {
                var prevButton = instantiatedButtons[(i - 1 + instantiatedButtons.Count) % instantiatedButtons.Count];
                var nextButton = instantiatedButtons[(i + 1) % instantiatedButtons.Count];
                Navigation nav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = (i == 0 && _previousOptions is not null && _previousOptions.HasButtons())
                        ? _previousOptions.GetLastButton()
                        : prevButton,
                    selectOnDown = (i == instantiatedButtons.Count - 1 && _nextOptions is not null &&
                                    _nextOptions.HasButtons())
                        ? _nextOptions.GetFirstButton()
                        : nextButton
                };
                instantiatedButtons[i].navigation = nav;
            }
        }

        /// <summary>
        ///  Store the currently selected gameObject for later restoring.
        /// </summary>
        public void SaveSelection() => _savedSelection = EventSystem.current.currentSelectedGameObject;


        /// <summary>
        /// Selects the first (or previously selected) button (so that is is "highlighted").
        /// OnFocus must be called each time that the DialogueOptionList comes into focus
        /// (e.g. after closing a submenue).
        /// </summary>
        public void OnFocus()
        {
            //Pass OnFocus event to next options if we have no elements to select
            if (instantiatedButtons.Count == 0)
            {
                if (_nextOptions is null) return;
                _nextOptions.OnFocus();
                return;
            }

            var eventSystem = EventSystem.current;
            var newSelectedGameObject =
                _savedSelection ?? instantiatedButtons.First().gameObject;
            eventSystem.SetSelectedGameObject(newSelectedGameObject, new BaseEventData(eventSystem));
            _savedSelection = null;
        }

        public Button GetFirstButton() => instantiatedButtons.Count > 0 ? instantiatedButtons[0] : null;
        public Button GetLastButton() => instantiatedButtons.Count > 0 ? instantiatedButtons[^1] : null;

        public bool HasButtons() => instantiatedButtons.Count > 0;

        /// <summary>
        /// Sets the previous and the next OptionList which will make it possible to navigate to them.
        /// </summary>
        public void SetNeighboringOptionLists(DialogueOptionList previousOptions, DialogueOptionList nextOptions)
        {
            _previousOptions = previousOptions;
            _nextOptions = nextOptions;
            UpdateNavigation();
        }
    }
}