using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Support for new Input System
#if ENABLE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
#endif

public class EventSystem_InitialSelection : MonoBehaviour
{
    #region variables
    [SerializeField] private GameObject _initialSelection;
    #endregion

    private void Start()
    {
        EventSystem eventSystem = EventSystem.current;
        if(eventSystem == null)
        {
            Debug.LogWarning("Could not set initial EventSystem selection. No EventSystem found.", this);
            return;
        }
        
        // Simulate mouse click for InputFields.
        InputField inputfield = _initialSelection.GetComponent<InputField>();
        if (inputfield != null)
            inputfield.OnPointerClick(new PointerEventData(eventSystem));
        
        eventSystem.SetSelectedGameObject(_initialSelection);
    }
}
